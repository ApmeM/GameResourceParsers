namespace AllodsParser
{
    public static class AllodsParserConfigurator
    {
        public static Dictionary<string, Func<BaseFileLoader>> FileFactory = new Dictionary<string, Func<BaseFileLoader>>{
            {".ttf", ()=>new BinaryFileLoader()},
            {".bmp", ()=>new BinaryFileLoader()},
            {".png", ()=>new BinaryFileLoader()},
            {".wav", ()=>new BinaryFileLoader()},
            {".txt", ()=>new BinaryFileLoader()},
            {".reg", ()=>new RegFileLoader()},
            {".16",  ()=>new Image16FileLoader()},
            {".16a", ()=>new Image16aFileLoader()},
            {".256", ()=>new Image256FileLoader()},
            {".alm", ()=>new AlmFileLoader()},
            {".pal", ()=>new PalFileLoader()},
        };

        public static Func<string, List<IBaseFileConverter>> FileConverters = outputFileDirectory => new List<IBaseFileConverter>{
            new RegToStructuresConverter(),
            new RegToUnitsConverter(),
            new RegToObjectsConverter(),
            new SaveFileConverter<RegFile>(outputFileDirectory),
            new PalMergerConverter(),
            new UnitsPalMergerConverter(),
            new StructuresReconstructionConverter(),
            new PaletteToSpriteConverter(),
            new SpriteToSeparateImageConverter(),
            new SpriteToPerLevelImageConverter(),
            new SpriteToSingleImageConverter(),
            new SpriteToGodotImageConverter(),
            new AlmToTmxConverter(),
        };
    }
}
