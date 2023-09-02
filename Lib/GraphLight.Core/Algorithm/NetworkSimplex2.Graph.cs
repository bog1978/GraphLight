using System;
using GraphLight.Model;

namespace GraphLight.Algorithm
{
    partial class NetworkSimplex2
    {
        public class GraphData
        {
            public VertexData Root;
        }

        public class EdgeData
        {
            public readonly int MinLength;
            public readonly int Weight;
            public int CutValue;
            public bool IsTree;

            public EdgeData(int weight, int minLength)
            {
                Weight = weight;
                MinLength = minLength;
            }
        }

        public class VertexData : IEquatable<VertexData>
        {
            internal VertexColor Color = VertexColor.White;
            public int Lim = int.MaxValue;
            public int Low = int.MaxValue;
            public IEdge<VertexData, EdgeData>? ParentEdge;
            public VertexData? ParentVertex;
            internal int ScanIndex;
            //internal int TreeEdgeCount;
            public int Value;
            public int Priority = int.MaxValue;
            private readonly int _n;
            private static int _counter;

            public VertexData(object original)
            {
                _n = _counter++;
                Original = original;
            }

            public bool Equals(VertexData? other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return _n == other._n;
            }

            public object Original { get; }

            public override bool Equals(object? obj) => Equals(obj as VertexData);

            public override int GetHashCode() => _n;

            public override string ToString() => $"{Original}: {Value}";
        }
    }
}