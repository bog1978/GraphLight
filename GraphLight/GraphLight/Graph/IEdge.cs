namespace GraphLight.Graph
{
    public interface IEdge<TVertex, TEdge>
    {
        TEdge Data { get; set; }
        IVertex<TVertex, TEdge> Src { get; set; }
        IVertex<TVertex, TEdge> Dst { get; set; }
        double Weight { get; set; }
        void Revert();
    }
}