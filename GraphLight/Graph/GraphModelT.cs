namespace GraphLight.Graph
{
    public class GraphModel<TVertex, TEdge> : BaseGraph<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override TEdge CreateEdgeData() => new TEdge();

        protected override TVertex CreateVertexData() => new TVertex();
    }
}