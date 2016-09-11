namespace GraphLight.Graph
{
    public class Graph<TVertex, TEdge> : BaseGraph<TVertex, TEdge>
        where TVertex : new()
        where TEdge : new()
    {
        protected override TEdge CreateEdgeData()
        {
            return new TEdge();
        }

        protected override TVertex CreateVertexData()
        {
            return new TVertex();
        }
    }
}