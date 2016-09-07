using GraphLight.Graph;

namespace GraphLight.Layout
{
    public interface INodeMeasure<TVertex, TEdge>
        where TVertex : IVertexAttrs, new()
        //where TEdge : IEdgeAttrs, new()
    {
        void Measure(IVertex<TVertex, TEdge> vertex);
    }

    public abstract class GraphLayout<TVertex, TEdge>
        where TVertex : IVertexAttrs, new()
        //where TEdge : IEdgeAttrs, new()
    {
        public INodeMeasure<TVertex, TEdge> NodeMeasure { get; set; }

        public IGraph<TVertex, TEdge> Graph { get; set; }

        public abstract void Layout();
    }
}