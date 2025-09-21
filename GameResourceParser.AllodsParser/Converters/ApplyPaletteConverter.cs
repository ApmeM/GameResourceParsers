using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Final converter of allods files to common files that are available for save to other formats like godot, tmx, etc.
/// </summary>
public class ApplyPaletteConverter : BaseFileConverter<SpritesWithPalettesFile>
{
    protected override IEnumerable<SpriteFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
    {
        if (toConvert.Palettes.Count == 0)
        {
            yield return new SpriteFile
            {
                Sprites = toConvert.Sprites,
                relativeFileExtension = ".png",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName,
            };
            yield break;
        }

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

            yield return new SpriteFile
            {
                Sprites = newImages,
                relativeFileExtension = ".png",
                relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, toConvert.relativeFileName),
                relativeFileName = toConvert.relativeFileName + "." + j
            };
        }
    }
}