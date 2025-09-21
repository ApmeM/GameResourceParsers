using SimpleTiled;

public class TmxFile : BaseFile
{
    public TmxMap Map;

    protected override void SaveInternal(string outputFileName)
    {
        using (var f = File.OpenWrite(outputFileName))
        {
            TiledHelper.Write(Map, f);
        }
    }
}