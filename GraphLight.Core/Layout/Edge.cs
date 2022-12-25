using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal class Edge : BaseEdge<IVertexData, IEdgeData>, IEdge
    {
        public Edge(IEdgeData data) : base(data)
        {
        }
    }
}