using System;
using System.Linq;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    internal class KruskalSpanningTree<V, E> : ISpanningTree<V, E>
    where V : IEquatable<V>
    {
        private readonly IGraph<V, E> _graph;
        private readonly Func<IEdge<V, E>, double> _weightFunc;
        private Action<IEdge<V, E>> _enterEdge = x => { };

        public KruskalSpanningTree(IGraph<V, E> graph, Func<IEdge<V, E>, double> weightFunc)
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
            var attrs = _graph.Vertices.ToDictionary(x => x, x => VertexColor.White);
            var sortedEdges = _graph.Edges.OrderBy(_weightFunc).ToList();
            foreach (var edge in sortedEdges)
            {
                var src = edge.Src;
                var dst = edge.Dst;
                if(src.Data.Equals(dst.Data))
                    continue;
                var srcColor = attrs[src];
                var dstColor = attrs[dst];
                if(srcColor == VertexColor.Black && dstColor == VertexColor.Black)
                    continue;
                attrs[src] = VertexColor.Black;
                attrs[dst] = VertexColor.Black;
                EnterEdge(edge);
            }
        }
    }
}