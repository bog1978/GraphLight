using GraphLight.Graph;

namespace GraphLight.Layout
{
    public interface INodeMeasure<TVertex, TEdge>
        where TVertex : VertexAttrs, new()
        //where TEdge : IEdgeAttrs, new()
    {
        void Measure(Vertex<TVertex, TEdge> vertex);
    }

    public abstract class GraphLayout<TVertex, TEdge>
        where TVertex : VertexAttrs, new() where TEdge : new()
        //where TEdge : IEdgeAttrs, new()
    {
        public INodeMeasure<TVertex, TEdge> NodeMeasure { get; set; }

        public Graph<TVertex, TEdge> Graph { get; set; }

        public abstract void Layout();
    }
}