using System.Collections.Generic;
using System.IO;

namespace GraphLight.Demo
{
    public class ExampleDir : ExampleItem
    {
        private readonly ICollection<ExampleItem> _children;
        public ExampleDir(DirectoryInfo di) : base(di)
        {
            _children = new List<ExampleItem>();
            foreach (var di1 in di.GetDirectories())
                _children.Add(new ExampleDir(di1));
            foreach (var fi1 in di.GetFiles("*.xml")) 
                _children.Add(new ExampleFile(fi1));
        }

        public IEnumerable<ExampleItem> Children => _children;
    }
}