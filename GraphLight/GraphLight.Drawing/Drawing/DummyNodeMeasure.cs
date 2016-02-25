using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class DummyNodeMeasure : INodeMeasure<VertexAttrs, EdgeAttrs>
    {
        public void Measure(IVertex<VertexAttrs, EdgeAttrs> vertex)
        {
        }
    }
}