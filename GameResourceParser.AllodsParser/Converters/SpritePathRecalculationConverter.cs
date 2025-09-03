namespace AllodsParser
{
    public class SpritePathRecalculationConverter : BaseFileConverter<SpritesWithPalettesFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
        {
            var units = files.OfType<RegUnitsFile>().First();

            var unit = units.Units.FirstOrDefault(a => toConvert.relativeFilePath.Replace("heroes", "humans").Contains(a.File, StringComparison.InvariantCultureIgnoreCase));

            if (unit != null)
            {
                Console.WriteLine($"Change path from {toConvert.relativeFilePath}");

                var path = toConvert.relativeFileDirectory.Split("/", StringSplitOptions.RemoveEmptyEntries);
                toConvert.relativeFileName = toConvert.relativeFileName.Replace("sprites", path[path.Length - 1]);

                Console.WriteLine($"Change path to   {toConvert.relativeFilePath}");
            }

            yield return toConvert;
            yield break;
        }
    }
}