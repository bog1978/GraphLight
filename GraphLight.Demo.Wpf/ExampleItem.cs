using System.IO;

namespace GraphLight.Demo
{
    public abstract class ExampleItem
    {
        private readonly FileSystemInfo _fsi;

        protected ExampleItem(FileSystemInfo fsi) => _fsi = fsi;

        public string Name => _fsi.Name;

        public string FullName => _fsi.FullName;

        public override string ToString() => Name;
    }
}