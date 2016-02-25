using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Algorithm
{
    internal abstract partial class NetworkSimplex
    {
        private readonly ICollection<Edge> _headToTailEdges = new List<Edge>();
        private readonly ICollection<Edge> _tailToHeadEdges = new List<Edge>();
        private Edge _exclude;
        private Graph _graph;
        private Edge _include;

        public void Execute()
        {
            ICollection<Vertex> vertices;
            ICollection<Edge> edges;
            Initialize(out vertices, out edges);

            _graph = makeRootedGraph(vertices, edges);

            feasibleTree();
            while (selectEdgeToExclude())
            {
                if (!selectEdgeToInclude())
                    throw new Exception("No edge to include");
                exchange();
            }
            normalize();

            Finalze();
        }

        /// <summary>
        /// Initializes vertex values to sutisfy all restrictions.
        /// Single rooted graph is required to work properly.
        /// </summary>
        private void feasibleTree()
        {
            foreach (var vertex in _graph.Verteces)
                vertex.Value = 0;
            foreach (var edge in _graph.Edges)
                edge.IsVisited = false;

            var root = _graph.Verteces.Single(x => x.InEdges.Length == 0);
            var q = new Queue<Vertex>();
            q.Enqueue(root);

            while (q.Count > 0)
            {
                var v = q.Dequeue();
                if (v.InEdges.Length > 0)
                {
                    var r = v.InEdges.Max(x => x.Src.Value + x.MinLength);
                    if (r > v.Value)
                        v.Value = r;
                }
                foreach (var outEdge in v.OutEdges)
                    outEdge.IsVisited = true;
                foreach (var outEdge in v.OutEdges)
                    if (outEdge.Dst.InEdges.All(x => x.IsVisited))
                        q.Enqueue(outEdge.Dst);
            }
            initCutValues();
        }

        private void initCutValues()
        {
            spanningTree(_graph);
            foreach (var v in _graph.Verteces)
                updateTreeEdges(v);
            postorderTraversal(_graph, _graph.Root);
            updateCutValues(_graph.Verteces);
        }

        private static void updateTreeEdges(Vertex v)
        {
            v.TreeEdgeCount = 0;
            for (var i = 0; i < v.Edges.Length; i++)
            {
                var e = v.Edges[i];
                if (!e.IsTree)
                    continue;
                var tmp = v.Edges[v.TreeEdgeCount];
                v.Edges[v.TreeEdgeCount] = e;
                v.Edges[i] = tmp;
                v.TreeEdgeCount++;
            }
        }

        /// <summary>
        /// vertices must be sorted by Lim.
        /// </summary>
        /// <param name="verteces"></param>
        private static void updateCutValues(IEnumerable<Vertex> verteces)
        {
            foreach (var w in verteces)
            {
                var breakingEdge = w.ParentEdge;
                if (breakingEdge == null)
                    break;

                var sum = 0;
                var cutValue = breakingEdge.Weight;

                var u = breakingEdge.Src;
                var v = breakingEdge.Dst;
                var uLim = u.Lim;
                var vLim = v.Lim;
                var lim = uLim < vLim ? uLim : vLim;
                var low = u.Low > v.Low ? u.Low : v.Low;

                for (var i = w.ParentEdge == null ? 0 : 1; i < w.Edges.Length; i++)
                {
                    var edge = w.Edges[i];
                    if (edge.IsTree)
                    {
                        var s = 0;
                        var u1 = edge.Src;
                        var v1 = edge.Dst;
                        var uLim1 = u1.Lim;
                        var vLim1 = v1.Lim;
                        var lim1 = uLim1 < vLim1 ? uLim1 : vLim1;
                        var low1 = u1.Low > v1.Low ? u1.Low : v1.Low;
                        for (var ni = w.TreeEdgeCount; ni < w.Edges.Length; ni++)
                        {
                            var ne = w.Edges[ni];
                            if (ne.Weight == 0)
                                continue;

                            var srcLim = ne.Src.Lim;
                            var dstLim = ne.Dst.Lim;

                            if (vLim1 < uLim1)
                            {
                                var tmp = srcLim;
                                srcLim = dstLim;
                                dstLim = tmp;
                            }
                            var isTailSrc = low1 <= srcLim && srcLim <= lim1;
                            var isTailDst = low1 <= dstLim && dstLim <= lim1;

                            if (isTailSrc && !isTailDst)
                                s -= ne.Weight;
                            if (!isTailSrc && isTailDst)
                                s += ne.Weight;
                        }

                        if (edge.Src.Lim > edge.Dst.Lim)
                            sum += (edge.CutValue - edge.Weight + s);
                        else
                            sum -= (edge.CutValue - edge.Weight + s);
                    }
                    else
                    {
                        if (edge.Weight == 0)
                            continue;
                        //cutValue += getEdgeType(breakingEdge, edge);

                        var srcLim = edge.Src.Lim;
                        var dstLim = edge.Dst.Lim;

                        if (vLim < uLim)
                        {
                            var tmp = srcLim;
                            srcLim = dstLim;
                            dstLim = tmp;
                        }
                        var isTailSrc = low <= srcLim && srcLim <= lim;
                        var isTailDst = low <= dstLim && dstLim <= lim;

                        if (isTailSrc && !isTailDst)
                            cutValue += edge.Weight; // SEdgeType.TailToHead;
                        if (!isTailSrc && isTailDst)
                            cutValue -= edge.Weight; //SEdgeType.HeadToTail;
                    }
                }

                if (breakingEdge.Src.Lim > breakingEdge.Dst.Lim)
                    cutValue += sum;
                else
                    cutValue -= sum;

                if (breakingEdge.CutValue != cutValue)
                    breakingEdge.CutValue = cutValue;
            }
        }

        private bool selectEdgeToInclude()
        {
            classifyEdges(_exclude);

            _include = null;
            foreach (var edge in _headToTailEdges)
            {
                // Search the edge to include
                var newSlack = edge.Slack();
                if (_include == null || newSlack < _include.Slack())
                    _include = edge;
            }

            return _include != null;
        }

        private bool selectEdgeToExclude()
        {
            _exclude = null;
            for (var i = 0; i < _graph.Edges.Length; i++)
            {
                var edge = _graph.Edges[i];
                if (!edge.IsTree || edge.CutValue >= 0)
                    continue;
                if (_exclude == null || edge.CutValue < _exclude.CutValue)
                    _exclude = edge;
            }
            return _exclude != null;
        }

        private void exchange()
        {
            Vertex localRoot;
            var path = getVertecesToUpdate(out localRoot);

            fixValues(_include.Slack());
            _exclude.IsTree = false;
            _include.IsTree = true;
            updateTreeEdges(_exclude.Src);
            updateTreeEdges(_exclude.Dst);
            updateTreeEdges(_include.Src);
            updateTreeEdges(_include.Dst);

            _exclude.CutValue = 0;
            postorderTraversal(_graph, localRoot);
            updateCutValues(path.OrderBy(x => x.Lim));
        }

        private void fixValues(int slack)
        {
            int min, max;
            if (_exclude.Src.Lim > _exclude.Dst.Lim)
            {
                min = _exclude.Dst.Low;
                max = _exclude.Dst.Lim;
            }
            else
            {
                min = _exclude.Src.Low;
                max = _exclude.Src.Lim;
            }
            for (var i = min; i <= max; i++)
            {
                var v = _graph.Verteces[i];
                v.Value += slack;
            }
        }

        private IEnumerable<Vertex> getVertecesToUpdate(out Vertex root)
        {
            var w = _include.Dst;
            var x = _include.Src;
            var minLim = Math.Min(w.Lim, x.Lim);
            var maxLim = Math.Max(w.Lim, x.Lim);

            var path = new List<Vertex>();
            var l1 = w;
            for (; l1.Low > minLim || l1.Lim < maxLim; l1 = l1.ParentVertex)
                path.Add(l1);
            var l2 = x;
            for (; l2.Low > minLim || l2.Lim < maxLim; l2 = l2.ParentVertex)
                path.Add(l2);

            if (l1 == l2)
            {
                root = l1;
            }
            else
            {
                throw new Exception();
            }

            return path;
        }

        private void normalize()
        {
            // Add 1 to ignore artificial root vertex.
            var minValue = _graph.Verteces.Min(x => x.Value) + 1;
            if (minValue == 0)
                return;
            foreach (var vertex in _graph.Verteces)
                vertex.Value -= minValue;
        }

        private void classifyEdges(Edge breakingEdge)
        {
            _tailToHeadEdges.Clear();
            _headToTailEdges.Clear();
            var u = breakingEdge.Src;
            var v = breakingEdge.Dst;
            var uLim = u.Lim;
            var vLim = v.Lim;
            var lim = Math.Min(uLim, vLim);
            var low = Math.Max(u.Low, v.Low);
            var edges = _graph.Edges;
            var edgeCount = edges.Length;
            for (var i = 0; i < edgeCount; i++)
            {
                var testEdge = edges[i];
                if (testEdge.IsTree && testEdge != breakingEdge)
                    continue;

                var srcLim = testEdge.Src.Lim;
                var dstLim = testEdge.Dst.Lim;

                if (vLim < uLim)
                {
                    var tmp = srcLim;
                    srcLim = dstLim;
                    dstLim = tmp;
                }
                var isTailSrc = low <= srcLim && srcLim <= lim;
                var isTailDst = low <= dstLim && dstLim <= lim;

                if (isTailSrc && !isTailDst)
                    _tailToHeadEdges.Add(testEdge);
                else if (!isTailSrc && isTailDst)
                    _headToTailEdges.Add(testEdge);
            }
        }

        #region Abstract members

        protected abstract void Finalze();
        protected abstract void Initialize(out ICollection<Vertex> vertices, out ICollection<Edge> edges);

        #endregion
    }
}