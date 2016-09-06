using System.Collections.Generic;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public interface IVertex<TVertex, TEdge> : IBinaryHeapItem<double>
    {
        string Id { get; }
        TVertex Data { get; set; }
        IEnumerable<IEdge<TVertex, TEdge>> Edges { get; }
        IEnumerable<IEdge<TVertex, TEdge>> InEdges { get; }
        IEnumerable<IEdge<TVertex, TEdge>> OutEdges { get; }
        IEnumerable<IEdge<TVertex, TEdge>> SelfEdges { get; }
        void RegisterEdge(IEdge<TVertex, TEdge> edge);
        void UnregisterEdge(IEdge<TVertex, TEdge> edge);
    }
}