using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface INodeMeasure<V, E>
    {
        void Measure(IVertex<V, E> vertex);
    }
}