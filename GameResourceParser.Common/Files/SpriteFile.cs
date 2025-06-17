using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class SpriteFile : BaseFile
{
    public class SpriteLevel
    {
        public List<Image<Rgba32>> Sprite;
    }

    public List<SpriteLevel> Levels;
}
