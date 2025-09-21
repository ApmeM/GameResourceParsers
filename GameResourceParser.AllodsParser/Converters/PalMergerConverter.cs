/// <summary>
/// Some units folders contains more then 1 palette file.
/// Merge them all together to a single PalFile .
/// </summary>
public class PalMergerConverter : BaseFileConverter<PalFile>
{
    protected override IEnumerable<PalFile> ConvertFile(PalFile toConvert, List<BaseFile> files)
    {
        if (!files.Contains(toConvert))
        {
            yield break;
        }

        var sameDirectory = files.Where(a => a.relativeFileDirectory == toConvert.relativeFileDirectory).OfType<PalFile>().ToList();

        yield return new PalFile
        {
            Palettes = sameDirectory.OrderBy(a => a.relativeFileName).SelectMany(a => a.Palettes).ToList(),
            relativeFileExtension = ".pal",
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = "palette"
        };
        sameDirectory.ForEach(a => files.Remove(a));
    }
}