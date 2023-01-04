using System;

namespace GraphLight.Graph
{
    public interface IVertexData :
        IEquatable<IVertexData>,
        IComparable<IVertexData>,
        IVertexDataLayered,
        IVertexDataLocation,
        ICommonData
    {
        string Id { get; }
        string Label { get; set; }
        VertexShape Shape { get; set; }
    }
}