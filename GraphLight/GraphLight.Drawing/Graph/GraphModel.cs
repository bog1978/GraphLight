namespace GraphLight.Graph
{
    public class GraphModel<TVertex, TEdge> : GraphModelBase<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override TEdge CreateEdgeData()
        {
            return new TEdge();
        }

        protected override TVertex CreateVertexData()
        {
            return new TVertex();
        }
    }

    public class GraphModel : GraphModelBase<object, object>
    {
        protected override object CreateEdgeData()
        {
            return new object();
        }

        protected override object CreateVertexData()
        {
            return new object();
        }
    }
}