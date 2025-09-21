using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Final converter of allods files to common files that are available for save to other formats like godot, tmx, etc.
/// </summary>
public class UnitSpriteDescribeConverter : BaseFileConverter<SpriteFile>
{
    protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
    {
        yield return toConvert;
        var units = files.OfType<RegUnitsFile>().First();

        var unit = units.Units.FirstOrDefault(a => toConvert.relativeFilePath.Replace("heroes", "humans").Contains(a.File, StringComparison.InvariantCultureIgnoreCase));

        if (unit == null)
        {
            yield break;
        }

        var sprites = toConvert.Sprites;

        var spriteLevel = new SpriteDescriptionFile
        {
            relativeFileExtension = ".txt",
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = toConvert.relativeFileName
        };
        var baseSkip = 0;

        spriteLevel.FrameWidth = toConvert.Sprites.Max(a => a.Width);
        spriteLevel.FrameHeight = toConvert.Sprites.Max(a => a.Height);

        spriteLevel.CountWidth = ((int)Math.Sqrt(toConvert.Sprites.Count)) + 1;
        spriteLevel.CountHeight = ((int)Math.Sqrt(toConvert.Sprites.Count)) + 1;

        spriteLevel.StayLength = 0.1f;
        spriteLevel.StayBottom = sprites.Skip(2 * 0).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayBottomLeft = sprites.Skip(2 * 1).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayLeft = sprites.Skip(2 * 2).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayTopLeft = sprites.Skip(2 * 3).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayTop = sprites.Skip(2 * 4).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayTopRight = sprites.Skip(2 * 5).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayRight = sprites.Skip(2 * 6).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayBottomRight = sprites.Skip(2 * 7).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        baseSkip += 2 * 8;

        spriteLevel.MoveLength = unit.MoveAnimFrame.Sum() / 10f;
        spriteLevel.MoveBottom = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 0).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveBottomLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 1).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 2).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveTopLeft = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 3).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveTop = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 4).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveTopRight = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 5).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveRight = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 6).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveBottomRight = sprites.Skip(baseSkip + (unit.MovePhases + unit.MoveBeginPhases) * 7).Take(unit.MovePhases + unit.MoveBeginPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        baseSkip += (unit.MovePhases + unit.MoveBeginPhases) * 8;

        spriteLevel.AttackLength = unit.AttackAnimTime.Sum() / 10f;
        spriteLevel.HitDelay = unit.AttackDelay / 10f;
        spriteLevel.AttackBottom = sprites.Skip(baseSkip + unit.AttackPhases * 0).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackBottomLeft = sprites.Skip(baseSkip + unit.AttackPhases * 1).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackLeft = sprites.Skip(baseSkip + unit.AttackPhases * 2).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackTopLeft = sprites.Skip(baseSkip + unit.AttackPhases * 3).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackTop = sprites.Skip(baseSkip + unit.AttackPhases * 4).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackTopRight = sprites.Skip(baseSkip + unit.AttackPhases * 5).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackRight = sprites.Skip(baseSkip + unit.AttackPhases * 6).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackBottomRight = sprites.Skip(baseSkip + unit.AttackPhases * 7).Take(unit.AttackPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        baseSkip += unit.AttackPhases * 8;

        spriteLevel.DieBottom = sprites.Skip(baseSkip + unit.DyingPhases * 0).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieBottomLeft = sprites.Skip(baseSkip + unit.DyingPhases * 1).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieLeft = sprites.Skip(baseSkip + unit.DyingPhases * 2).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieTopLeft = sprites.Skip(baseSkip + unit.DyingPhases * 3).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieTop = sprites.Skip(baseSkip + unit.DyingPhases * 4).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieTopRight = sprites.Skip(baseSkip + unit.DyingPhases * 5).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieRight = sprites.Skip(baseSkip + unit.DyingPhases * 6).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieBottomRight = sprites.Skip(baseSkip + unit.DyingPhases * 7).Take(unit.DyingPhases).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        yield return spriteLevel;
    }

    private SpriteDescriptionFile.Description ToDescription(Image<Rgba32> image, int index)
    {
        return new SpriteDescriptionFile.Description
        {
            Id = index,
            Width = image.Width,
            Height = image.Height
        };
    }
}