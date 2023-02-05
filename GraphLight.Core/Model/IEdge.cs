namespace GraphLight.Model
{
    public interface IEdge<TVertexData, TEdgeData> : IElement<TEdgeData>
    {
        bool IsRevert { get; }
        IVertex<TVertexData> Dst { get; }
        IVertex<TVertexData> Src { get; }
    }
}