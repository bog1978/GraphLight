using System;

namespace GraphLight.Graph
{
    public interface IVertexData : IEquatable<IVertexData>, IComparable<IVertexData>
    {
        string Id { get; }
    }
}