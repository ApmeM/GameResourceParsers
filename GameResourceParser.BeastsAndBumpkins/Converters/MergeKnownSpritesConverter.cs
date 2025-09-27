using System.ComponentModel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class MergeKnownSpritesConverter : BaseFileConverter<SpriteFile>
{
    private class SpriteDescription
    {
        public SpriteDescription(string name, string stay, string move, string attack, string dying, string dead)
        {
            Name = name;
            Stay = stay;
            Move = move;
            Attack = attack;
            Dying = dying;
            Dead = dead;
            Other = new List<string>();
        }

        public SpriteDescription(string name, string stay, string move, string attack, string dying, string dead, string other)
            : this(name, stay, move, attack, dying, dead)
        {
            Other = other.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public string Name;
        public string Stay;
        public string Move;
        public string Attack;
        public string Dying;
        public string Dead;
        public List<string> Other;

        public List<string> Actions => new List<string> { Stay, Move, Attack, Dying, Dead }.ToList();
        public List<string> All => new[] { Actions, Other }
            .SelectMany(a => a)
            .ToList();
    }

    private Dictionary<string, SpriteDescription> knownSprites = new Dictionary<string, SpriteDescription>
    {
        {"wasp", new SpriteDescription("wasp", "wasp", "wasp", "waspat", "waspdead", "") },
        {"wolf", new SpriteDescription("wolf", "wolfstil", "wolf", "wolfatta", "wolfdead", "") },
        {"ft_still", new SpriteDescription("footman", "ft_still", "footwalk", "footatta", "d_foot", "footdie") },
        {"gianstil", new SpriteDescription("giant", "gianstil", "giant", "gianatt", "giandead", "") },
        {"dw_still", new SpriteDescription("darkWolf", "dw_still", "dw_walk", "dw_attak", "darkdead", "") },
        {"bloodwsp", new SpriteDescription("bloodWasp", "bloodwsp", "bloodwsp", "bldsting", "bloodead", "") },
        {"bat", new SpriteDescription("bat", "bat", "bat", "batatta", "deadbat", "") },
        {"cavalier", new SpriteDescription("cavalier", "", "cavalier", "cavatta", "d_cava", "") },
        {"ogrestil", new SpriteDescription("ogre", "ogrestil", "ogrewalk", "ogreatta", "ogredead", "") },
        {"knigwalk", new SpriteDescription("knight", "kn_still", "knigwalk", "knigatta", "knigdie", "d_knig") },
        {"flamewlk", new SpriteDescription("fireman", "", "flamewlk", "firing", "", "", "fl_east,fl_noea,fl_nort,fl_nowe,fl_soea,fl_sout,fl_sowe,fl_west") },
        {"arc_stil", new SpriteDescription("archer", "arc_stil", "archer", "archshoo", "d_arch", "") },
        {"zombie", new SpriteDescription("zombie", "", "zombie", "zombatta", "zombdead", "") },
        {"mk_stil", new SpriteDescription("monk", "mk_stil", "monkwalk", "", "", "") },
        {"pk_still", new SpriteDescription("pikeman", "pk_still", "pikewalk", "pikeatta", "pikedie", "d_pike") },
        {"flagwalk", new SpriteDescription("flag", "fg_still", "flagwalk", "", "", "flagdie") },
        {"pr_still", new SpriteDescription("priest", "pr_still", "priest", "pri_heal", "d_prie", "", "fleeprie") },
        {"tx_still", new SpriteDescription("taxman", "tx_still", "taxman", "", "d_taxm", "") },
        {"wz_still", new SpriteDescription("wizard", "wz_still", "wizwalk", "wizcast", "d_wiza", "wizadie", "wizaim") },
        {"minstrel", new SpriteDescription("minstrel", "minjuggl", "minstrel", "", "d_jest", "", "minlyre,minbow") },
        {"vampstil", new SpriteDescription("vampire", "vampstil", "vampbat", "", "", "vampdead") },
        {"girlstil", new SpriteDescription("girl", "girlstil", "kidf", "", "", "d_kidf") },
        {"boystill", new SpriteDescription("boy", "boystill", "kidm", "", "", "d_kidm") },
        {"elderf", new SpriteDescription("oldWoman", "", "elderf", "", "", "d_oldw") },
        {"elderm", new SpriteDescription("oldMan", "", "elderm", "", "", "d_oldm") },
        {"rp_still", new SpriteDescription("builder", "rp_still", "reparman", "", "", "d_buil", "pick,saw") },
    };
    HashSet<string> allSprites;

    public MergeKnownSpritesConverter()
    {
        allSprites = [.. knownSprites.Values.SelectMany(a => a.All)];
    }

    protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
    {
        if (!knownSprites.ContainsKey(toConvert.relativeFileName.ToLower()))
        {
            if (!allSprites.Contains(toConvert.relativeFileName.ToLower()))
            {
                yield return toConvert;
            }
            yield break;
        }

        var data = knownSprites[toConvert.relativeFileName.ToLower()];

        var stay = ((SpriteFile)files.SingleOrDefault(a => a.relativeFileName == data.Stay))?.Sprites ?? new List<Image<Rgba32>>();
        var move = ((SpriteFile)files.SingleOrDefault(a => a.relativeFileName == data.Move))?.Sprites ?? new List<Image<Rgba32>>();
        var attack = ((SpriteFile)files.SingleOrDefault(a => a.relativeFileName == data.Attack))?.Sprites ?? new List<Image<Rgba32>>();
        var dead = ((SpriteFile)files.SingleOrDefault(a => a.relativeFileName == data.Dead))?.Sprites ?? new List<Image<Rgba32>>();
        var dying = ((SpriteFile)files.SingleOrDefault(a => a.relativeFileName == data.Dying))?.Sprites ?? new List<Image<Rgba32>>();

        var spriteFiles = data
            .Actions
            .Distinct()
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => files.Single(b => b.relativeFileName == a))
            .OfType<SpriteFile>()
            .ToList();

        spriteFiles.ForEach(a => files.Remove(a));
        var sprites = spriteFiles.SelectMany(a => a.Sprites).ToList();

        yield return new SpriteFile
        {
            Sprites = sprites,
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = data.Name,
            relativeFileExtension = toConvert.relativeFileExtension
        };

        var spriteLevel = new SpriteDescriptionFile
        {
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = data.Name,
            relativeFileExtension = toConvert.relativeFileExtension
        };

        spriteLevel.FrameWidth = sprites.Max(a => a.Width);
        spriteLevel.FrameHeight = sprites.Max(a => a.Height);

        spriteLevel.CountWidth = ((int)Math.Sqrt(sprites.Count)) + 1;
        spriteLevel.CountHeight = ((int)Math.Sqrt(sprites.Count)) + 1;

        spriteLevel.StayLength = 0.1f;
        spriteLevel.StayBottomLeft = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(0 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(0 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayLeft = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(1 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(1 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayTopLeft = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(2 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(2 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayTop = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(3 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(3 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayTopRight = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(4 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(4 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayRight = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(5 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(5 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayBottomRight = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(6 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(6 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.StayBottom = string.IsNullOrWhiteSpace(data.Stay) ? move.Skip(7 * move.Count / 8).Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList() : stay.Skip(7 * stay.Count / 8).Take(stay.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        spriteLevel.MoveLength = 0.5f;
        spriteLevel.MoveBottomLeft = move.Skip(0 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveLeft = move.Skip(1 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveTopLeft = move.Skip(2 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveTop = move.Skip(3 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveTopRight = move.Skip(4 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveRight = move.Skip(5 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveBottomRight = move.Skip(6 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.MoveBottom = move.Skip(7 * move.Count / 8).Take(move.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        spriteLevel.AttackLength = 0.5f;
        spriteLevel.AttackBottomLeft = attack.Skip(0 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackLeft = attack.Skip(1 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackTopLeft = attack.Skip(2 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackTop = attack.Skip(3 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackTopRight = attack.Skip(4 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackRight = attack.Skip(5 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackBottomRight = attack.Skip(6 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.AttackBottom = attack.Skip(7 * attack.Count / 8).Take(attack.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        spriteLevel.DieLength = 1f;
        spriteLevel.DieBottomLeft = dying.Skip(0 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieLeft = dying.Skip(1 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieTopLeft = dying.Skip(2 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieTop = dying.Skip(3 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieTopRight = dying.Skip(4 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieRight = dying.Skip(5 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieBottomRight = dying.Skip(6 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();
        spriteLevel.DieBottom = dying.Skip(7 * dying.Count / 8).Take(dying.Count / 8).Select(a => ToDescription(a, sprites.IndexOf(a))).ToList();

        spriteLevel.DieBottomLeft.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieLeft.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieTopLeft.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieTop.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieTopRight.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieRight.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieBottomRight.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));
        spriteLevel.DieBottom.AddRange(dead.Take(1).Select(a => ToDescription(a, sprites.IndexOf(a))));

        yield return spriteLevel;

        foreach (var action in data.Other)
        {
            var actionSprites = files
                .OfType<SpriteFile>()
                .Single(b => b.relativeFileName == action);
            files.Remove(actionSprites);

            yield return new SpriteFile
            {
                Sprites = actionSprites.Sprites,
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = data.Name + "_" + action,
                relativeFileExtension = toConvert.relativeFileExtension
            };
        }
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