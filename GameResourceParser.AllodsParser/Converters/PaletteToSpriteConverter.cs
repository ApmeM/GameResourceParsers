using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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

            var spriteLevel = new SpriteFile.SpriteLevel();
            var baseSkip = 0;

            spriteLevel.StayLength = 0.1f;
            spriteLevel.StayBottom = sprites.Skip(2 * 0).Take(1).ToList();
            spriteLevel.StayBottomLeft = sprites.Skip(2 * 1).Take(1).ToList();
            spriteLevel.StayLeft = sprites.Skip(2 * 2).Take(1).ToList();
            spriteLevel.StayTopLeft = sprites.Skip(2 * 3).Take(1).ToList();
            spriteLevel.StayTop = sprites.Skip(2 * 4).Take(1).ToList();
            spriteLevel.StayTopRight = unit.Flip
                ? spriteLevel.StayTopLeft.Select(FlipH).ToList()
                : sprites.Skip(2 * 5).Take(1).ToList();
            spriteLevel.StayRight = unit.Flip
                ? spriteLevel.StayLeft.Select(FlipH).ToList()
                : sprites.Skip(2 * 6).Take(1).ToList();
            spriteLevel.StayBottomRight = unit.Flip
                ? spriteLevel.StayBottomLeft.Select(FlipH).ToList()
                : sprites.Skip(2 * 7).Take(1).ToList();

            baseSkip += unit.Flip ? 2 * 5 - 1 : 2 * 8;

            spriteLevel.MoveLength = unit.MoveAnimFrame.Sum() / 10f;
            spriteLevel.MoveBottom = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 0).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveBottomLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 1).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 2).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveTopLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 3).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveTop = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 4).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveTopRight = unit.Flip
                ? spriteLevel.MoveTopLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 5).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveRight = unit.Flip
                ? spriteLevel.MoveLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 6).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();
            spriteLevel.MoveBottomRight = unit.Flip
                ? spriteLevel.MoveBottomLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 7).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();

            baseSkip += unit.Flip ? (unit.MovePhases + unit.MoveBeginPhases) * 5 : (unit.MovePhases + unit.MoveBeginPhases) * 8;

            spriteLevel.AttackLength = unit.AttackAnimTime.Sum() / 10f;
            spriteLevel.HitDelay = unit.AttackDelay / 10f;
            spriteLevel.AttackBottom = sprites.Skip(baseSkip + unit.AttackPhases * 0).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackBottomLeft = sprites.Skip(baseSkip + unit.AttackPhases * 1).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackLeft = sprites.Skip(baseSkip + unit.AttackPhases * 2).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackTopLeft = sprites.Skip(baseSkip + unit.AttackPhases * 3).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackTop = sprites.Skip(baseSkip + unit.AttackPhases * 4).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackTopRight = unit.Flip
                ? spriteLevel.AttackTopLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + unit.AttackPhases * 5).Take(unit.AttackPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();
            spriteLevel.AttackRight = unit.Flip
                ? spriteLevel.AttackLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + unit.AttackPhases * 6).Take(unit.AttackPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();
            spriteLevel.AttackBottomRight = unit.Flip
                ? spriteLevel.AttackBottomLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + unit.AttackPhases * 7).Take(unit.AttackPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();

            baseSkip += unit.Flip ? unit.AttackPhases * 5 : unit.AttackPhases * 8;

            spriteLevel.DieBottom = sprites.Skip(baseSkip + unit.DyingPhases * 0).Take(unit.DyingPhases).ToList();
            spriteLevel.DieBottomLeft = sprites.Skip(baseSkip + unit.DyingPhases * 1).Take(unit.DyingPhases).ToList();
            spriteLevel.DieLeft = sprites.Skip(baseSkip + unit.DyingPhases * 2).Take(unit.DyingPhases).ToList();
            spriteLevel.DieTopLeft = sprites.Skip(baseSkip + unit.DyingPhases * 3).Take(unit.DyingPhases).ToList();
            spriteLevel.DieTop = sprites.Skip(baseSkip + unit.DyingPhases * 4).Take(unit.DyingPhases).ToList();
            spriteLevel.DieTopRight = unit.Flip
                ? spriteLevel.DieTopLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + unit.DyingPhases * 5).Take(unit.DyingPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();
            spriteLevel.DieRight = unit.Flip
                ? spriteLevel.DieLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + unit.DyingPhases * 6).Take(unit.DyingPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();
            spriteLevel.DieBottomRight = unit.Flip
                ? spriteLevel.DieBottomLeft.Select(FlipH).ToList()
                : sprites.Skip(baseSkip + unit.DyingPhases * 7).Take(unit.DyingPhases).Select(a => unit.Flip ? FlipH(a) : a).ToList();

            return spriteLevel;
        }

        public Image<Rgba32> FlipH(Image<Rgba32> orig)
        {
            var copy = orig.Clone();
            copy.Mutate(x => x.Flip(FlipMode.Horizontal));
            return copy;
        }
    }
}