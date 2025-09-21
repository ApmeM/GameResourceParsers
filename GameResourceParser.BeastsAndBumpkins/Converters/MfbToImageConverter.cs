using System.Numerics;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class MfbToImageConverter : BaseFileConverter<BinaryFile>
{
    protected override IEnumerable<BaseFile> ConvertFile(BinaryFile toConvert, List<BaseFile> files)
    {
        if (!toConvert.relativeFileExtension.Contains("MFB", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return toConvert;
            yield break;
        }

        var data = toConvert.Data;
        using var stream = new MemoryStream(data);
        using var binr = new BinaryReader(stream, Encoding.ASCII);

        if (Encoding.ASCII.GetString(binr.ReadBytes(3)) != "MFB")
        {
            throw new Exception("MFB file is corrupted. Cant read header.");
        }

        var version = int.Parse(new string(binr.ReadChars(3))); // 101

        if (version != 101)
        {
            Console.Error.WriteLine("MFB file version is not supported.");
            yield break;

        }

        var Width = binr.ReadInt16();
        var Height = binr.ReadInt16();
        var Offset = new Point(binr.ReadInt16(), binr.ReadInt16());

        var flags = binr.ReadInt16();
        var IsTransparent = (flags & (byte)EntryFlags.Transparent) != 0;
        var IsUnknown = (flags & (byte)EntryFlags.Unknown) != 0;
        var IsCompressed = (flags & (byte)EntryFlags.Compressed) != 0;

        var NumSptites = binr.ReadInt16();

        var spritesize = Width * Height;


        var sprites = new List<byte[]>();
        for (var i = 0; i < NumSptites; i++)
        {
            byte[] buffer;
            if (IsCompressed)
            {
                var size = binr.ReadInt32();
                buffer = Utils.UnpackRLE(binr.ReadBytes(size), spritesize);
            }
            else
            {
                buffer = binr.ReadBytes(spritesize);
            }

            sprites.Add(buffer);
        }
#if MULTIPALETTE
        foreach (var palette in Enum.GetValues<Palettes.TypePalette>())
        {
#else
        var palette = Palettes.TypePalette.PALETTE;
#endif
        var result = new List<Image<Rgba32>>();

        foreach (var buffer in sprites)
        {
            var p = Palettes.GetPalette(palette);
            var img = new Image<Rgba32>(Width, Height);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int curPixel = x + y * img.Width;
                    img[x, y] = new Rgba32(p[buffer[curPixel] * 4], p[buffer[curPixel] * 4 + 1], p[buffer[curPixel] * 4 + 2], p[buffer[curPixel] * 4 + 3]);
                }
            }

            bool firstColorSet = false;
            Vector4 firstColor = default;

            if (IsTransparent)
            {
                img.Mutate(x => x.ProcessPixelRowsAsVector4(row =>
                {
                    for (int x = 0; x < row.Length; x++)
                    {
                        if (!firstColorSet)
                        {
                            firstColor = row[x];
                            firstColorSet = true;
                        }

                        if (row[x] == firstColor)
                        {
                            row[x].W = 0;
                        }
                    }
                }));
            }

            result.Add(img);
        }

        yield return new SpriteFile
        {
#if MULTIPALETTE
            relativeFileDirectory = Path.Join(toConvert.relativeFileDirectory, toConvert.relativeFileName),
            relativeFileName = palette.ToString(),
#else
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = toConvert.relativeFileName,
#endif
            relativeFileExtension = toConvert.relativeFileExtension,
            Sprites = result
        };
#if MULTIPALETTE
        }
#endif
    }

    private enum EntryFlags
    {
        Transparent = 1,
        Compressed = 2,
        Unknown = 4
    }
}