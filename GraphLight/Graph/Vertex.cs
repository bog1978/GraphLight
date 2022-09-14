namespace GraphLight.Graph
{
    public class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        public Vertex(IVertexData data) : base(data)
        {
        }
    }
}