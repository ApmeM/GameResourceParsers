public class SkipFileConverter<T> : BaseFileConverter<T> where T : BaseFile
{
    protected override IEnumerable<BaseFile> ConvertFile(T toConvert, List<BaseFile> files)
    {
        yield break;
    }
}