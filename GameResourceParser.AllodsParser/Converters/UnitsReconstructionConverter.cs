using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class UnitsReconstructionConverter : BaseFileConverter<SpritesWithPalettesFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
        {
            yield return toConvert;
         
            var units = files.OfType<RegUnitsFile>().First();

            var unit = units.Units.FirstOrDefault(a => toConvert.relativeFilePath.Replace("heroes", "humans").Contains(a.File, StringComparison.InvariantCultureIgnoreCase));

            if (unit == null || !unit.Flip)
            {
                yield break;
            }

            var baseSkip = 0;
            var sprites = toConvert.Sprites;

            var newSprites = new List<Image<Rgba32>>();

            newSprites.AddRange(sprites.Skip(2 * 0).Take(2));
            newSprites.AddRange(sprites.Skip(2 * 1).Take(2));
            newSprites.AddRange(sprites.Skip(2 * 2).Take(2));
            newSprites.AddRange(sprites.Skip(2 * 3).Take(2));
            newSprites.AddRange(sprites.Skip(2 * 4).Take(1));
            newSprites.AddRange(sprites.Skip(2 * 3).Take(2).Reverse().Select(FlipH));
            newSprites.AddRange(sprites.Skip(2 * 2).Take(2).Reverse().Select(FlipH));
            newSprites.AddRange(sprites.Skip(2 * 1).Take(2).Reverse().Select(FlipH));

            baseSkip += 2 * 5 - 1;

            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 0).Take(unit.MovePhases + unit.MoveBeginPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 1).Take(unit.MovePhases + unit.MoveBeginPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 2).Take(unit.MovePhases + unit.MoveBeginPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 3).Take(unit.MovePhases + unit.MoveBeginPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 4).Take(unit.MovePhases + unit.MoveBeginPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 3).Take(unit.MovePhases + unit.MoveBeginPhases).Select(FlipH));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 2).Take(unit.MovePhases + unit.MoveBeginPhases).Select(FlipH));
            newSprites.AddRange(sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 1).Take(unit.MovePhases + unit.MoveBeginPhases).Select(FlipH));

            baseSkip +=(unit.MovePhases + unit.MoveBeginPhases) * 5;

            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 0).Take(unit.AttackPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 1).Take(unit.AttackPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 2).Take(unit.AttackPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 3).Take(unit.AttackPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 4).Take(unit.AttackPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 3).Take(unit.AttackPhases).Select(FlipH));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 2).Take(unit.AttackPhases).Select(FlipH));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.AttackPhases * 1).Take(unit.AttackPhases).Select(FlipH));

            baseSkip += unit.AttackPhases * 5;

            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 0).Take(unit.DyingPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 1).Take(unit.DyingPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 2).Take(unit.DyingPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 3).Take(unit.DyingPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 4).Take(unit.DyingPhases));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 3).Take(unit.DyingPhases).Select(FlipH));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 2).Take(unit.DyingPhases).Select(FlipH));
            newSprites.AddRange(sprites.Skip(baseSkip + unit.DyingPhases * 1).Take(unit.DyingPhases).Select(FlipH));

            baseSkip += unit.DyingPhases * 5;

            newSprites.AddRange(sprites.Skip(baseSkip));

            toConvert.Sprites.Clear();
            toConvert.Sprites.AddRange(newSprites);
            yield break;
        }

        public Image<Rgba32> FlipH(Image<Rgba32> orig)
        {
            var copy = orig.Clone();
            copy.Mutate(x => x.Flip(FlipMode.Horizontal));
            return copy;
        }
    }
}