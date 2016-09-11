using System;
using System.Linq;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    public class PrimSpanningTree
    {
        private readonly IGraph _graph;
        private readonly Func<IEdge, double> _weightFunc;
        private Action<IEdge> _enterEdge = x => { };

        public PrimSpanningTree(IGraph graph, Func<IEdge, double> weightFunc)
        {
            _graph = graph;
            _weightFunc = weightFunc;
        }

        public Action<IEdge> EnterEdge
        {
            get { return _enterEdge; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _enterEdge = value;
            }
        }

        public void Execute(IVertex root)
        {
            var attrs = _graph.Verteces.ToDictionary(x => x, x => new PrimAttr());
            var i = 0;
            foreach (var item in _graph.Verteces)
            {
                item.HeapKey = i == 0 ? 0 : double.MaxValue;
                i++;
            }
            var q = new PriorityQueue<double, IVertex>(_graph.Verteces, HeapType.Min);
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
            public IEdge Parent;

            public PrimAttr()
            {
                Parent = null;
                Color = VertexColor.White;
            }
        }

        #endregion
    }
}