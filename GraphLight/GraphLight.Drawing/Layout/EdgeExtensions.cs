using System;
using System.Linq;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    internal static class EdgeExtensions
    {
        public static bool Cross<TVertex, TEdge>(this IEdge<TVertex, TEdge> edge, IEdge<TVertex, TEdge> otherEdge)
            //where TVertex : IVertexAttrs
            //where TEdge : IEdgeAttrs
        {
            var a1 = edge.Src;
            var b1 = edge.Dst;
            var a2 = otherEdge.Src;
            var b2 = otherEdge.Dst;
            return a1.Position < a2.Position && b1.Position > b2.Position
                || a1.Position > a2.Position && b1.Position < b2.Position
                || a1.Position == a2.Position && b1.Position == b2.Position;
        }

        public static double PositionSpan<TVertex, TEdge>(this IEdge<TVertex, TEdge> edge)
            where TVertex : IVertexAttrs
            //where TEdge : IEdgeAttrs
        {
            var delta = edge.Src.Position - edge.Dst.Position;
            return Math.Abs(delta) * edge.Weight;
        }

        public static IVertex<TVertex, TEdge> InsertControlPoint<TVertex, TEdge>(this IGraph<TVertex, TEdge> graph,
            IEdge<TVertex, TEdge> edge)
            where TVertex : IVertexAttrs, new()
        {
            var va = new TVertex();
            return graph.InsertVertex(edge, va);
        }

        public static void RemoveControlPoint<TVertex, TEdge>(this IGraph<TVertex, TEdge> graph,
            IVertex<TVertex, TEdge> vertex)
        {
            var inCnt = vertex.InEdges.Count();
            var outCnt = vertex.OutEdges.Count();

            if (inCnt > 1 || outCnt > 1)
                throw new Exception("Can't remove node");

            if (inCnt == 0 && outCnt == 1)
                graph.RemoveEdge(vertex.OutEdges.First());
            else if (inCnt == 1 && outCnt == 0)
                graph.RemoveEdge(vertex.InEdges.First());
            else if (inCnt == 1 && outCnt == 1)
            {
                var inEdge = vertex.InEdges.First();
                var outEdge = vertex.OutEdges.First();

                inEdge.Dst = outEdge.Dst;
                graph.RemoveEdge(outEdge);
            }
            graph.RemoveVertex(vertex);
        }
    }
}