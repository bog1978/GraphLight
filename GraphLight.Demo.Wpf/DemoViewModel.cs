using System.Collections.Generic;
using System.IO;
using GraphLight.Graph;

namespace GraphLight.Demo
{
    public class DemoViewModel : BaseViewModel
    {
        private IGraph? _graph;
        private string? _selectedExample;

        public DemoViewModel()
        {
            var examplesDir = Path.GetFullPath("Examples");
            var di = new DirectoryInfo(examplesDir);
            RootItems = new List<ExampleItem>
            {
                new ExampleDir(di)
            };
        }

        public IGraph? Graph
        {
            get => _graph;
            set => SetProperty(ref _graph, value);
        }

        public IEnumerable<ExampleItem> RootItems { get; }

        public string? SelectedExample
        {
            get => _selectedExample;
            set
            {
                _selectedExample = value;
                RaisePropertyChanged();
                if (value == null)
                    return;
                using var stream = File.OpenRead(value);
                Graph = GraphUtils.LoadLgml(stream);
            }
        }
    }
}