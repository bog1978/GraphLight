using System;

namespace GraphLight.Algorithm
{
    partial class NetworkSimplex
    {
        #region Nested type: Edge

        protected class Edge
        {
            public readonly Vertex Dst;
            public readonly int MinLength;
            public readonly Vertex Src;
            public readonly int Weight;
            public int CutValue;
            internal bool IsTree;
            public bool IsVisited;

            public Edge(Vertex src, Vertex dst, int weight, int minLength)
            {
                Src = src;
                Dst = dst;
                Weight = weight;
                MinLength = minLength;
            }

            public int Length => Math.Abs(Dst.Value - Src.Value);

            public int Slack() => Dst.Value - Src.Value - MinLength;
        }

        #endregion

        #region Nested type: Graph

        protected class Graph
        {
            public Edge[] Edges;
            public Vertex[] Vertices;
            public Vertex Root;
        }

        #endregion

        #region Nested type: Vertex

        protected class Vertex
        {
            public Vertex(object original) => Original = original;

            public object Original { get; }
            internal VertexColor Color = VertexColor.White;
            public Edge[] Edges;
            public string Id;
            public Edge[] InEdges;
            public int Lim = int.MaxValue;
            public int Low = int.MaxValue;
            public Edge[] OutEdges;
            public Edge ParentEdge;
            public Vertex ParentVertex;
            internal int ScanIndex;
            internal int TreeEdgeCount;
            public int Value;
            public int Priority { get; set; } = int.MaxValue;

            public override string ToString() => $"{Original}: {Value}";
        }

        #endregion
    }
}