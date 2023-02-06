using System;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal class PrimSpanningTree<G, V, E> : ISpanningTree<V, E>
        where V : class, IEquatable<V>
    {
        private readonly IGraph<G, V, E> _graph;
        private readonly Func<IEdge<V, E>, double> _weightFunc;
        private Action<IEdge<V, E>> _enterEdge = x => { };

        public PrimSpanningTree(IGraph<G, V, E> graph, Func<IEdge<V, E>, double> weightFunc)
        {
            _graph = graph;
            _weightFunc = weightFunc;
        }

        public Action<IEdge<V, E>> EnterEdge
        {
            get => _enterEdge;
            set => _enterEdge = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void Execute(V root)
        {
            var attrs = _graph.Vertices.ToDictionary(x => x, x => new PrimAttr());
            attrs[root].HeapKey = 0;

            var q = new PriorityQueue<double, V>(
                _graph.Vertices, x => attrs[x].HeapKey, HeapType.Min);

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                var uAttr = attrs[u];
                uAttr.Color = VertexColor.Black;

                if (uAttr.Parent != null)
                    EnterEdge(uAttr.Parent);

                foreach (var e in _graph.GetEdges(u))
                {
                    var v = u.Equals(e.Src) ? e.Dst : e.Src;
                    var vAttr = attrs[v];
                    var weight = _weightFunc(e);
                    if (vAttr.Color == VertexColor.White && weight < vAttr.HeapKey)
                    {
                        vAttr.Parent = e;
                        vAttr.HeapKey = _weightFunc(e);
                        q.Remove(v);
                        q.Enqueue(v, _weightFunc(e));
                    }
                }
            }
        }

        #region Nested type: PrimAttr

        private class PrimAttr
        {
            public VertexColor Color;
            public IEdge<V, E>? Parent;
            public double HeapKey;

            public PrimAttr()
            {
                Parent = null;
                Color = VertexColor.White;
                HeapKey = double.MaxValue;
            }
        }

        #endregion
    }
}