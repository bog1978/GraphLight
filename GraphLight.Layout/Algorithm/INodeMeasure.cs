using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public interface INodeMeasure
    {
        void Measure(IVertex<IVertexData, IEdgeData> vertex);
    }
}