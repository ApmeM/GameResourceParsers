namespace AllodsParser
{
    public class SaveFileConverter<T> : BaseFileConverter<T> where T : BaseFile
    {
        private readonly string outputDirectory;

        public SaveFileConverter(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }
        protected override IEnumerable<BaseFile> ConvertFile(T toConvert, List<BaseFile> files)
        {
            toConvert.Save(outputDirectory);
            yield break;
        }
    }
}