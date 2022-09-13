namespace GraphLight.Graph
{
    public class GenericGraph<TVertex, TEdge> : BaseGraph<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override IEdge<TVertex, TEdge> CreateEdge(TEdge data) => new BaseEdge<TVertex, TEdge>(data);

        protected override IVertex<TVertex, TEdge> CreateVertex(TVertex data) => new BaseVertex<TVertex, TEdge>(data);
    }
}