namespace AllodsParser
{
    /// <summary>
    /// Apply palettes from PalFile in /units/ directory to sprite file in the same directory.
    /// Exception: palette.pal files under heroes, heroes_l and humans folder
    /// </summary>
    public class UnitsPalMergerConverter : BaseFileConverter<PalFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(PalFile toConvert, List<BaseFile> files)
        {
            if (!toConvert.relativeFileDirectory.Contains("/units/"))
            {
                yield return toConvert;
                yield break;
            }

            var oldFiles = files
                .OfType<SpritesWithPalettesFile>()
                .Where(a => a.relativeFileDirectory == toConvert.relativeFileDirectory)
                .ToList();

            if (oldFiles.Count != 0)
            {
                Update(toConvert, oldFiles);
                yield break;
            }

            if (!toConvert.relativeFilePath.EndsWith("palette.pal"))
            {
                Console.WriteLine($"Cant find Sprites for palette {toConvert.relativeFilePath}");
                yield break;
            }

            var allFilesInFolder = files
                .OfType<SpritesWithPalettesFile>()
                .Where(a => a.relativeFileDirectory.StartsWith(toConvert.relativeFileDirectory + "/"))
                .GroupBy(a => a.relativeFileDirectory)
                .ToList();

            foreach (var filesInFolder in allFilesInFolder)
            {
                Update(toConvert, filesInFolder.ToList());
            }
            yield break;
        }

        private void Update(PalFile toConvert, List<SpritesWithPalettesFile> oldFiles)
        {
            SpritesWithPalettesFile oldFile = null;

            if (oldFiles.Count == 2)
            {
                if (oldFiles[0].relativeFileName == oldFiles[1].relativeFileName + "b")
                {
                    oldFile = oldFiles[1];
                }
                if (oldFiles[0].relativeFileName + "b" == oldFiles[1].relativeFileName)
                {
                    oldFile = oldFiles[0];
                }
            }

            if (oldFile == null)
            {
                Console.WriteLine($"Too many sprites for palette {toConvert.relativeFilePath}");
                return;
            }

            oldFile.Palettes.Clear();
            oldFile.Palettes.AddRange(toConvert.Palettes);
        }
    }
}