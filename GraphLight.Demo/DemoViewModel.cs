using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphLight.Graph;
using GraphLight.Parser;

namespace GraphLight
{
    public class DemoViewModel : BaseViewModel
    {
        private IGraph _graph;
        private string _selectedExample;
        private int _tabIndex;

        public DemoViewModel()
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            ExampleCollection = resources.Where(x => x.EndsWith(".graph")).ToList();
            SelectedExample = ExampleCollection.FirstOrDefault();
            Palette = new LayoutGraphModel();

            var v1 = Palette.CreateVertexData("1");
            v1.Label = "AAA";
            v1.Category = "large_font";
            Palette.AddVertex(v1);

            var v2 = Palette.CreateVertexData("2");
            v2.Label = "BBB";
            v2.Category = "small_font";
            Palette.AddVertex(v2);

            var v3 = Palette.CreateVertexData("3");
            v3.Label = "CCC";
            v3.Category = "with_tooltip";
            Palette.AddVertex(v3);

            var v4 = Palette.CreateVertexData("4");
            v4.Label = "DDD";
            Palette.AddVertex(v4);
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

        public IGraph Palette { get; private set; }

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
            var graph = GraphHelper.ReadFromFile(stream);
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