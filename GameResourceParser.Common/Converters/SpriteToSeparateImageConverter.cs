namespace AllodsParser
{
    public class SpriteToSeparateImageConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            if (GameParserConfigurator.SpriteMergeVariant != GameParserConfigurator.SpriteMergerFlags.EachSprite)
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

        private IEnumerable<ImageFile> ConvertFile(SpriteFile toConvert, List<BaseFile> files)
        {
            for (int i = 0; i < toConvert.Levels.Count; i++)
            {
                for (int j = 0; j < toConvert.Levels[i].Sprite.Count; j++)
                {
                    var newImage = toConvert.Levels[i].Sprite[j];
                    yield return new ImageFile
                    {
                        Image = newImage,
                        relativeFileExtension = ".png",
                        relativeFileDirectory = toConvert.relativeFileDirectory,
                        relativeFileName = toConvert.relativeFileName + "." + i + "." + j
                    };
                }
            }
        }
    }
}