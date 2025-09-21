using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class SaveSpriteToSingleImageConverter : BaseFileConverter<SpriteFile>
{
    private readonly string outputDirectory;

    public SaveSpriteToSingleImageConverter(string outputDirectory)
    {
        this.outputDirectory = outputDirectory;
    }

    protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
    {
        yield return toConvert;

        var newWidth = toConvert.Sprites.Max(a => a.Width);
        var newHeight = toConvert.Sprites.Max(a => a.Height);

        var countWidth = ((int)Math.Sqrt(toConvert.Sprites.Count)) + 1;
        var countHeight = ((int)Math.Sqrt(toConvert.Sprites.Count)) + 1;

        var newImage = new Image<Rgba32>(countWidth * newWidth, countHeight * newHeight);

        for (int j = 0; j < toConvert.Sprites.Count; j++)
        {
            newImage.Mutate(a => a.DrawImage(toConvert.Sprites[j], new Point(newWidth * (j % countWidth), newHeight * (j / countWidth)), 1));
        }

        var image = new ImageFile
        {
            Image = newImage,
            relativeFileExtension = ".png",
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = toConvert.relativeFileName
        };

        image.Save(outputDirectory);
    }
}