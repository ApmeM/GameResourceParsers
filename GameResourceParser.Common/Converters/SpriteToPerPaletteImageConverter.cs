using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class SpriteToPerLevelImageConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteOutput != GameParserConfigurator.SpriteOutputFormat.PerLevelSprite)
            {
                return;
            }

            var oldFiles = files
                .OfType<SpriteFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<ImageFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
        {
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