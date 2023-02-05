using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;

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
            var inEdges = graph.GetInEdges(vertex);
            var outEdges = graph.GetOutEdges(vertex);

            switch (inEdges.Count, outEdges.Count)
            {
                case (0, 1):
                case (1, 0):
                    graph.RemoveVertex(vertex);
                    break;
                case (1, 1):
                    var inEdge = inEdges[0];
                    var outEdge = outEdges[0];
                    graph.ChangeDestination(inEdge, outEdge.Dst);
                    graph.RemoveEdge(outEdge);
                    graph.RemoveVertex(vertex);
                    break;
                default:
                    throw new Exception("Can't remove node");
            }
        }
    }
}