using System;

namespace GraphLight.Graph
{
    public class EdgeChangedEventArgs<V, E> : EventArgs
    {
        public EdgeChangedEventArgs(IVertex<V, E> oldVertex, IVertex<V, E> newVertex)
        {
            OldVertex = oldVertex;
            NewVertex = newVertex;
        }

        public IVertex<V, E> OldVertex { get; }
        public IVertex<V, E> NewVertex { get; }
    }
}