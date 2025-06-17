using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AllodsParser
{
    public class PaletteToSpriteConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<SpritesWithPalettesFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<SpriteFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
        {
            if (toConvert.Palettes.Count == 0)
            {
                yield return new SpriteFile
                {
                    Levels = new List<SpriteFile.SpriteLevel>
                    {
                        new SpriteFile.SpriteLevel{
                            Sprite = toConvert.Sprites
                        }
                    },
                    relativeFileExtension = ".png",
                    relativeFileDirectory = toConvert.relativeFileDirectory,
                    relativeFileName = toConvert.relativeFileName
                };
            }

            var newLevels = new List<SpriteFile.SpriteLevel>();

            for (var j = 0; j < toConvert.Palettes.Count; j++)
            {
                var newImages = new List<Image<Rgba32>>();
                for (int i = 0; i < toConvert.Sprites.Count; i++)
                {
                    var f = toConvert.Sprites[i];
                    var p = toConvert.Palettes[j];

                    var newImage = new Image<Rgba32>(f.Width, f.Height);
                    for (var x = 0; x < f.Width; x++)
                        for (var y = 0; y < f.Height; y++)
                        {
                            var color = p[f[x, y].R, 0];
                            color.A = f[x, y].A;
                            newImage[x, y] = color;
                        }
                    newImages.Add(newImage);
                }

                newLevels.Add(new SpriteFile.SpriteLevel
                {
                    Sprite = newImages
                });
            }

            yield return new SpriteFile
            {
                Levels = newLevels,
                relativeFileExtension = ".png",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName
            };
        }
    }
}