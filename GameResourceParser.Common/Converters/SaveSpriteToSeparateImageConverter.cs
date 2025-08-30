namespace AllodsParser
{
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

            var filename = toConvert.relativeFileName[0].ToString().ToUpper() + toConvert.relativeFileName.Substring(1);

            for (int i = 0; i < toConvert.Sprites.Count; i++)
            {
                var newImage = toConvert.Sprites[i];
                var image = new ImageFile
                {
                    Image = newImage,
                    relativeFileExtension = ".png",
                    relativeFileDirectory = toConvert.relativeFileDirectory,
                    relativeFileName = filename + "." + i
                };

                image.Save(outputDirectory);
            }
        }
    }
}