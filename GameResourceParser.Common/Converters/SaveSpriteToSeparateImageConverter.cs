public class SaveSpriteToSeparateImageConverter : BaseFileConverter<SpriteFile>
{
    private readonly string outputDirectory;

    public SaveSpriteToSeparateImageConverter(string outputDirectory)
    {
        this.outputDirectory = outputDirectory;
    }

    protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
    {
        yield return toConvert;

        for (int i = 0; i < toConvert.Sprites.Count; i++)
        {
            var newImage = toConvert.Sprites[i];
            var image = new ImageFile
            {
                Image = newImage,
                relativeFileExtension = ".png",
                relativeFileDirectory = Path.Join(toConvert.relativeFileDirectory, toConvert.relativeFileName),
                relativeFileName = toConvert.relativeFileName + "." + i
            };

            image.Save(outputDirectory);
        }
    }
}