namespace GraphLight.Graph
{
    public class DrawingGraph : Graph<VertexAttrs, EdgeAttrs>
    {
        protected override Edge<VertexAttrs, EdgeAttrs> CreateEdge(EdgeAttrs data)
        {
            return new DrawingEdge(data);
        }

        protected override Vertex<VertexAttrs, EdgeAttrs> CreateVertex(VertexAttrs data)
        {
            return new DrawingVertex(data);
        }
    }
}