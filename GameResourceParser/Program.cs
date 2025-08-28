using System.Text;

namespace AllodsParser
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var type = "Allods";
            var resources = "/home/vas/Downloads/Sprites/Allods2_en/data/";
            var result = "../Result/Allods";

            if (args.Length > 0)
            {
                resources = args[0];
            }

            if (args.Length > 1)
            {
                result = args[1];
            }

            if (args.Length > 2)
            {
                type = args[2];
            }

            Dictionary<string, Func<BaseFileLoader>> FileFactory;
            List<IBaseFileConverter> FileConverters;

            switch (type.ToLower())
            {
                case "allods":
                    FileFactory = AllodsParserConfigurator.FileFactory;
                    FileConverters = AllodsParserConfigurator.FileConverters(result);
                    break;
                default:
                    Console.WriteLine("Can not recognize parser type.");
                    return;
            }

            if (Directory.Exists(result))
            {
                Directory.Delete(result, true);
            }
            Directory.CreateDirectory(result);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var knownUnknowns = new HashSet<string>();

            var files = new List<BaseFile>();

            Console.WriteLine("Loading...");

            foreach (var filePath in Directory.GetFiles(resources, "*.*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(filePath);
                if (!FileFactory.ContainsKey(ext))
                {
                    if (!knownUnknowns.Contains(ext))
                    {
                        Console.Error.WriteLine($"Unknown file extension: {ext}");
                        knownUnknowns.Add(ext);
                    }
                    continue;
                }

                var relativePath = Path.GetRelativePath(resources, filePath);

                var bytes = File.ReadAllBytes(filePath);

                files.Add(FileFactory[ext]().Load(relativePath, bytes));
            }

            Console.WriteLine("Converting...");
            FileConverters.ForEach(a => a.Convert(files));

            Console.WriteLine("Saving.");
            foreach (var file in files)
            {
                file.Save(result);
            }
        }
    }
}
