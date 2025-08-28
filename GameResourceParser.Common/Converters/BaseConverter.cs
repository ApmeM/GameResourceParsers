
namespace AllodsParser
{

    public abstract class BaseFileConverter<T> : IBaseFileConverter where T : BaseFile
    {
        public void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<T>()
                .ToList();

            Console.WriteLine($"{this.GetType()} start converting {oldFiles.Count} files of type {typeof(T)}.");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            Console.WriteLine($"{this.GetType()} finish converting {oldFiles.Count} files of type {typeof(T)} to {newFiles.Count} files of type {string.Join(",", newFiles.GroupBy(a => a.GetType()).Select(a => $"{a.Key}:{a.Count()}"))}.");

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        protected abstract IEnumerable<BaseFile> ConvertFile(T a, List<BaseFile> files);
    }
}