namespace AllodsParser
{
    public static class GameParserConfigurator
    {
        public enum SpriteOutputFormat
        {
            SingleSprite,
            PerLevelSprite,
            EachSprite,
            GodotSprite
        }

        public static SpriteOutputFormat SpriteOutput = SpriteOutputFormat.GodotSprite;
    }
}