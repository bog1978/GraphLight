using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    internal static class GraphExtensions
    {
        internal static IEnumerable<List<V>> GetRankList<G, V, E>(this IGraph<G, V, E> graph)
            where V : IVertexDataLayered, IEquatable<V>
        {
            return
                from node in graph.Vertices
                orderby node.Rank, node.Position
                group node by node.Rank
                    into rank
                select rank.ToList();
        }

        internal static V InsertControlPoint<G, V, E>(this IGraph<G, V, E> graph, IEdge<V, E> edge, V vertexData, E edgeData)
            where V : IEquatable<V>
        {
            graph.InsertVertex(edge, vertexData, edgeData);
            return vertexData;
        }

        internal static void RemoveControlPoint<G, V, E>(this IGraph<G, V, E> graph, V vertex)
            where V : IEquatable<V>
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