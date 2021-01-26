namespace GraphLight.Graph
{

    public class GraphModel : BaseGraph<object, object>
    {
        protected override object CreateEdgeData() => new object();

        protected override object CreateVertexData() => new object();
    }
}