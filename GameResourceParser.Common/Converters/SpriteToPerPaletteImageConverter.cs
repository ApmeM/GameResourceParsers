using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class SpriteToPerLevelImageConverter : BaseFileConverter<SpriteFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteOutput != GameParserConfigurator.SpriteOutputFormat.PerLevelSprite)
            {
                yield return toConvert;
                yield break;
            }

            for (var i = 0; i < toConvert.Levels.Count; i++)
            {
                if (toConvert.Levels[i].AllSprites.Count == 0)
                {
                    Console.Error.WriteLine($"Sprite {toConvert.relativeFilePath} does not have sprites converted.");
                    yield break;
                }

                var newWidth = toConvert.Levels[i].AllSprites.Max(a => a.Width);
                var newHeight = toConvert.Levels[i].AllSprites.Max(a => a.Height);

                var newImage = new Image<Rgba32>(toConvert.Levels[i].AllSprites.Count * newWidth, newHeight);

                for (int j = 0; j < toConvert.Levels[i].AllSprites.Count; j++)
                {
                    newImage.Mutate(a => a.DrawImage(toConvert.Levels[i].AllSprites[j], new Point(newWidth * j, 0), 1));
                }

                yield return new ImageFile
                {
                    Image = newImage,
                    relativeFileExtension = ".png",
                    relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, toConvert.relativeFileName),
                    relativeFileName = toConvert.relativeFileName + "." + i
                };
            }
        }
    }
}