using System.Text;

public class BoxUnpackConverter : BaseFileConverter<BinaryFile>
{
    protected override IEnumerable<BaseFile> ConvertFile(BinaryFile toConvert, List<BaseFile> files)
    {
        if (!toConvert.relativeFileExtension.Contains("BOX", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return toConvert;
            yield break;
        }

        var data = toConvert.Data;
        using var stream = new MemoryStream(data);
        using var binr = new BinaryReader(stream, Encoding.ASCII);

        if (Encoding.ASCII.GetString(binr.ReadBytes(3)) != "BOX")
        {
            throw new Exception("BOX file is corrupted. Cant read header.");
        }

        binr.BaseStream.Seek(5, SeekOrigin.Current);

        while (binr.PeekChar() != -1)
        {
            var entryName = Utils.TrimFromZero(new string(binr.ReadChars(256))).ToLower();
            var entryPath = Utils.TrimFromZero(new string(binr.ReadChars(256)));
            var timeYear = binr.ReadUInt16();
            var timeMonth = binr.ReadUInt16();
            var timeDOW = binr.ReadUInt16();
            var timeDay = binr.ReadUInt16();
            var timeHour = binr.ReadUInt16();
            var timeMinute = binr.ReadUInt16();
            var timeSecond = binr.ReadUInt16();
            var timeMills = binr.ReadUInt16();
            var size = binr.ReadInt32();
            var entryData = binr.ReadBytes(size);

            // if (entryName.EndsWith(".MFB", StringComparison.InvariantCultureIgnoreCase))
            // if (entryName.EndsWith(".MIS", StringComparison.InvariantCultureIgnoreCase))
            // if (entryName.EndsWith(".SAV", StringComparison.InvariantCultureIgnoreCase))

            var newBinary = new BinaryFile
            {
                Data = entryData,
                relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, toConvert.relativeFileName),
                relativeFileExtension = Path.GetExtension(entryName),
                relativeFileName = Path.GetFileNameWithoutExtension(entryName)
            };

            if (entryName.EndsWith(".BOX", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var b in ConvertFile(newBinary, files))
                {
                    yield return b;
                }
            }
            else
            {
                yield return newBinary;
            }
        }

    }
}