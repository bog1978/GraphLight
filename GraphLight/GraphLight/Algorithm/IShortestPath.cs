using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface IShortestPath<TVertex, TEdge>
    {
        void Find(TVertex start, TVertex end);
        Action<IEdge<TVertex, TEdge>> EnterEdge { get; set; }
        Action<IVertex<TVertex, TEdge>> EnterNode { get; set; }
    }
}
