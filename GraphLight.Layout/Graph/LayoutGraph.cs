namespace GraphLight.Graph
{
    public class LayoutGraph : GenericGraph<IGraphData, IVertexData, IEdgeData>, IGraph
    {
        public LayoutGraph(IGraphData data) : base(data)
        {
        }

        public override IVertexData CreateVertexData(object id) => new VertexData(id.ToString(), null, null);

        public override IEdgeData CreateEdgeData() => new EdgeData(null, null);
    }

    public interface IGraphData
    {
        double Width { get; set; }

        double Height { get; set; }
    }

    public class GraphData : IGraphData
    {
        public double Width { get; set; }

        public double Height { get; set; }
    }
}