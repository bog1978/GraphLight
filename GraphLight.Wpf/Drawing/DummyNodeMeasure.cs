using GraphLight.Algorithm;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class DummyNodeMeasure<V, E> : INodeMeasure<V, E>
    {
        public void Measure(IVertex<V, E> vertex)
        {
        }
    }
}