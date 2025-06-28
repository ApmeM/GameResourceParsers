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

                var countWidth = ((int)Math.Sqrt(toConvert.Levels[i].AllSprites.Count)) + 1;
                var countHeight = ((int)Math.Sqrt(toConvert.Levels[i].AllSprites.Count)) + 1;

                var newImage = new Image<Rgba32>(countWidth * newWidth, countHeight * newHeight);

                for (int j = 0; j < toConvert.Levels[i].AllSprites.Count; j++)
                {
                    newImage.Mutate(a => a.DrawImage(toConvert.Levels[i].AllSprites[j], new Point(newWidth * (j % countWidth), newHeight * (j / countWidth)), 1));
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

{string.Join("\n", toConvert.Levels[i].AllSprites.Select((a, b) =>

@$"[sub_resource type=""AtlasTexture"" id={b + 1}]
flags = 4
atlas = ExtResource( 1 )
region = Rect2( {newWidth * (b % countWidth)}, {newHeight * (b / countWidth)}, {a.Width}, {a.Height} )
"
))}

[sub_resource type=""SpriteFrames"" id={toConvert.Levels[i].AllSprites.Count + 1}]
animations = [ 
{{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackBottom.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackBottom"",
""speed"": {toConvert.Levels[i].AttackBottom.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackBottomLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackBottomLeft"",
""speed"": {toConvert.Levels[i].AttackBottomLeft.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackBottomRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackBottomRight"",
""speed"": {toConvert.Levels[i].AttackBottomRight.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackLeft"",
""speed"": {toConvert.Levels[i].AttackLeft.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackRight"",
""speed"": {toConvert.Levels[i].AttackRight.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackTop.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackTop"",
""speed"": {toConvert.Levels[i].AttackTop.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackTopLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackTopLeft"",
""speed"": {toConvert.Levels[i].AttackTopLeft.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].AttackTopRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackTopRight"",
""speed"": {toConvert.Levels[i].AttackTopRight.Count() / toConvert.Levels[i].AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveBottom.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveBottom"",
""speed"": {toConvert.Levels[i].MoveBottom.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveBottomLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveBottomLeft"",
""speed"": {toConvert.Levels[i].MoveBottomLeft.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveBottomRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveBottomRight"",
""speed"": {toConvert.Levels[i].MoveBottomRight.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveLeft"",
""speed"": {toConvert.Levels[i].MoveLeft.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveRight"",
""speed"": {toConvert.Levels[i].MoveRight.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveTop.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveTop"",
""speed"": {toConvert.Levels[i].MoveTop.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveTopLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveTopLeft"",
""speed"": {toConvert.Levels[i].MoveTopLeft.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].MoveTopRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveTopRight"",
""speed"": {toConvert.Levels[i].MoveTopRight.Count() / toConvert.Levels[i].MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayBottom.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayBottom"",
""speed"": {toConvert.Levels[i].StayBottom.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayBottomLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayBottomLeft"",
""speed"": {toConvert.Levels[i].StayBottomLeft.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayBottomRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayBottomRight"",
""speed"": {toConvert.Levels[i].StayBottomRight.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayLeft"",
""speed"": {toConvert.Levels[i].StayLeft.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayRight"",
""speed"": {toConvert.Levels[i].StayRight.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayTop.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayTop"",
""speed"": {toConvert.Levels[i].StayTop.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayTopLeft.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayTopLeft"",
""speed"": {toConvert.Levels[i].StayTopLeft.Count() / toConvert.Levels[i].StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.Levels[i].StayTopRight.Select(f => $"SubResource({toConvert.Levels[i].AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayTopRight"",
""speed"": {toConvert.Levels[i].StayTopRight.Count() / toConvert.Levels[i].StayLength}
}} ]

[sub_resource type=""AnimationNodeStateMachinePlayback"" id={toConvert.Levels[i].AllSprites.Count + 2}]

[node name=""{filename}"" instance=ExtResource( 2 )]
script = ExtResource( 3 )
MaxHP = 30
HP = 30
AttackPower = 5
AttackDistance = 100.0
AttackDelay = {toConvert.Levels[i].AttackLength}
HitDelay = {toConvert.Levels[i].HitDelay}
MoveSpeed = 100.0
MoveFloors = PoolIntArray( 2 )

[node name=""Healthbar"" parent=""."" index=""0""]
position = Vector2( 28, -41 )

[node name=""AnimatedSprite"" parent=""."" index=""1""]
position = Vector2( 0, -49 )
frames = SubResource( {toConvert.Levels[i].AllSprites.Count + 1} )
animation = ""StayBottom""
centered = true

[node name=""AnimationTree"" parent=""."" index=""4""]
parameters/playback = SubResource( {toConvert.Levels[i].AllSprites.Count + 2} )

[editable path=""QuestPopup""]
[editable path=""SignPopup""]
"
                };
            }
        }
    }
}