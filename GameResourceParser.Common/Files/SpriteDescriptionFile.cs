using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class SpriteDescriptionFile : BaseFile
{
    public List<Image<Rgba32>> AllSprites => StayAll.Concat(MoveAll).Concat(AttackAll).Concat(DieAll).ToList();

    public float StayLength;
    public List<Image<Rgba32>> StayLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayTopLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayBottomLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayTopRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayBottomRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayTop = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayBottom = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> StayAll => StayBottom
        .Concat(StayBottomLeft)
        .Concat(StayLeft)
        .Concat(StayTopLeft)
        .Concat(StayTop)
        .Concat(StayTopRight)
        .Concat(StayRight)
        .Concat(StayBottomRight)
        .ToList();

    public float MoveLength;
    public List<Image<Rgba32>> MoveLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveTopLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveBottomLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveTopRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveBottomRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveTop = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveBottom = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> MoveAll => MoveBottom
        .Concat(MoveBottomLeft)
        .Concat(MoveLeft)
        .Concat(MoveTopLeft)
        .Concat(MoveTop)
        .Concat(MoveTopRight)
        .Concat(MoveRight)
        .Concat(MoveBottomRight)
        .ToList();

    public float HitDelay;
    public float AttackLength;
    public List<Image<Rgba32>> AttackLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackTopLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackBottomLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackTopRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackBottomRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackTop = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackBottom = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> AttackAll => AttackBottom
        .Concat(AttackBottomLeft)
        .Concat(AttackLeft)
        .Concat(AttackTopLeft)
        .Concat(AttackTop)
        .Concat(AttackTopRight)
        .Concat(AttackRight)
        .Concat(AttackBottomRight)
        .ToList();

    public float DieFPS = 5;
    public List<Image<Rgba32>> DieLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieTopLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieBottomLeft = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieTopRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieBottomRight = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieTop = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieBottom = new List<Image<Rgba32>>();
    public List<Image<Rgba32>> DieAll => DieBottom
        .Concat(DieBottomLeft)
        .Concat(DieLeft)
        .Concat(DieTopLeft)
        .Concat(DieTop)
        .Concat(DieTopRight)
        .Concat(DieRight)
        .Concat(DieBottomRight)
        .ToList();
}