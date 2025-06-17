using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class SpriteToSingleImageConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteMergeVariant != GameParserConfigurator.SpriteMergerFlags.SingleSprite)
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
            var newWidth = toConvert.Levels.Max(a => a.Sprite.Max(b => b.Width));
            var newHeight = toConvert.Levels.Max(a => a.Sprite.Max(b => b.Height));

            var newColumns = toConvert.Levels.Max(a => a.Sprite.Count);
            var newRows = toConvert.Levels.Count();

            var newImage = new Image<Rgba32>(newColumns * newWidth, newRows * newHeight);

            var i = 0;
            foreach (var s in toConvert.Levels)
            {
                for (var j = 0; j < s.Sprite.Count; j++)
                {
                    newImage.Mutate(a => a.DrawImage(s.Sprite[i], new Point(newWidth * j, newHeight * i), 1));
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