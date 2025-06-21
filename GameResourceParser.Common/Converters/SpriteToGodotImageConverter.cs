using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class SpriteToGodotImageConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteOutput != GameParserConfigurator.SpriteOutputFormat.GodotSprite)
            {
                return;
            }

            var oldFiles = files
                .OfType<SpriteFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
        {
            for (var i = 0; i < toConvert.Levels.Count; i++)
            {
                if (toConvert.Levels[i].AllSprites.Count == 0)
                {
                    Console.Error.WriteLine($"Sprite {toConvert.relativeFilePath} does not have sprites converted.");
                    yield break;
                }

                var newWidth = toConvert.Levels[i].AllSprites.Max(a => a.Width);
                var newHeight = toConvert.Levels[i].AllSprites.Max(a => a.Height);

                var newImage = new Image<Rgba32>(toConvert.Levels[i].AllSprites.Count * newWidth, newHeight);

                for (int j = 0; j < toConvert.Levels[i].AllSprites.Count; j++)
                {
                    newImage.Mutate(a => a.DrawImage(toConvert.Levels[i].AllSprites[j], new Point(newWidth * j, 0), 1));
                }

                var filename = toConvert.relativeFileName[0].ToString().ToUpper() + toConvert.relativeFileName.Substring(1) + (i == 0 ? "" : i.ToString());

                yield return new ImageFile
                {
                    Image = newImage,
                    relativeFileExtension = ".png",
                    relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, filename),
                    relativeFileName = filename
                };

                yield return new StringFile
                {
                    relativeFileExtension = ".cs",
                    relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, filename),
                    relativeFileName = filename,
                    Data = @$"using Godot;

[SceneReference(""{filename}.tscn"")]
public partial class {filename}
{{
}}"
                };

                yield return new StringFile
                {
                    relativeFileExtension = ".tscn",
                    relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, filename),
                    relativeFileName = filename,
                    Data = @$"
[gd_scene load_steps=57 format=2]

[ext_resource path=""res://Presentation/units/{filename}/{filename}.png"" type=""Texture"" id=1]
[ext_resource path=""res://Presentation/units/BaseUnit.tscn"" type=""PackedScene"" id=2]
[ext_resource path=""res://Presentation/units/{filename}/{filename}.cs"" type=""Script"" id=3]

{string.Join("\n", toConvert.Levels[i].AllSprites.Select((a,b) =>

@$"[sub_resource type=""AtlasTexture"" id={b + 1}]
flags = 4
atlas = ExtResource( 1 )
region = Rect2( {newWidth * b}, 0, {newWidth}, {newHeight} )
"
))}

[sub_resource type=""SpriteFrames"" id={toConvert.Levels[i].AllSprites.Count + 1}]
animations = [ 
{{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackBottom.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackBottom"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackBottomLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackBottomLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackBottomRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackBottomRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackTop.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackTop"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackTopLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackTopLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].AttackTopRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""AttackTopRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveBottom.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveBottom"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveBottomLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveBottomLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveBottomRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveBottomRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveTop.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveTop"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveTopLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveTopLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].MoveTopRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""MoveTopRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayBottom.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayBottom"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayBottomLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayBottomLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayBottomRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayBottomRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayRight"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayTop.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayTop"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayTopLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayTopLeft"",
""speed"": 5.0
}}, {{""frames"": [ { string.Join(", ", toConvert.Levels[i].StayTopRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f)})")) } ],
""loop"": true,
""name"": ""StayTopRight"",
""speed"": 5.0
}} ]

[sub_resource type=""AnimationNodeStateMachinePlayback"" id={toConvert.Levels[i].AllSprites.Count + 2}]

[node name=""{filename}"" instance=ExtResource( 2 )]
script = ExtResource( 3 )
MaxHP = 30
HP = 30
AttackPower = 5
AttackDistance = 100.0
AttackDelay = 1.0
MoveSpeed = 100.0
MoveFloors = PoolIntArray( 2 )
Loot = [ ExtResource( 6 ) ]

[node name=""Healthbar"" parent=""."" index=""0""]
position = Vector2( 28, -41 )

[node name=""AnimatedSprite"" parent=""."" index=""1""]
position = Vector2( 0, -49 )
frames = SubResource( 97 )
animation = ""StayBottom""

[node name=""AnimationTree"" parent=""."" index=""4""]
parameters/playback = SubResource( 146 )

[editable path=""QuestPopup""]
[editable path=""SignPopup""]                    
                    "
                };
            }
        }
    }
}