using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphLight.Graph;
using GraphLight.Parser;

namespace GraphLight
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
            get { return _graph; }
            set
            {
                _graph = value;
                RaisePropertyChanged("Graph");
            }
        }

        public List<string> ExampleCollection { get; private set; }

        public string SelectedExample
        {
            get { return _selectedExample; }
            set
            {
                _selectedExample = value;
                RaisePropertyChanged("SelectedExample");
                if (value == null)
                    return;
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(value);
                loadGraph(stream);
            }
        }

        private void loadGraph(Stream stream)
        {
            var graph = GraphHelper.ReadFromFile(stream);
            Graph = graph;
            RaisePropertyChanged("GraphDefinition");
        }
    }
}