using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GraphLight.Collections;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public abstract partial class NetworkSimplex2 : IAlgorithm
    {
        #region Константы и поля

        private readonly ICollection<IEdge<VertexData, EdgeData>> _headToTailEdges = new List<IEdge<VertexData, EdgeData>>();
        private readonly ICollection<IEdge<VertexData, EdgeData>> _tailToHeadEdges = new List<IEdge<VertexData, EdgeData>>();
        private IEdge<VertexData, EdgeData> _exclude;
        private IEdge<VertexData, EdgeData> _include;
        private IGraph<GraphData, VertexData, EdgeData> _graph;
        private int _step;

        #endregion

        #region IAlgorithm

        public void Execute()
        {
            _graph = Graph.CreateInstance<GraphData, VertexData, EdgeData>(new GraphData());
            Initialize(_graph);
            MakeRootedGraph(_graph);

            FeasibleTree();
            InitCutValues();

            //while (SelectEdgeToExclude())
            //{
            //    if (!SelectEdgeToInclude())
            //        throw new Exception("No edge to include");
            //    Exchange();
            //}
            //Normalize();

            //Finalze();
        }

        public Action<string, IGraph<GraphData, VertexData, EdgeData>>? Step;

        private void OnStep([CallerMemberName] string? member = null) => Step?.Invoke($"{member}_{_step++}", _graph);

        #endregion

        #region Другое

        protected abstract void Finalze();

        protected abstract void Initialize(IGraph<GraphData, VertexData, EdgeData> graph);

        private static int Length(IEdge<VertexData, EdgeData> edge) => Math.Abs(edge.Dst.Value - edge.Src.Value);

        private void MakeRootedGraph(IGraph<GraphData, VertexData, EdgeData> graph)
        {
            var root = new VertexData("_ROOT_");
            var roots = graph.GetRoots();
            foreach (var r in roots)
                graph.AddEdge(root, r, new EdgeData(0, 1));
            graph.Data.Root = root;
            OnStep();
        }

        private void PostOrderTraversal(IGraph<GraphData, VertexData, EdgeData> graph)
        {
            var alg = graph.DepthFirstSearch(TraverseRule.PostOrder);
            alg.OnNode = ni =>
            {
                var currAttr = ni.Vertex;
                var outEdges = graph.GetOutEdges(ni.Vertex);
                currAttr.Lim = ni.Order;
                currAttr.Low = ni.VertexType switch
                {
                    DfsVertexType.Leaf => ni.Order,
                    DfsVertexType.LeafCycle => ni.Order,
                    _ => outEdges.Min(x => x.Dst.Low)
                };
            };
            alg.Execute();
            OnStep();
        }

        private static int Slack(IEdge<VertexData, EdgeData> edge) => edge.Dst.Value - edge.Src.Value - edge.Data.MinLength;

        private void SpanningTree(IGraph<GraphData, VertexData, EdgeData> graph)
        {
            graph.Data.Root.Priority = 0;
            var heap = new BinaryHeap<int, VertexData>(graph.Vertices, x => x.Priority, HeapType.Min);
            while (heap.Count > 0)
            {
                var u = heap.RemoveRoot();
                u.Color = VertexColor.Black;

                if (u.ParentEdge != null)
                    u.ParentEdge.Data.IsTree = true;

                foreach (var e in graph.GetEdges(u))
                {
                    var v = e.Src == u ? e.Dst : e.Src;
                    var weight = Length(e);
                    if (v.Color != VertexColor.White || weight >= v.Priority)
                        continue;
                    v.ParentEdge = e;
                    v.Priority = weight;
                    heap.Remove(v);
                    heap.Add(weight, v);
                }
            }
            OnStep();
        }

        private void ClassifyEdges(IEdge<VertexData, EdgeData> breakingEdge)
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
            var edgeCount = edges.Count;
            for (var i = 0; i < edgeCount; i++)
            {
                var testEdge = edges[i];
                if (testEdge.Data.IsTree && testEdge != breakingEdge)
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

        private void Exchange()
        {
            var path = GetVerticesToUpdate(out var localRoot);

            FixValues(Slack(_include));
            _exclude.Data.IsTree = false;
            _include.Data.IsTree = true;
            //UpdateTreeEdges(_exclude.Src);
            //UpdateTreeEdges(_exclude.Dst);
            //UpdateTreeEdges(_include.Src);
            //UpdateTreeEdges(_include.Dst);

            _exclude.Data.CutValue = 0;
            PostOrderTraversal(_graph);
            UpdateCutValues(path.OrderBy(x => x.Lim));
        }

        /// <summary>
        ///     Initializes vertex values to satisfy all restrictions.
        ///     Single rooted graph is required to work properly.
        /// </summary>
        private void FeasibleTree()
        {
            foreach (var vertex in _graph.Vertices)
                vertex.Value = 0;

            var dfs = _graph.DepthFirstSearch(TraverseRule.PreOrder);
            dfs.OnEdge = 
                ei => ei.Edge.Dst.Value = Math.Max(
                    ei.Edge.Dst.Value,
                    ei.Edge.Src.Value + ei.Edge.Data.MinLength);
            dfs.Execute();

            OnStep();
        }

        private void FixValues(int slack)
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
                var v = _graph.Vertices[i];
                v.Value += slack;
            }
        }

        private IEnumerable<VertexData> GetVerticesToUpdate(out VertexData root)
        {
            var w = _include.Dst;
            var x = _include.Src;
            var minLim = Math.Min(w.Lim, x.Lim);
            var maxLim = Math.Max(w.Lim, x.Lim);

            var path = new List<VertexData>();
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

        private void InitCutValues()
        {
            SpanningTree(_graph);
            //foreach (var v in _graph.Vertices)
            //    UpdateTreeEdges(v);
            PostOrderTraversal(_graph);
            //UpdateCutValues(_graph.Vertices);
        }

        private void Normalize()
        {
            // Add 1 to ignore artificial root vertex.
            var minValue = _graph.Vertices.Min(x => x.Value) + 1;
            if (minValue == 0)
                return;
            foreach (var vertex in _graph.Vertices)
                vertex.Value -= minValue;
        }

        private bool SelectEdgeToExclude()
        {
            _exclude = null;
            foreach (var edge in _graph.Edges)
            {
                if (!edge.Data.IsTree || edge.Data.CutValue >= 0)
                    continue;
                if (_exclude == null || edge.Data.CutValue < _exclude.Data.CutValue)
                    _exclude = edge;
            }
            return _exclude != null;
        }

        private bool SelectEdgeToInclude()
        {
            ClassifyEdges(_exclude);

            _include = null;
            foreach (var edge in _headToTailEdges)
            {
                // Search the edge to include
                var newSlack = Slack(edge);
                if (_include == null || newSlack < Slack(_include))
                    _include = edge;
            }

            return _include != null;
        }

        /// <summary>
        ///     vertices must be sorted by Lim.
        /// </summary>
        /// <param name="vertices"></param>
        private void UpdateCutValues(IEnumerable<VertexData> vertices)
        {
            foreach (var w in vertices)
            {
                var breakingEdge = w.ParentEdge;
                if (breakingEdge == null)
                    break;

                var sum = 0;
                var cutValue = breakingEdge.Data.Weight;

                var u = breakingEdge.Src;
                var v = breakingEdge.Dst;
                var uLim = u.Lim;
                var vLim = v.Lim;
                var lim = uLim < vLim
                    ? uLim
                    : vLim;
                var low = u.Low > v.Low
                    ? u.Low
                    : v.Low;

                var wEdges = _graph.GetEdges(w);
                for (var i = w.ParentEdge == null
                         ? 0
                         : 1;
                     i < wEdges.Count;
                     i++)
                {
                    var edge = wEdges[i];
                    if (edge.Data.IsTree)
                    {
                        var s = 0;
                        var u1 = edge.Src;
                        var v1 = edge.Dst;
                        var uLim1 = u1.Lim;
                        var vLim1 = v1.Lim;
                        var lim1 = uLim1 < vLim1
                            ? uLim1
                            : vLim1;
                        var low1 = u1.Low > v1.Low
                            ? u1.Low
                            : v1.Low;
                        for (var ni = 0; ni < wEdges.Count; ni++)
                        {
                            var ne = wEdges[ni];
                            if(ne.Data.IsTree)
                                continue;
                            if (ne.Data.Weight == 0)
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
                                s -= ne.Data.Weight;
                            if (!isTailSrc && isTailDst)
                                s += ne.Data.Weight;
                        }

                        if (edge.Src.Lim > edge.Dst.Lim)
                            sum += (edge.Data.CutValue - edge.Data.Weight + s);
                        else
                            sum -= (edge.Data.CutValue - edge.Data.Weight + s);
                    }
                    else
                    {
                        if (edge.Data.Weight == 0)
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
                            cutValue += edge.Data.Weight; // SEdgeType.TailToHead;
                        if (!isTailSrc && isTailDst)
                            cutValue -= edge.Data.Weight; //SEdgeType.HeadToTail;
                    }
                }

                if (breakingEdge.Src.Lim > breakingEdge.Dst.Lim)
                    cutValue += sum;
                else
                    cutValue -= sum;

                if (breakingEdge.Data.CutValue != cutValue)
                    breakingEdge.Data.CutValue = cutValue;
            }
        }

        //private void UpdateTreeEdges(VertexData v)
        //{
        //    v.TreeEdgeCount = 0;
        //    var edges = _graph.GetEdges(v);
        //    for (var i = 0; i < edges.Count; i++)
        //    {
        //        var e = edges[i];
        //        if (e.Data.IsTree)
        //        {
        //            // var tmp = edges[v.TreeEdgeCount];
        //            // edges[v.TreeEdgeCount] = e;
        //            // edges[i] = tmp;
        //            v.TreeEdgeCount++;
        //        }
        //    }
        //}

        #endregion
    }
}