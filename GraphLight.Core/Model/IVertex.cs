using System.Collections.Generic;

namespace GraphLight.Model
{
    public interface IVertex<TVertexData, TEdgeData> : IElement<TVertexData>
    {
        IReadOnlyList<IEdge<TVertexData, TEdgeData>> Edges { get; }
        IReadOnlyList<IEdge<TVertexData, TEdgeData>> InEdges { get; }
        IReadOnlyList<IEdge<TVertexData, TEdgeData>> OutEdges { get; }
        IReadOnlyList<IEdge<TVertexData, TEdgeData>> SelfEdges { get; }

        void RegisterEdge(IEdge<TVertexData, TEdgeData> edge);
        void UnRegisterEdge(IEdge<TVertexData, TEdgeData> edge);
    }
}