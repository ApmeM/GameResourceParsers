namespace AllodsParser
{
    public static class GameParserConfigurator
    {
        public enum SpriteMergerFlags
        {
            SingleSprite,
            PerLevelSprite,
            EachSprite
        }

        public static SpriteMergerFlags SpriteMergeVariant = SpriteMergerFlags.SingleSprite;
    }
}