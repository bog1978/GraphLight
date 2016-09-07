using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphLight.Graph;
using GraphLight.ViewModel;

namespace GraphLight
{
    public class DemoViewModel : BaseViewModel
    {
        private DrawingGraph _graph;
        private string _selectedExample;
        private int _tabIndex;

        public DemoViewModel()
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            ExampleCollection = resources.Where(x => x.EndsWith(".graph")).ToList();
            SelectedExample = ExampleCollection.FirstOrDefault();
            Palette = new DrawingGraph();
            var v1 = Palette.AddVertex(new VertexAttrs("1"));
            v1.Category = "large_font";
            v1.Label = "AAA";
            var v2 = Palette.AddVertex(new VertexAttrs("2"));
            v2.Category = "small_font";
            v2.Label = "BBB";
            var v3 = Palette.AddVertex(new VertexAttrs("3"));
            v3.Category = "with_tooltip";
            v3.Label = "CCC";
            var v4 = Palette.AddVertex(new VertexAttrs("4"));
            v4.Label = "DDD";
        }

        public DrawingGraph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                RaisePropertyChanged("Graph");
            }
        }

        public DrawingGraph Palette { get; private set; }

        public int TabIndex
        {
            get { return _tabIndex; }
            set
            {
                _tabIndex = value;
                RaisePropertyChanged("TabIndex");
                if (_tabIndex == 1)
                    RaisePropertyChanged("GraphDefinition");
            }
        }

        public string GraphDefinition
        {
            get
            {
                var s = new MemoryStream();
                Graph.WriteToFile(s);
                var data = s.ToArray();
                var str = Encoding.UTF8.GetString(data, 0, data.Length);
                return str;
            }
            set
            {
                //_graphDefinition = value;
                //RaisePropertyChanged("GraphDefinition");
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

        public void Open()
        {
            /*var of = new OpenFileDialog { Filter = "Graph files (*.graph)|*.graph" };
            var result = of.ShowDialog();
            if (result.HasValue && result.Value)
                loadGraph(of.File.OpenRead());*/
        }

        private void loadGraph(Stream stream)
        {
            var graph = DrawingGraph.ReadFromFile(stream);
            //var engine = new GraphVizLayout<VertexAttrs, EdgeAttrs>
            //    {
            //        NodeMeasure = new NodeMeasure(),
            //        Graph = graph
            //    };
            //engine.Layout();
            Graph = graph;
            RaisePropertyChanged("GraphDefinition");
        }
    }
}