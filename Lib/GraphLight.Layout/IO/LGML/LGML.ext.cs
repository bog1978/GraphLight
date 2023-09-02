namespace GraphLight.IO.LGML
{
    partial class LgmlVertex
    {
        public override string ToString() =>
            string.IsNullOrWhiteSpace(Category)
                ? Id
                : $"{Category}: {Id}";
    }

    partial class LgmlEdge
    {
        public override string ToString() => 
            string.IsNullOrWhiteSpace(Category)
                ? $"{Src} -> {Dst}"
                : $"{Category}: {Src} -> {Dst}";
    }

    partial class LgmlVertexCategory
    {
        public override string ToString() => Id;
    }

    partial class LgmlEdgeCategory
    {
        public override string ToString() => Id;
    }
}