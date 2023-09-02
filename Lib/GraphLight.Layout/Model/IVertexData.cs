using System;

namespace GraphLight.Model
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