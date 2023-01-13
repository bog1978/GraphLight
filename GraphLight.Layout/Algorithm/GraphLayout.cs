using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public abstract class GraphLayout<G, V, E>
    {
        public INodeMeasure<V, E> NodeMeasure { get; set; }

        public IGraph<G, V, E> Graph { get; set; }

        public abstract void Layout();
    }
}