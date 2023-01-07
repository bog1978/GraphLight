using System;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class VertexData : CommonData, IVertexData
    {
        public VertexData(string id, string? label, string? category) : base(id ?? label, category)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Shape = VertexShape.Ellipse;
            Margin = 5;
            Rect = new Rect2D();
        }

        public string Id { get; }

        bool IVertexDataLocation.IsTmp { get; set; }

        int IVertexDataLayered.Rank { get; set; }

        int IVertexDataLayered.Position { get; set; }

        public Rect2D Rect { get; }

        public VertexShape Shape { get; set; }

        public double Margin { get; set; }

        public bool Equals(IVertexData other) => other?.Id == Id;

        public int CompareTo(IVertexData other) => StringComparer.Ordinal.Compare(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as IVertexData);

        public override string ToString() => Id;
    }
}