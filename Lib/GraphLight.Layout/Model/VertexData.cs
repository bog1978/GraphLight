using System;
using GraphLight.Geometry;

namespace GraphLight.Model
{
    public class VertexData(string id, string? category = null) : CommonData(id, category), IVertexData
    {
        public string Id { get; } = id ?? throw new ArgumentNullException(nameof(id));

        bool IVertexDataLocation.IsTmp { get; set; }

        int IVertexDataLayered.Rank { get; set; }

        int IVertexDataLayered.Position { get; set; }

        public Rect2D Rect { get; } = new();

        public VertexShape Shape { get; set; } = VertexShape.Ellipse;

        public double Margin { get; set; } = 5;

        public bool Equals(IVertexData? other) => other?.Id == Id;

        public int CompareTo(IVertexData? other) => StringComparer.Ordinal.Compare(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object? obj) => Equals(obj as IVertexData);

        public override string ToString() => Id;

        public static implicit operator VertexData(string id) => new(id);
    }
}