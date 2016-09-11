using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class DrawingGraph : BaseGraph<string, string>
    {
        private static int _cnt;
        protected override Edge<string, string> CreateEdge(string data)
        {
            return new DrawingEdge(data);
        }

        protected override Vertex<string, string> CreateVertex(string data)
        {
            return new DrawingVertex(data);
        }

        protected override string CreateEdgeData()
        {
            return string.Empty;
        }

        protected override string CreateVertexData()
        {
            _cnt++;
            return _cnt.ToString();
        }
    }
}