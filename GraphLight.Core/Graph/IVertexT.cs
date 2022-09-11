﻿using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IVertex<TVertexData, TEdgeData>
    {
        TVertexData Data { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> Edges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> InEdges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> OutEdges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> SelfEdges { get; }

        void RegisterEdge(IEdge<TVertexData, TEdgeData> edge);
        void UnRegisterEdge(IEdge<TVertexData, TEdgeData> edge);
    }
}