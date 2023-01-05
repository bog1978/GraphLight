namespace GraphLight.Model.LGML
{
    partial class Vertex
    {
        public override string ToString() =>
            string.IsNullOrWhiteSpace(Category)
                ? Id
                : $"{Category}: {Id}";
    }

    partial class Edge
    {
        public override string ToString() => 
            string.IsNullOrWhiteSpace(Category)
                ? $"{Src} -> {Dst}"
                : $"{Category}: {Src} -> {Dst}";
    }

    partial class VertexCategory
    {
        public override string ToString() => Id;
    }

    partial class EdgeCategory
    {
        public override string ToString() => Id;
    }
}