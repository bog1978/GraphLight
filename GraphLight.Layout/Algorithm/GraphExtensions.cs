using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    internal static class GraphExtensions
    {
        internal static IEnumerable<List<IVertex<V, E>>> GetRankList<G, V, E>(this IGraph<G, V, E> graph)
            where V : IVertexDataLayered
        {
            return
                from node in graph.Vertices
                orderby node.Data.Rank, node.Data.Position
                group node by node.Data.Rank
                    into rank
                select rank.ToList();
        }

        internal static IVertex<V, E> InsertControlPoint<G, V, E>(this IGraph<G, V, E> graph, IEdge<V, E> edge, V vertexData, E edgeData)
        {
            return graph.InsertVertex(edge, vertexData, edgeData);
        }

        internal static void RemoveControlPoint<G, V, E>(this IGraph<G, V, E> graph, IVertex<V, E> vertex)
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