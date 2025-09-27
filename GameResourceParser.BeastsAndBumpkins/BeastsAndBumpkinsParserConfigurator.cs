public static class BeastsAndBumpkinsParserConfigurator
{
    public static Dictionary<string, Func<BaseFileLoader>> FileFactory = new Dictionary<string, Func<BaseFileLoader>>{
            {".box", ()=>new BOXFileLoader()},
        };

    public static Func<string, List<IBaseFileConverter>> FileConverters = outputFileDirectory => new List<IBaseFileConverter>{
            new BoxUnpackConverter(),
            new MfbToImageConverter(),
            new M10ToWavConverter(),
            new MergeKnownSpritesConverter(),
            new MisToTmxConverter(),

            new SaveSpriteToSeparateImageConverter(outputFileDirectory),
            new SaveSpriteToSingleImageConverter(outputFileDirectory),
            new SkipFileConverter<SpriteFile>(),

            new SpriteDescriptionToGodotImageConverter(),
        };
}