using GraphLight.Graph;

namespace GraphLight.Layout
{
    public interface INodeMeasure<V, E>
    {
        void Measure(IVertex<V, E> vertex);
    }

    public abstract class GraphLayout<V, E>
    {
        public INodeMeasure<V, E> NodeMeasure { get; set; }

        public IGraph<V, E> Graph { get; set; }

        public abstract void Layout();
    }
}