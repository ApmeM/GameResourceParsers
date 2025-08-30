using System.Net.WebSockets;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    /// <summary>
    /// Final converter of allods files to common files that are available for save to other formats like godot, tmx, etc.
    /// </summary>
    public class SpriteDescribeConverter : BaseFileConverter<SpriteFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
        {
            yield return toConvert;
            var units = files.OfType<RegUnitsFile>().First();

            var unit = units.Units.FirstOrDefault(a => toConvert.relativeFilePath.Replace("heroes", "humans").Contains(a.File, StringComparison.InvariantCultureIgnoreCase));

            var path = toConvert.relativeFileDirectory.Split("/");

            var description = BuildLevel(unit, toConvert.Sprites);

            description.relativeFileExtension = ".png";
            description.relativeFileDirectory = unit == null ? toConvert.relativeFileDirectory : Path.Combine(toConvert.relativeFileDirectory, "..");
            description.relativeFileName = unit == null ? toConvert.relativeFileName : path[path.Length - 1];

            yield return description;
        }

        private SpriteDescriptionFile BuildLevel(RegUnitsFile.UnitFileContent unit, List<Image<Rgba32>> sprites)
        {
            if (unit == null)
            {
                return new SpriteDescriptionFile
                {
                    StayBottom = sprites
                };
            }

            var spriteLevel = new SpriteDescriptionFile();
            var baseSkip = 0;

            spriteLevel.StayLength = 0.1f;
            spriteLevel.StayBottom = sprites.Skip(2 * 0).Take(1).ToList();
            spriteLevel.StayBottomLeft = sprites.Skip(2 * 1).Take(1).ToList();
            spriteLevel.StayLeft = sprites.Skip(2 * 2).Take(1).ToList();
            spriteLevel.StayTopLeft = sprites.Skip(2 * 3).Take(1).ToList();
            spriteLevel.StayTop = sprites.Skip(2 * 4).Take(1).ToList();
            spriteLevel.StayTopRight = sprites.Skip(2 * 5).Take(1).ToList();
            spriteLevel.StayRight = sprites.Skip(2 * 6).Take(1).ToList();
            spriteLevel.StayBottomRight = sprites.Skip(2 * 7).Take(1).ToList();

            baseSkip += 2 * 8;

            spriteLevel.MoveLength = unit.MoveAnimFrame.Sum() / 10f;
            spriteLevel.MoveBottom = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 0).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveBottomLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 1).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 2).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveTopLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 3).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveTop = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 4).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveTopRight = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 5).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveRight = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 6).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();
            spriteLevel.MoveBottomRight = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 7).Take(unit.MovePhases + unit.MoveBeginPhases).ToList();

            baseSkip += (unit.MovePhases + unit.MoveBeginPhases) * 8;

            spriteLevel.AttackLength = unit.AttackAnimTime.Sum() / 10f;
            spriteLevel.HitDelay = unit.AttackDelay / 10f;
            spriteLevel.AttackBottom = sprites.Skip(baseSkip + unit.AttackPhases * 0).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackBottomLeft = sprites.Skip(baseSkip + unit.AttackPhases * 1).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackLeft = sprites.Skip(baseSkip + unit.AttackPhases * 2).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackTopLeft = sprites.Skip(baseSkip + unit.AttackPhases * 3).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackTop = sprites.Skip(baseSkip + unit.AttackPhases * 4).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackTopRight = sprites.Skip(baseSkip + unit.AttackPhases * 5).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackRight = sprites.Skip(baseSkip + unit.AttackPhases * 6).Take(unit.AttackPhases).ToList();
            spriteLevel.AttackBottomRight = sprites.Skip(baseSkip + unit.AttackPhases * 7).Take(unit.AttackPhases).ToList();

            baseSkip += unit.AttackPhases * 8;

            spriteLevel.DieBottom = sprites.Skip(baseSkip + unit.DyingPhases * 0).Take(unit.DyingPhases).ToList();
            spriteLevel.DieBottomLeft = sprites.Skip(baseSkip + unit.DyingPhases * 1).Take(unit.DyingPhases).ToList();
            spriteLevel.DieLeft = sprites.Skip(baseSkip + unit.DyingPhases * 2).Take(unit.DyingPhases).ToList();
            spriteLevel.DieTopLeft = sprites.Skip(baseSkip + unit.DyingPhases * 3).Take(unit.DyingPhases).ToList();
            spriteLevel.DieTop = sprites.Skip(baseSkip + unit.DyingPhases * 4).Take(unit.DyingPhases).ToList();
            spriteLevel.DieTopRight = sprites.Skip(baseSkip + unit.DyingPhases * 5).Take(unit.DyingPhases).ToList();
            spriteLevel.DieRight = sprites.Skip(baseSkip + unit.DyingPhases * 6).Take(unit.DyingPhases).ToList();
            spriteLevel.DieBottomRight = sprites.Skip(baseSkip + unit.DyingPhases * 7).Take(unit.DyingPhases).ToList();

            return spriteLevel;
        }
    }
}