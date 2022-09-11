using System;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    internal class PrimSpanningTree<V, E> : ISpanningTree<V, E>
    {
        private readonly IGraph<V, E> _graph;
        private readonly Func<IEdge<V, E>, double> _weightFunc;
        private Action<IEdge<V, E>> _enterEdge = x => { };

        public PrimSpanningTree(IGraph<V, E> graph, Func<IEdge<V, E>, double> weightFunc)
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
            var attrs = _graph.Vertices.ToDictionary(x => x, x => new PrimAttr());
            var i = 0;
            foreach (var item in _graph.Vertices)
            {
                item.HeapKey = i == 0 ? 0 : double.MaxValue;
                i++;
            }
            var q = new PriorityQueue<double, IVertex<V, E>>(_graph.Vertices, HeapType.Min);
            while (!q.IsEmpty)
            {
                var u = q.Dequeue();
                var uAttr = attrs[u];
                uAttr.Color = VertexColor.Black;

                if (uAttr.Parent != null)
                    EnterEdge(uAttr.Parent);

                foreach (var e in u.Edges)
                {
                    var v = e.Src == u ? e.Dst : e.Src;
                    var vAttr = attrs[v];
                    var weight = _weightFunc(e);
                    if (vAttr.Color == VertexColor.White && weight < v.HeapKey)
                    {
                        vAttr.Parent = e;
                        v.HeapKey = _weightFunc(e);
                        q.Remove(v);
                        q.Add(v);
                    }
                }
            }
        }

        #region Nested type: PrimAttr

        private class PrimAttr
        {
            public VertexColor Color;
            public IEdge<V, E> Parent;

            public PrimAttr()
            {
                Parent = null;
                Color = VertexColor.White;
            }
        }

        #endregion
    }
}