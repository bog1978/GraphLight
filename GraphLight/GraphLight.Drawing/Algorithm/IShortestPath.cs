using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface IShortestPath<TVertex, TEdge>
    {
        void Find(TVertex start, TVertex end);
        Action<Edge<TVertex, TEdge>> EnterEdge { get; set; }
        Action<Vertex<TVertex, TEdge>> EnterNode { get; set; }
    }
}
