using System;
using System.Linq;
using GraphLight.Graph;

namespace GraphLight.Layout
{
    public static class EdgeExtensions
    {
        public static bool Cross(this IEdge edge, IEdge otherEdge)
        {
            var a1 = edge.Src;
            var b1 = edge.Dst;
            var a2 = otherEdge.Src;
            var b2 = otherEdge.Dst;
            return a1.Position < a2.Position && b1.Position > b2.Position
                || a1.Position > a2.Position && b1.Position < b2.Position
                || a1.Position == a2.Position && b1.Position == b2.Position;
        }

        public static double PositionSpan(this IEdge edge)
        {
            var delta = edge.Src.Position - edge.Dst.Position;
            return Math.Abs(delta) * edge.Weight;
        }

        public static IVertex InsertControlPoint(this IGraph graph, IEdge edge, object data)
        {
            return graph.InsertVertex(edge, data);
        }

        public static void RemoveControlPoint(this IGraph graph, IVertex vertex)
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