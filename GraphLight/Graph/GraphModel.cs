using GraphLight.Drawing;

namespace GraphLight.Graph
{
    public class GraphModel<TVertex, TEdge> : GraphModelBase<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override IEdge<TVertex, TEdge> CreateEdge(TEdge data) => new Edge<TVertex, TEdge>(data);

        protected override IVertex<TVertex, TEdge> CreateVertex(TVertex data) => new Vertex<TVertex, TEdge>(data);

        protected override TEdge CreateEdgeData() => new TEdge();

        protected override TVertex CreateVertexData() => new TVertex();
    }

    public class GraphModel : GraphModelBase<object, object>
    {
        protected override IEdge<object, object> CreateEdge(object data) => new Edge<object, object>(data);

        protected override IVertex<object, object> CreateVertex(object data) => new Vertex<object, object>(data);

        protected override object CreateEdgeData() => new object();

        protected override object CreateVertexData() => new object();
    }
}