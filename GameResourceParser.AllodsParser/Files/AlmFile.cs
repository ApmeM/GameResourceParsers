using System.Numerics;
using System.Text;
using AllodsParser;

public class AlmFile : BaseFile
{
    public class AlmData
    {
        public uint Width;
        public uint Height;
        public float SolarAngle;
        public uint TimeOfDay;
        public uint Darkness;
        public uint Contrast;
        public uint UseTiles;

        public uint CountPlayers;
        public uint CountStructures;
        public uint CountUnits;
        public uint CountTriggers;
        public uint CountSacks;
        public uint CountGroups;
        public uint CountInns;
        public uint CountShops;
        public uint CountPointers;
        public uint CountMusic;
        public string Name;
        public uint RecPlayers;
        public uint Level;
        public uint Junk1;
        public uint Junk2;
        public string Author;

        public void LoadFromStream(BinaryReader br)
        {
            Width = br.ReadUInt32();
            Height = br.ReadUInt32();
            SolarAngle = br.ReadSingle();
            TimeOfDay = br.ReadUInt32();
            Darkness = br.ReadUInt32();
            Contrast = br.ReadUInt32();
            UseTiles = br.ReadUInt32();
            CountPlayers = br.ReadUInt32();
            CountStructures = br.ReadUInt32();
            CountUnits = br.ReadUInt32();
            CountTriggers = br.ReadUInt32();
            CountSacks = br.ReadUInt32();
            CountGroups = br.ReadUInt32();
            CountInns = br.ReadUInt32();
            CountShops = br.ReadUInt32();
            CountPointers = br.ReadUInt32();
            CountMusic = br.ReadUInt32();
            Name = UnpackByteString(1251, br.ReadBytes(0x40));
            RecPlayers = br.ReadUInt32();
            Level = br.ReadUInt32();
            Junk1 = br.ReadUInt32();
            Junk2 = br.ReadUInt32();
            //Console.WriteLine("data junk1 = {0}, junk2 = {1}, light = {2}", Junk1, Junk2, Darkness);
            Author = UnpackByteString(1251, br.ReadBytes(0x200));
        }
    }

    public class AlmEffect
    {
        public bool IsItem;

        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();
            X = br.ReadUInt32();
            Y = br.ReadUInt32();
            IsItem = X == 0 && Y == 0;
            MagicOrFlag = br.ReadUInt16();
            if (!IsItem)
            {
                StructureID = br.ReadUInt32();
            }
            else
            {
                MagicMinDmg = br.ReadUInt16();
                AvgDmg = br.ReadUInt16();
            }
            TypeID = br.ReadUInt16();
            MagicPower = br.ReadUInt16();
            CountOfModifier = br.ReadUInt32();
            EffectModifiers = new AlmEffectModifier[CountOfModifier];
            for (int i = 0; i < CountOfModifier; i++)
            {
                EffectModifiers[i] = new AlmEffectModifier();
                EffectModifiers[i].LoadFromStream(br);
            }
        }

        public uint CountOfModifier;
        public ushort MagicPower;
        public ushort TypeID;
        public ushort AvgDmg;
        public ushort MagicMinDmg;
        public uint StructureID;
        public uint MagicOrFlag;
        public uint Y;
        public uint X;
        public uint ID;
        public AlmEffectModifier[] EffectModifiers;
    }

    public class AlmEffectModifier
    {
        public void LoadFromStream(BinaryReader br)
        {
            TypeOfMod = br.ReadUInt16();
            Value = br.ReadUInt32();
        }

        public uint Value;
        public ushort TypeOfMod;
    }

    public class AlmGroup
    {
        [Flags]
        public enum AlmGroupFlags
        {
            AiInstantEnabled = 1, RandomPositions = 2,
            QuestKill = 4, QuestIntercept = 8
        }

        public void LoadFromStream(BinaryReader br)
        {
            GroupID = br.ReadUInt32();
            RepopTime = br.ReadUInt32();
            GroupFlag = (AlmGroupFlags)br.ReadUInt32();
            InstanceID = br.ReadUInt32();
        }

        public uint InstanceID;
        public AlmGroupFlags GroupFlag;
        public uint RepopTime;
        public uint GroupID;
    }

    public class AlmInnInfo
    {
        public enum QuestType
        {
            Delivery = 0x02,
            RaiseDead = 0x04,
            KillAllHumans = 0x10,
            KillAllMonsters = 0x20,
            KillAllUndeadNecro = 0x40
        }

        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();
            Flag = (QuestType)br.ReadUInt32();
            DeliveryID = br.ReadUInt32();
        }

        public uint DeliveryID;
        public QuestType Flag;
        public uint ID;
    }


    public class AlmLogic
    {
        public AlmLogicItem[] LogicItems;
        public AlmCheckItem[] CheckItems;
        public AlmTrigger[] TriggerItems;
        public enum InstanceType
        {
            Num = 1, Group = 2,
            Player = 3, Unit = 4,
            X = 5, Y = 6,
            Item = 8, Building = 9
        }

        public void LoadFromStream(BinaryReader br)
        {
            NumberOfItems = br.ReadUInt32();
            LogicItems = new AlmLogicItem[NumberOfItems];
            for (int i = 0; i < NumberOfItems; i++)
            {
                LogicItems[i] = new AlmLogicItem();
                LogicItems[i].LoadFromStream(br);
            }
            NumberOfChecks = br.ReadUInt32();
            CheckItems = new AlmCheckItem[NumberOfChecks];
            for (int i = 0; i < NumberOfChecks; i++)
            {
                CheckItems[i] = new AlmCheckItem();
                CheckItems[i].LoadFromStream(br);
            }
            NumberOfTriggers = br.ReadUInt32();
            TriggerItems = new AlmTrigger[NumberOfTriggers];
            for (int i = 0; i < NumberOfTriggers; i++)
            {
                TriggerItems[i] = new AlmTrigger();
                TriggerItems[i].LoadFromStream(br);
            }
        }

        public class AlmLogicItem
        {
            public uint ExecOnceFlag;
            public uint TypeID;
            public uint TypeIndex;
            public string Name;
            public LogicVal[] Values;
            public struct LogicVal
            {
                public string Name;
                public uint Value;
                public InstanceType InstType;
            }

            public void LoadFromStream(BinaryReader br)
            {
                Name = UnpackByteString(1251, br.ReadBytes(0x40));
                TypeID = br.ReadUInt32();
                TypeIndex = br.ReadUInt32();
                ExecOnceFlag = br.ReadUInt32();

                Values = new LogicVal[10];
                for (int i = 0; i < 10; i++)
                {
                    Values[i].Value = br.ReadUInt32();
                }
                for (int i = 0; i < 10; i++)
                {
                    Values[i].InstType = (InstanceType)br.ReadUInt32();
                }
                for (int i = 0; i < 10; i++)
                {
                    Values[i].Name = UnpackByteString(1251, br.ReadBytes(0x40));
                }
            }
        }

        public class AlmCheckItem : AlmLogicItem { }

        public class AlmTrigger
        {
            public enum TriggerOperator
            {
                Equal = 0, NotEqual = 1,
                MoreThan = 2, LowerThan = 3,
                MoreOrEq = 4, LowerOrEq = 5
            }

            public void LoadFromStream(BinaryReader br)
            {
                Name = UnpackByteString(1251, br.ReadBytes(0x80));

                for (int i = 0; i < 6; i++)
                    Checks[i] = br.ReadUInt32();

                for (int i = 0; i < 4; i++)
                    Instances[i] = br.ReadUInt32();

                for (int i = 0; i < 3; i++)
                    Operators[i] = (TriggerOperator)br.ReadUInt32();

                RunOnce = br.ReadUInt32();
            }

            public string Name;

            public uint RunOnce;
            public uint[] Checks = new uint[6];
            public uint[] Instances = new uint[4];
            public TriggerOperator[] Operators = new TriggerOperator[3]; // between every 2 Checks
        }

        public uint NumberOfTriggers;
        public uint NumberOfChecks;
        public uint NumberOfItems;


        public Vector2 GetDropLocation()
        {
            if (LogicItems != null && LogicItems.Length > 0)
            {
                var dropLoc = LogicItems.Where(x => x.Name.ToLower() == "drop location")
                                        .OrderBy(x => Guid.NewGuid())
                                        .FirstOrDefault();
                if (dropLoc != null)
                {
                    return new Vector2(dropLoc.Values.Where(x => x.InstType == InstanceType.X).Select(x => x.Value).FirstOrDefault(),
                                       dropLoc.Values.Where(x => x.InstType == InstanceType.Y).Select(x => x.Value).FirstOrDefault());
                }
            }
            return new Vector2(6, 6);
        }
    }

    public class AlmMusic
    {
        public void LoadFromStream(BinaryReader br)
        {
            X = br.ReadUInt32();
            Y = br.ReadUInt32();
            Radius = br.ReadUInt32();
            for (int i = 0; i < TypeIDs.Length; i++)
                TypeIDs[i] = br.ReadUInt32();
        }

        public uint[] TypeIDs = new uint[4];
        public uint Radius;
        public uint Y;
        public uint X;
    }
    public class AlmOptionPointer
    {
        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();
            InstanceOn = br.ReadUInt32();
            InstanceID = br.ReadUInt32();
        }

        public uint InstanceID;
        public uint InstanceOn;
        public uint ID;
    }

    public class AlmPlayer
    {
        public int Color;
        public uint Flags;//1=AI,2=quest kill
        public int Money;
        public string Name;
        public ushort[] Diplomacy;

        public void LoadFromStream(BinaryReader br)
        {
            Color = br.ReadInt32();
            Flags = br.ReadUInt32();
            Money = br.ReadInt32();
            Name = UnpackByteString(1251, br.ReadBytes(0x20));
            Diplomacy = new ushort[16];
            for (int i = 0; i < 16; i++)
                Diplomacy[i] = br.ReadUInt16();
        }
    }
    public class AlmSack
    {
        public void LoadFromStream(BinaryReader br)
        {
            NumberOfItems = br.ReadUInt32();
            UnitID = br.ReadUInt32();
            X = br.ReadUInt32();
            Y = br.ReadUInt32();
            Gold = br.ReadUInt32();
            Items = new AlmSackItem[NumberOfItems];
            for (int i = 0; i < NumberOfItems; i++)
            {
                Items[i] = new AlmSackItem();
                Items[i].LoadFromStream(br);
            }
        }

        public uint Gold;
        public uint Y;
        public uint X;
        public uint UnitID;
        public uint NumberOfItems;
        public AlmSackItem[] Items;
    }
    public class AlmSackItem
    {
        public void LoadFromStream(BinaryReader br)
        {
            ItemID = br.ReadUInt32();
            Wielded = br.ReadUInt16();
            EffectNumber = br.ReadUInt32();
        }

        public uint EffectNumber;
        public ushort Wielded;
        public uint ItemID;
    }

    public class AlmShop
    {
        [Flags]
        public enum AlmShopItemMaterial
        {
            Iron = 0x00000001,
            Bronze = 0x00000002,
            Steel = 0x00000004,
            Silver = 0x00000008,
            Gold = 0x00000010,
            Mithrill = 0x00000020,
            Adamantium = 0x00000040,
            Meteoric = 0x00000080,
            Wood = 0x00000100,
            MagicWood = 0x00000200,
            Leather = 0x00000400,
            HardLeather = 0x00000800,
            DragonLeather = 0x00001000,
            Crystal = 0x00002000,
            None = 0x00004000
        }

        [Flags]
        public enum AlmShopItemType
        {
            Weapon = 0x00400000,
            Shield = 0x00800000,
            Armor = 0x01000000,
            ArmorMage = 0x02000000,
            Other = 0x04000000,
            Wands = 0x08000000,
        }

        [Flags]
        public enum AlmShopItemExtra
        {
            Common = 0x10000000,
            Magic = 0x20000000
        }

        [Flags]
        public enum AlmShopItemClass
        {
            Common = 0x00008000,
            Uncommon = 0x00010000,
            Rare = 0x00020000,
            VeryRare = 0x00040000,
            Elven = 0x00080000,
            Bad = 0x00100000,
            Good = 0x00200000,
        }

        public class AlmShopShelf
        {
            public AlmShopItemMaterial ItemMaterials;
            public AlmShopItemType ItemTypes;
            public AlmShopItemExtra ItemExtras;
            public AlmShopItemClass ItemClasses;
            public uint PriceMin;
            public uint PriceMax;
            public uint MaxItems;
            public uint MaxSameItems;
        }

        private static uint GetFlagsMask(Type t)
        {
            uint mask = 0;
            IEnumerable<Enum> values = Enum.GetValues(t).Cast<Enum>();
            foreach (Enum v in values)
                mask |= (uint)((int)Enum.ToObject(t, v));
            return mask;
        }

        public uint ID;
        public AlmShopShelf[] Shelves = new AlmShopShelf[4];

        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();

            for (int i = 0; i < 4; i++)
                Shelves[i] = new AlmShopShelf();

            for (int i = 0; i < 4; i++)
            {
                uint flags = br.ReadUInt32();
                Shelves[i].ItemMaterials = (AlmShopItemMaterial)(flags & GetFlagsMask(typeof(AlmShopItemMaterial)));
                Shelves[i].ItemTypes = (AlmShopItemType)(flags & GetFlagsMask(typeof(AlmShopItemType)));
                Shelves[i].ItemExtras = (AlmShopItemExtra)(flags & GetFlagsMask(typeof(AlmShopItemExtra)));
                Shelves[i].ItemClasses = (AlmShopItemClass)(flags & GetFlagsMask(typeof(AlmShopItemClass)));
            }

            for (int i = 0; i < 4; i++)
                Shelves[i].PriceMin = br.ReadUInt32();
            for (int i = 0; i < 4; i++)
                Shelves[i].PriceMax = br.ReadUInt32();
            for (int i = 0; i < 4; i++)
                Shelves[i].MaxItems = br.ReadUInt32();
            for (int i = 0; i < 4; i++)
                Shelves[i].MaxSameItems = br.ReadUInt32();
        }
    }
    public class AlmStructure
    {
        public float X;
        public float Y;
        public int TypeID;
        public short Health;
        public int Player;
        public int ID;
        public bool IsBridge;
        public int Width;
        public int Height;

        public void LoadFromStream(BinaryReader br)
        {
            int xRaw = br.ReadInt32();
            int yRaw = br.ReadInt32();
            X = ((xRaw & 0x0000FF00) >> 8) + (float)(xRaw & 0x00FF) / 256; // 00 10 00 80 = 16.5
            Y = ((yRaw & 0x0000FF00) >> 8) + (float)(yRaw & 0x00FF) / 256;
            TypeID = br.ReadInt32();
            Health = br.ReadInt16();
            Player = br.ReadInt32();
            ID = br.ReadInt16();
            if ((TypeID & 0x01000000) != 0)
            {
                Width = br.ReadInt32();
                Height = br.ReadInt32();
                IsBridge = true;
            }
            TypeID &= 0xFFFF;
        }
    }

    public class AlmUnit
    {
        public float X;
        public float Y;
        public ushort TypeID;
        public ushort Face;
        public uint Flags;
        public uint Flags2;
        public int ServerID;
        public int Player;
        public int Sack;
        public int Angle;
        public short Health;
        public short HealthMax;
        public int ID;
        public int Group;

        public void LoadFromStream(BinaryReader br)
        {
            int xRaw = br.ReadInt32();
            int yRaw = br.ReadInt32();
            X = ((xRaw & 0x0000FF00) >> 8) + (float)(xRaw & 0x00FF) / 256; // 00 10 00 80 = 16.5
            Y = ((yRaw & 0x0000FF00) >> 8) + (float)(yRaw & 0x00FF) / 256;
            TypeID = br.ReadUInt16();
            Face = br.ReadUInt16();
            Flags = br.ReadUInt32();
            Flags2 = br.ReadUInt32();
            ServerID = br.ReadInt32();
            Player = br.ReadInt32();
            Sack = br.ReadInt32();
            Angle = br.ReadInt32();
            Health = br.ReadInt16();
            HealthMax = br.ReadInt16();
            ID = br.ReadInt32() & 0xFFFF;
            Group = br.ReadInt32();
        }
    }
    public class TerrainLighting
    {
        public readonly int Width;
        public readonly int Height;
        public readonly byte[] Result = null;

        public TerrainLighting(int w, int h)
        {
            Width = w;
            Height = h;
            Result = new byte[w * h];
        }

        public void Calculate(sbyte[] heights, double solarAngle)
        {
            double sunang = solarAngle * (Math.PI / 180.0);

            double sunx = Math.Cos(sunang) * 1.0;
            double suny = Math.Sin(sunang) * 1.0;
            double sunz = -0.75;

            //
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if ((x <= 1) || (y <= 1) ||
                        (x >= Width - 2) || (y >= Height - 2))
                    {
                        Result[y * Width + x] = 0;
                        continue;
                    }

                    // 
                    double p1x = x * 32.0;
                    double p1y = y * 32.0;
                    double p1z = heights[y * Width + x];

                    double p2x = x * 32.0 + 32.0;
                    double p2y = y * 32.0;
                    double p2z = heights[y * Width + (x + 1)];

                    double p3x = x * 32.0;
                    double p3y = y * 32.0 + 32.0;
                    double p3z = heights[(y + 1) * Width + x];

                    //
                    double ux = p2x - p1x;
                    double uy = p2y - p1y;
                    double uz = p2z - p1z;

                    double vx = p3x - p1x;
                    double vy = p3y - p1y;
                    double vz = p3z - p1z;

                    //
                    double nx = (uy * vz) - (uz * vy);
                    double ny = (uz * vx) - (ux * vz);
                    double nz = (ux * vy) - (uy * vx);

                    //
                    double nl = Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                    nx /= nl;
                    ny /= nl;
                    nz /= nl;

                    //
                    //double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 64.0 + 128.0;
                    double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 64.0 + 96.0;
                    /*double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 62.0;
                    int dot_i = (int)dot;
                    uint dot_ab = (uint)Math.Abs(dot_i-31);
                    // pow dot_ab
                    dot_ab = (uint)(1 << (int)dot_ab);
                    dot_ab = Math.Min(31, dot_ab / 16777216 / 8);
                    if (dot_i < 32)
                        dot = 128.0 - ((double)dot_ab * 2);
                    else dot = 128.0 + ((double)dot_ab * 2);*/

                    //double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 128.0 + 64.0;
                    Result[y * Width + x] = (byte)dot;
                    //Console.WriteLine("dot = {0}", dot);
                }
            }
        }
    }


    public static string UnpackByteString(int encoding, byte[] bytes)
    {
        var codePages = CodePagesEncodingProvider.Instance.GetEncoding(encoding);
        string str_in = codePages.GetString(bytes);
        string str_out = str_in;
        int i = str_out.IndexOf('\0');
        if (i >= 0) str_out = str_out.Substring(0, i);
        return str_out;
    }

    public AlmData Data = new AlmData();
    public ushort[] Tiles;
    public sbyte[] Heights;
    public byte[] Objects;
    public AlmPlayer[] Players;
    public AlmStructure[] Structures;
    public AlmUnit[] Units;
    public AlmLogic Logic;
    public AlmMusic[] Music;
    public AlmGroup[] Groups;
    public AlmSack[] Sacks;
    public AlmEffect[] Effects;
    public AlmShop[] Shops;
    public AlmOptionPointer[] Pointers;
    public AlmInnInfo[] Inns;
}
