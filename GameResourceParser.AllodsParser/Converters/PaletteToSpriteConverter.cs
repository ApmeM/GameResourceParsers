using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AllodsParser
{
    public class PaletteToSpriteConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<SpritesWithPalettesFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<SpriteFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
        {
            var units = files.OfType<RegUnitsFile>().First();
            var unit = units.Units.FirstOrDefault(a => toConvert.relativeFilePath.Contains(a.File, StringComparison.InvariantCultureIgnoreCase));

            var path = toConvert.relativeFileDirectory.Split("/");

            if (toConvert.Palettes.Count == 0)
            {
                yield return new SpriteFile
                {
                    Levels = new List<SpriteFile.SpriteLevel>{
                        BuildLevel(unit, toConvert.Sprites)
                    },
                    relativeFileExtension = ".png",
                    relativeFileDirectory = unit == null ? toConvert.relativeFileDirectory : Path.Combine(toConvert.relativeFileDirectory, ".."),
                    relativeFileName = unit == null ? toConvert.relativeFileName : path[path.Length - 1],
                };
            }

            var newLevels = new List<SpriteFile.SpriteLevel>();

            for (var j = 0; j < toConvert.Palettes.Count; j++)
            {
                var newImages = new List<Image<Rgba32>>();
                for (int i = 0; i < toConvert.Sprites.Count; i++)
                {
                    var f = toConvert.Sprites[i];
                    var p = toConvert.Palettes[j];

                    var newImage = new Image<Rgba32>(f.Width, f.Height);
                    for (var x = 0; x < f.Width; x++)
                        for (var y = 0; y < f.Height; y++)
                        {
                            var color = p[f[x, y].R, 0];
                            color.A = f[x, y].A;
                            newImage[x, y] = color;
                        }
                    newImages.Add(newImage);
                }

                newLevels.Add(BuildLevel(unit, newImages));
            }

            yield return new SpriteFile
            {
                Levels = newLevels,
                relativeFileExtension = ".png",
                relativeFileDirectory = unit == null ? toConvert.relativeFileDirectory : Path.Combine(toConvert.relativeFileDirectory, ".."),
                relativeFileName = unit == null ? toConvert.relativeFileName : path[path.Length - 1],
            };
        }

        private SpriteFile.SpriteLevel BuildLevel(RegUnitsFile.UnitFileContent unit, List<Image<Rgba32>> sprites)
        {
            if (unit == null)
            {
                return new SpriteFile.SpriteLevel
                {
                    StayBottom = sprites
                };
            }

            return new SpriteFile.SpriteLevel
            {
                StayBottom = sprites.Skip(2 * 0).Take(1).ToList(),
                StayBottomLeft = sprites.Skip(2 * 1).Take(1).ToList(),
                StayLeft = sprites.Skip(2 * 2).Take(1).ToList(),
                StayTopLeft = sprites.Skip(2 * 3).Take(1).ToList(),
                StayTop = sprites.Skip(2 * 4).Take(1).ToList(),
                StayTopRight = sprites.Skip(2 * 5).Take(1).ToList(),
                StayRight = sprites.Skip(2 * 6).Take(1).ToList(),
                StayBottomRight = sprites.Skip(2 * 7).Take(1).ToList(),

                MoveBottom = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 0).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveBottomLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 1).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 2).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveTopLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 3).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveTop = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 4).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveTopRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 5).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 6).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),
                MoveBottomRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 7).Take(unit.MovePhases + unit.MoveBeginPhases).ToList(),

                AttackBottom = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 0).Take(unit.AttackPhases).ToList(),
                AttackBottomLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 1).Take(unit.AttackPhases).ToList(),
                AttackLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 2).Take(unit.AttackPhases).ToList(),
                AttackTopLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 3).Take(unit.AttackPhases).ToList(),
                AttackTop = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 4).Take(unit.AttackPhases).ToList(),
                AttackTopRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 5).Take(unit.AttackPhases).ToList(),
                AttackRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 6).Take(unit.AttackPhases).ToList(),
                AttackBottomRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 7).Take(unit.AttackPhases).ToList(),

                DieBottom = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 0).Take(unit.DyingPhases).ToList(),
                DieBottomLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 1).Take(unit.DyingPhases).ToList(),
                DieLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 2).Take(unit.DyingPhases).ToList(),
                DieTopLeft = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 3).Take(unit.DyingPhases).ToList(),
                DieTop = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 4).Take(unit.DyingPhases).ToList(),
                DieTopRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 5).Take(unit.DyingPhases).ToList(),
                DieRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 6).Take(unit.DyingPhases).ToList(),
                DieBottomRight = sprites.Skip(2 * 8 + (unit.MovePhases + unit.MoveBeginPhases) * 8 + unit.AttackPhases * 8 + unit.DyingPhases * 7).Take(unit.DyingPhases).ToList(),
            };
        }
    }
}