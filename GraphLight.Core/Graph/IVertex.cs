﻿using System.Collections.Generic;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public interface IVertex<TVertexData, TEdgeData> : IElement<TVertexData>, IBinaryHeapItem<double>
    {
        IEnumerable<IEdge<TVertexData, TEdgeData>> Edges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> InEdges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> OutEdges { get; }
        IEnumerable<IEdge<TVertexData, TEdgeData>> SelfEdges { get; }

        void RegisterEdge(IEdge<TVertexData, TEdgeData> edge);
        void UnRegisterEdge(IEdge<TVertexData, TEdgeData> edge);
    }
}