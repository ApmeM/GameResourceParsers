public class StringFile : BaseFile
{
    public string Data;

    protected override void SaveInternal(string outputFileName)
    {
        File.WriteAllText(outputFileName, Data);
    }
}