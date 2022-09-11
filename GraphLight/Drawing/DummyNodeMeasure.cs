using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class DummyNodeMeasure<V, E> : INodeMeasure<V, E>
    {
        public void Measure(IVertex<V, E> vertex)
        {
        }
    }
}