namespace GraphLight.Graph
{
    internal class LayoutVertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        public LayoutVertex(IVertexData data) : base(data)
        {
        }
    }
}