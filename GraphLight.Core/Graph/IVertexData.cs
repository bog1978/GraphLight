using System;

namespace GraphLight.Graph
{
    public interface IVertexData :
        IEquatable<IVertexData>,
        IComparable<IVertexData>,
        IVertexDataLayered,
        IVertexDataLocation
    {
        string Id { get; }
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
        int ZIndex { get; set; }
        string Category { get; set; }
        string Label { get; set; }
        string ShapeData { get; set; }
    }
}