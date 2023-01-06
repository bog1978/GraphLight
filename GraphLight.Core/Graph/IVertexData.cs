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
        double Margin { get; set; }
    }
}