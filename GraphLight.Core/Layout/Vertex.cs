using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        public Vertex(IVertexData data) : base(data)
        {
        }
    }
}