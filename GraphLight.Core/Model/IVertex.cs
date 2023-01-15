using System.Collections.Generic;

namespace GraphLight.Model
{
    public interface IVertex<TVertexData, TEdgeData> : IElement<TVertexData>
    {
        IEnumerable<IEdge<TVertexData, TEdgeData>> Edges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> InEdges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> OutEdges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> SelfEdges { get; }

        void RegisterEdge(IEdge<TVertexData, TEdgeData> edge);
        void UnRegisterEdge(IEdge<TVertexData, TEdgeData> edge);
    }
}