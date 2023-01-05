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
        VertexShape Shape { get; set; }
        string Background { get; set; }
        string Foreground { get; set; }
    }
}