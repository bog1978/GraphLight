using System;
using System.Diagnostics;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Algorithm
{
    partial class NetworkSimplex
    {
        #region Nested type: Edge

        //[DebuggerDisplay("{Src.Id} -> {Dst.Id}")]
        protected class Edge
        {
            private static int _counter;
            public readonly Vertex Dst;
            public readonly int MinLength;
            public readonly Vertex Src;
            public readonly int Weight;
            public int Cnt = _counter++;
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

            public int Lenght
            {
                get { return Math.Abs(Dst.Value - Src.Value); }
            }

            public int Slack()
            {
                return (Dst.Value - Src.Value) - MinLength;
            }
        }

        #endregion

        #region Nested type: EdgeType

        protected enum EdgeType
        {
            Unknown,
            HeadToTail,
            TailToHead,
        }

        #endregion

        #region Nested type: Graph

        protected class Graph
        {
            public Edge[] Edges;
            public Vertex[] Verteces;
            public Vertex Root;
        }

        #endregion

        #region Nested type: Vertex

        //[DebuggerDisplay("{Id}")]
        protected class Vertex : IBinaryHeapItem<int>
        {
            public VertexColor Color = VertexColor.White;
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
            private int _heapKey = int.MaxValue;

            #region IBinaryHeapItem<int> Members

            public int HeapIndex { get; set; }

            public int HeapKey
            {
                get { return _heapKey; }
                set { _heapKey = value; }
            }

            #endregion
        }

        #endregion
    }
}