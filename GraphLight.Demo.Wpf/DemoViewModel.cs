using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphLight.Graph;
using GraphLight.Parser;

namespace GraphLight.Demo
{
    public class DemoViewModel : BaseViewModel
    {
        private IGraph _graph;
        private string _selectedExample;

        public DemoViewModel()
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            ExampleCollection = resources.Where(x => x.EndsWith(".graph")).ToList();
            SelectedExample = ExampleCollection.FirstOrDefault();
        }

        public IGraph Graph
        {
            get => _graph;
            set
            {
                _graph = value;
                RaisePropertyChanged();
            }
        }

        public List<string> ExampleCollection { get; }

        public string SelectedExample
        {
            get => _selectedExample;
            set
            {
                _selectedExample = value;
                RaisePropertyChanged();
                if (value == null)
                    return;
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(value);
                LoadGraph(stream);
            }
        }

        private void LoadGraph(Stream stream) => 
            Graph = GraphHelper.ReadFromFile(stream);
    }
}