using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class SpriteToGodotImageConverter : BaseFileConverter<SpriteDescriptionFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(SpriteDescriptionFile toConvert, List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteOutput != GameParserConfigurator.SpriteOutputFormat.GodotSprite)
            {
                yield return toConvert;
                yield break;
            }

            var newWidth = toConvert.AllSprites.Max(a => a.Width);
            var newHeight = toConvert.AllSprites.Max(a => a.Height);

            var countWidth = ((int)Math.Sqrt(toConvert.AllSprites.Count)) + 1;
            var countHeight = ((int)Math.Sqrt(toConvert.AllSprites.Count)) + 1;

            var filename = toConvert.relativeFileName;

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

{string.Join("\n", toConvert.AllSprites.Select((a, b) =>

@$"[sub_resource type=""AtlasTexture"" id={b + 1}]
flags = 4
atlas = ExtResource( 1 )
region = Rect2( {newWidth * (b % countWidth)}, {newHeight * (b / countWidth)}, {a.Width}, {a.Height} )
"
))}

[sub_resource type=""SpriteFrames"" id={toConvert.AllSprites.Count + 1}]
animations = [ 
{{""frames"": [ {string.Join(", ", toConvert.AttackBottom.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackBottom"",
""speed"": {toConvert.AttackBottom.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackBottomLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackBottomLeft"",
""speed"": {toConvert.AttackBottomLeft.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackBottomRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackBottomRight"",
""speed"": {toConvert.AttackBottomRight.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackLeft"",
""speed"": {toConvert.AttackLeft.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackRight"",
""speed"": {toConvert.AttackRight.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackTop.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackTop"",
""speed"": {toConvert.AttackTop.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackTopLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackTopLeft"",
""speed"": {toConvert.AttackTopLeft.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.AttackTopRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""AttackTopRight"",
""speed"": {toConvert.AttackTopRight.Count() / toConvert.AttackLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveBottom.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveBottom"",
""speed"": {toConvert.MoveBottom.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveBottomLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveBottomLeft"",
""speed"": {toConvert.MoveBottomLeft.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveBottomRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveBottomRight"",
""speed"": {toConvert.MoveBottomRight.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveLeft"",
""speed"": {toConvert.MoveLeft.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveRight"",
""speed"": {toConvert.MoveRight.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveTop.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveTop"",
""speed"": {toConvert.MoveTop.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveTopLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveTopLeft"",
""speed"": {toConvert.MoveTopLeft.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.MoveTopRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""MoveTopRight"",
""speed"": {toConvert.MoveTopRight.Count() / toConvert.MoveLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayBottom.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayBottom"",
""speed"": {toConvert.StayBottom.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayBottomLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayBottomLeft"",
""speed"": {toConvert.StayBottomLeft.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayBottomRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayBottomRight"",
""speed"": {toConvert.StayBottomRight.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayLeft"",
""speed"": {toConvert.StayLeft.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayRight"",
""speed"": {toConvert.StayRight.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayTop.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayTop"",
""speed"": {toConvert.StayTop.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayTopLeft.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayTopLeft"",
""speed"": {toConvert.StayTopLeft.Count() / toConvert.StayLength}
}}, {{""frames"": [ {string.Join(", ", toConvert.StayTopRight.Select(f => $"SubResource({toConvert.AllSprites.IndexOf(f) + 1})"))} ],
""loop"": true,
""name"": ""StayTopRight"",
""speed"": {toConvert.StayTopRight.Count() / toConvert.StayLength}
}} ]

[sub_resource type=""AnimationNodeStateMachinePlayback"" id={toConvert.AllSprites.Count + 2}]

[node name=""{filename}"" instance=ExtResource( 2 )]
script = ExtResource( 3 )
MaxHP = 30
HP = 30
AttackPower = 5
AttackDistance = 100.0
AttackDelay = {toConvert.AttackLength}
HitDelay = {toConvert.HitDelay}
MoveSpeed = 100.0
MoveFloors = PoolIntArray( 2 )

[node name=""Healthbar"" parent=""."" index=""0""]
position = Vector2( 28, -41 )

[node name=""AnimatedSprite"" parent=""."" index=""1""]
position = Vector2( 0, -49 )
frames = SubResource( {toConvert.AllSprites.Count + 1} )
animation = ""StayBottom""
centered = true

[node name=""AnimationTree"" parent=""."" index=""4""]
parameters/playback = SubResource( {toConvert.AllSprites.Count + 2} )

[editable path=""QuestPopup""]
[editable path=""SignPopup""]
"
            };
        }
    }
}