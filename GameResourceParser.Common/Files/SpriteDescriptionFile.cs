public class SpriteDescriptionFile : BaseFile
{
    public struct Description
    {
        public int Id;
        public int Width;
        public int Height;
    }

    public int FrameWidth;
    public int FrameHeight;
    public int CountWidth;
    public int CountHeight;

    public List<Description> AllSprites => StayAll.Concat(MoveAll).Concat(AttackAll).Concat(DieAll).ToList();

    public float StayLength;
    public List<Description> StayLeft = new List<Description>();
    public List<Description> StayTopLeft = new List<Description>();
    public List<Description> StayBottomLeft = new List<Description>();
    public List<Description> StayRight = new List<Description>();
    public List<Description> StayTopRight = new List<Description>();
    public List<Description> StayBottomRight = new List<Description>();
    public List<Description> StayTop = new List<Description>();
    public List<Description> StayBottom = new List<Description>();
    public List<Description> StayAll => StayBottom
        .Concat(StayBottomLeft)
        .Concat(StayLeft)
        .Concat(StayTopLeft)
        .Concat(StayTop)
        .Concat(StayTopRight)
        .Concat(StayRight)
        .Concat(StayBottomRight)
        .ToList();

    public float MoveLength;
    public List<Description> MoveLeft = new List<Description>();
    public List<Description> MoveTopLeft = new List<Description>();
    public List<Description> MoveBottomLeft = new List<Description>();
    public List<Description> MoveRight = new List<Description>();
    public List<Description> MoveTopRight = new List<Description>();
    public List<Description> MoveBottomRight = new List<Description>();
    public List<Description> MoveTop = new List<Description>();
    public List<Description> MoveBottom = new List<Description>();
    public List<Description> MoveAll => MoveBottom
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
    public List<Description> AttackLeft = new List<Description>();
    public List<Description> AttackTopLeft = new List<Description>();
    public List<Description> AttackBottomLeft = new List<Description>();
    public List<Description> AttackRight = new List<Description>();
    public List<Description> AttackTopRight = new List<Description>();
    public List<Description> AttackBottomRight = new List<Description>();
    public List<Description> AttackTop = new List<Description>();
    public List<Description> AttackBottom = new List<Description>();
    public List<Description> AttackAll => AttackBottom
        .Concat(AttackBottomLeft)
        .Concat(AttackLeft)
        .Concat(AttackTopLeft)
        .Concat(AttackTop)
        .Concat(AttackTopRight)
        .Concat(AttackRight)
        .Concat(AttackBottomRight)
        .ToList();

    public float DieLength = 5;
    public List<Description> DieLeft = new List<Description>();
    public List<Description> DieTopLeft = new List<Description>();
    public List<Description> DieBottomLeft = new List<Description>();
    public List<Description> DieRight = new List<Description>();
    public List<Description> DieTopRight = new List<Description>();
    public List<Description> DieBottomRight = new List<Description>();
    public List<Description> DieTop = new List<Description>();
    public List<Description> DieBottom = new List<Description>();

    public List<Description> DieAll => DieBottom
        .Concat(DieBottomLeft)
        .Concat(DieLeft)
        .Concat(DieTopLeft)
        .Concat(DieTop)
        .Concat(DieTopRight)
        .Concat(DieRight)
        .Concat(DieBottomRight)
        .ToList();
}