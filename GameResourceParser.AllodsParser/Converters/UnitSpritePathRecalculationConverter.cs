public class UnitSpritePathRecalculationConverter : BaseFileConverter<SpritesWithPalettesFile>
{
    protected override IEnumerable<BaseFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
    {
        if (!toConvert.relativeFileDirectory.Contains("/units/"))
        {
            yield return toConvert;
            yield break;
        }

        var path = toConvert.relativeFileDirectory.Split("/", StringSplitOptions.RemoveEmptyEntries);
        toConvert.relativeFileName = toConvert.relativeFileName.Replace("sprites", path[path.Length - 1]);

        yield return toConvert;
        yield break;
    }
}