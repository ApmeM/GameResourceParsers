public class BOXFileLoader : BaseFileLoader
{
    protected override BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br)
    {
        return new BinaryFile
        {
            Data = br.ReadBytes((int)ms.Length)
        };
    }
}