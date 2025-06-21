using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class SpriteToSingleImageConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteOutput != GameParserConfigurator.SpriteOutputFormat.SingleSprite)
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
            if (toConvert.Levels.SelectMany(a => a.AllSprites).Count() == 0)
            {
                Console.Error.WriteLine($"Sprite {toConvert.relativeFilePath} does not have sprites converted.");
                yield break;
            }

            var newWidth = toConvert.Levels.SelectMany(a => a.AllSprites).Max(a => a.Width);
            var newHeight = toConvert.Levels.SelectMany(a => a.AllSprites).Max(a => a.Height);

            var newColumns = toConvert.Levels.Max(a => a.AllSprites.Count);
            var newRows = toConvert.Levels.Count();

            var newImage = new Image<Rgba32>(newColumns * newWidth, newRows * newHeight);

            var i = 0;
            foreach (var s in toConvert.Levels)
            {
                for (var j = 0; j < s.AllSprites.Count; j++)
                {
                    newImage.Mutate(a => a.DrawImage(s.AllSprites[j], new Point(newWidth * j, newHeight * i), 1));
                }
                i++;
            }
            yield return new ImageFile
            {
                Image = newImage,
                relativeFileExtension = ".png",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName
            };
        }
    }
}