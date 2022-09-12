using GraphLight.Drawing;

namespace GraphLight.Graph
{
    public class GraphModel<TVertex, TEdge> : GraphModelBase<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override IEdge<TVertex, TEdge> CreateEdge(TEdge data) => new Edge<TVertex, TEdge>(data);

        protected override IVertex<TVertex, TEdge> CreateVertex(TVertex data) => new Vertex<TVertex, TEdge>(data);
    }

    public class GraphModel : GraphModelBase<object, object>
    {
        protected override IEdge<object, object> CreateEdge(object data) => new Edge<object, object>(data);

        protected override IVertex<object, object> CreateVertex(object data) => new Vertex<object, object>(data);
    }
}