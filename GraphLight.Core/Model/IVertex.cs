namespace GraphLight.Model
{
    public interface IVertex<TVertexData, TEdgeData> : IElement<TVertexData>
    {
        void RegisterEdge(IEdge<TVertexData, TEdgeData> edge);
        void UnRegisterEdge(IEdge<TVertexData, TEdgeData> edge);
    }
}