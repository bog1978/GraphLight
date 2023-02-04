using System;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class KruskalSpanningTree<G, V, E> : ISpanningTree<V, E>
    where V : IEquatable<V>
    {
        private readonly IGraph<G, V, E> _graph;
        private readonly Func<IEdge<V, E>, double> _weightFunc;
        private Action<IEdge<V, E>> _enterEdge = x => { };

        public KruskalSpanningTree(IGraph<G, V, E> graph, Func<IEdge<V, E>, double> weightFunc)
        {
            _graph = graph;
            _weightFunc = weightFunc;
        }

        public Action<IEdge<V, E>> EnterEdge
        {
            get => _enterEdge;
            set => _enterEdge = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void Execute(IVertex<V, E> root)
        {
            var ds = new DisjointSet<IVertex<V, E>>(_graph.Vertices);
            var sortedEdges = _graph.Edges.OrderBy(_weightFunc).ToList();
            foreach (var edge in sortedEdges)
                if (ds.Unite(edge.Src, edge.Dst))
                    EnterEdge(edge);
        }
    }
}