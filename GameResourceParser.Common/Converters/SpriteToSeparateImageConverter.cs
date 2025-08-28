namespace AllodsParser
{
    public class SpriteToSeparateImageConverter : BaseFileConverter<SpriteFile>
    {
        protected override IEnumerable<BaseFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteOutput != GameParserConfigurator.SpriteOutputFormat.EachSprite)
            {
                yield return toConvert;
                yield break;
            }
            for (int i = 0; i < toConvert.Levels.Count; i++)
            {
                for (int j = 0; j < toConvert.Levels[i].AllSprites.Count; j++)
                {
                    var newImage = toConvert.Levels[i].AllSprites[j];
                    yield return new ImageFile
                    {
                        Image = newImage,
                        relativeFileExtension = ".png",
                        relativeFileDirectory = Path.Combine(toConvert.relativeFileDirectory, toConvert.relativeFileName),
                        relativeFileName = toConvert.relativeFileName + "." + i + "." + j
                    };
                }
            }
        }
    }
}