using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface INodeMeasure
    {
        void Measure(IVertex<IVertexData, IEdgeData> vertex);
    }
}