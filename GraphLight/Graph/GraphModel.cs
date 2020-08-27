namespace GraphLight.Graph
{
    public class GraphModel<TVertex, TEdge> : GraphModelBase<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override TEdge CreateEdgeData() => new TEdge();

        protected override TVertex CreateVertexData() => new TVertex();
    }

    public class GraphModel : GraphModelBase<object, object>
    {
        protected override object CreateEdgeData() => new object();

        protected override object CreateVertexData() => new object();
    }
}