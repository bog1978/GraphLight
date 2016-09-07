using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class DummyNodeMeasure : INodeMeasure<VertexAttrs, EdgeAttrs>
    {
        public void Measure(Vertex<VertexAttrs, EdgeAttrs> vertex)
        {
        }
    }
}