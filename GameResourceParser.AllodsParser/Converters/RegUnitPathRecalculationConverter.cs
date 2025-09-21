public class RegUnitPathRecalculationConverter : BaseFileConverter<RegUnitsFile>
{
    protected override IEnumerable<BaseFile> ConvertFile(RegUnitsFile toConvert, List<BaseFile> files)
    {
        foreach (var unit in toConvert.Units)
        {
            var path = unit.File.Split("/");
            var isB = path[path.Length - 1] == "spriteb" || path[path.Length - 1] == path[path.Length - 2] + "b";
            path[path.Length - 1] = path[path.Length - 2] + (isB ? "b" : "");
            unit.File = string.Join("/", path);
        }

        yield return toConvert;
    }
}