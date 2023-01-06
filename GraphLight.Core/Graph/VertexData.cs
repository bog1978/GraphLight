using System;

namespace GraphLight.Graph
{
    public class VertexData : CommonData, IVertexData
    {
        public VertexData(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Label = id;
            Shape = VertexShape.Ellipse;
            Margin = 5;
        }

        public string Id { get; }

        bool IVertexDataLocation.IsTmp { get; set; }

        int IVertexDataLayered.Rank { get; set; }

        int IVertexDataLayered.Position { get; set; }

        double IVertexDataLocation.CenterX { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Left { get; set; }

        public double Top { get; set; }

        public double Right => Left + Width;

        public double Bottom => Top + Height;

        public VertexShape Shape { get; set; }

        public double Margin { get; set; }

        public bool Equals(IVertexData other) => other?.Id == Id;

        public int CompareTo(IVertexData other) => StringComparer.Ordinal.Compare(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as IVertexData);

        public override string ToString() => Id;
    }
}