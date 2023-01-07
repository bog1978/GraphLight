using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public abstract class GraphLayout<V, E>
    {
        public INodeMeasure<V, E> NodeMeasure { get; set; }

        public IGraph<V, E> Graph { get; set; }

        public abstract void Layout();
    }
}