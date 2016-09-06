using System;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public class PrimSpanningTree<TVertex, TEdge>
    {
        private readonly IGraph<TVertex, TEdge> _graph;
        private readonly Func<IEdge<TVertex, TEdge>, double> _weightFunc;
        private Action<IEdge<TVertex, TEdge>> _enterEdge = x => { };

        public PrimSpanningTree(IGraph<TVertex, TEdge> graph,
            Func<IEdge<TVertex, TEdge>, double> weightFunc)
        {
            _graph = graph;
            _weightFunc = weightFunc;
        }

        public Action<IEdge<TVertex, TEdge>> EnterEdge
        {
            get { return _enterEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _enterEdge = value;
            }
        }

        public void Execute(IVertex<TVertex, TEdge> root)
        {
            var attrs = _graph.Verteces.ToDictionary(x => x, x => new PrimAttr());
            var i = 0;
            foreach (var item in _graph.Verteces)
            {
                item.HeapKey = i == 0 ? 0 : double.MaxValue;
                i++;
            }
            var q = new PriorityQueue<double, IVertex<TVertex, TEdge>>(_graph.Verteces, HeapType.Min);
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
            public IEdge<TVertex, TEdge> Parent;

            public PrimAttr()
            {
                Parent = null;
                Color = VertexColor.White;
            }
        }

        #endregion
    }
}