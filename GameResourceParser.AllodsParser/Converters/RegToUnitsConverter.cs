using SixLabors.ImageSharp;

public class RegToUnitsConverter : BaseFileConverter<RegFile>
{
    protected override IEnumerable<BaseFile> ConvertFile(RegFile toConvert, List<BaseFile> files)
    {
        yield return toConvert;
        if (toConvert.relativeFilePath != "graphics/units/units.reg")
        {
            yield break;
        }

        var fileList = ((Dictionary<string, object>)toConvert.Root["Files"])
            .ToDictionary(
                a => int.Parse(a.Key.Remove(0, "File".Length)),
                a => ((string)a.Value).Replace("\\", "/"));

        yield return new RegUnitsFile
        {
            Units = toConvert.Root
                .Where(a => a.Key != "Global")
                .Where(a => a.Key != "Files")
                .Select(a =>
                {
                    var value = (Dictionary<string, object>)a.Value;

                    var file = fileList[GetInt(value, "File") ?? -1];

                    var sprite = files
                        .OfType<SpritesWithPalettesFile>()
                        .Where(a => Path.Join(a.relativeFileDirectory, a.relativeFileName).EndsWith(file))
                        .First()
                        .Sprites[0];

                    var unit = BuildUnit(toConvert.Root, fileList, value);
                    unit.Width = sprite.Width;
                    unit.Height = sprite.Height;
                    return unit;
                })
                .ToList(),
            relativeFileExtension = ".json",
            relativeFileDirectory = toConvert.relativeFileDirectory,
            relativeFileName = toConvert.relativeFileName
        };
    }

    private RegUnitsFile.UnitFileContent BuildUnit(Dictionary<string, object> root, Dictionary<int, string> fileList, Dictionary<string, object> value)
    {
        var file = fileList[GetInt(value, "File") ?? -1];

        var id = GetInt(value, "ID");
        var parentId = GetInt(value, "Parent");
        RegUnitsFile.UnitFileContent parent = null;

        if (parentId != null && parentId >= 0 && parentId != id)
        {
            parent = BuildUnit(root, fileList, (Dictionary<string, object>)root.Values.First(a => GetInt((Dictionary<string, object>)a, "ID") == parentId));
        }

        return new RegUnitsFile.UnitFileContent
        {
            Description = GetString(value, "DescText") ?? parent?.Description ?? string.Empty,
            Id = id ?? parent?.Id ?? -1,
            File = file.Replace("heroes", "humans"),
            Index = GetInt(value, "Index") ?? parent?.Id ?? -1,
            MovePhases = GetInt(value, "MovePhases") ?? parent?.MovePhases ?? -1,
            MoveBeginPhases = GetInt(value, "MoveBeginPhases") ?? parent?.MoveBeginPhases ?? -1,
            AttackPhases = GetInt(value, "AttackPhases") ?? parent?.AttackPhases ?? -1,
            DyingPhases = GetInt(value, "DyingPhases") ?? parent?.DyingPhases ?? -1,
            Width = GetInt(value, "Width") ?? parent?.Width ?? -1, //ToDo: check width = -1
            Height = GetInt(value, "Height") ?? parent?.Height ?? -1,
            CenterX = GetInt(value, "CenterX") ?? parent?.CenterX ?? -1,
            CenterY = GetInt(value, "CenterY") ?? parent?.CenterY ?? -1,
            SelectionX1 = GetInt(value, "SelectionX1") ?? parent?.SelectionX1 ?? -1,
            SelectionX2 = GetInt(value, "SelectionX2") ?? parent?.SelectionX2 ?? -1,
            SelectionY1 = GetInt(value, "SelectionY1") ?? parent?.SelectionY1 ?? -1,
            SelectionY2 = GetInt(value, "SelectionY2") ?? parent?.SelectionY2 ?? -1,
            AttackAnimTime = GetIntArray(value, "AttackAnimTime") ?? parent?.AttackAnimTime ?? [],
            AttackAnimFrame = GetIntArray(value, "AttackAnimFrame") ?? parent?.AttackAnimFrame ?? [],
            MoveAnimTime = GetIntArray(value, "MoveAnimTime") ?? parent?.MoveAnimTime ?? [],
            MoveAnimFrame = GetIntArray(value, "MoveAnimFrame") ?? parent?.MoveAnimFrame ?? [],
            Dying = GetInt(value, "Dying") ?? parent?.Dying ?? -1,
            BonePhases = GetInt(value, "BonePhases") ?? parent?.BonePhases ?? -1,
            Palette = GetInt(value, "Palette") ?? parent?.Palette ?? -1,
            Sound = GetIntArray(value, "Sound") ?? parent?.Sound ?? [],
            Projectile = GetInt(value, "Projectile") ?? parent?.Projectile ?? -1,
            AttackDelay = GetInt(value, "AttackDelay") ?? parent?.AttackDelay ?? -1,
            ShootDelay = GetInt(value, "ShootDelay") ?? parent?.ShootDelay ?? -1,
            ShootOffset = GetIntArray(value, "ShootOffset") ?? parent?.ShootOffset ?? [],
            InfoPicture = GetString(value, "InfoPicture") ?? parent?.InfoPicture ?? string.Empty,
            InMapEditor = GetBoolean(value, "InMapEditor") ?? parent?.InMapEditor ?? false,
            Flip = GetBoolean(value, "Flip") ?? parent?.Flip ?? false,
        };
    }

    public string GetString(Dictionary<string, object> value, string key)
    {
        if (!value.ContainsKey(key))
        {
            return null;
        }
        return (string)value[key];
    }
    public int? GetInt(Dictionary<string, object> value, string key)
    {
        if (!value.ContainsKey(key))
        {
            return null;
        }
        return (int)value[key];
    }
    public bool? GetBoolean(Dictionary<string, object> value, string key)
    {
        if (!value.ContainsKey(key))
        {
            return null;
        }
        return (int)value[key] == 1;
    }
    public int[] GetIntArray(Dictionary<string, object> value, string key)
    {
        if (!value.ContainsKey(key))
        {
            return null;
        }

        if (value[key].GetType() != typeof(int[]))
        {
            return null;
        }
        return (int[])value[key];
    }
}