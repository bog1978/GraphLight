using System;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public interface IShortestPath
    {
        void Find(object start, object end);
        Action<IEdge> EnterEdge { get; set; }
        Action<IVertex> EnterNode { get; set; }
    }
}
