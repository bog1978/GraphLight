namespace GraphLight.Model
{
    public interface IEdge<TVertexData, TEdgeData> : IElement<TEdgeData>
    {
        bool IsRevert { get; }
        IVertex<TVertexData, TEdgeData> Dst { get; }
        IVertex<TVertexData, TEdgeData> Src { get; }
    }
}