namespace GraphLight.Graph
{
    internal class LayoutEdge : BaseEdge<IVertexData, IEdgeData>, IEdge
    {
        public LayoutEdge(IEdgeData data) : base(data)
        {
        }
    }
}