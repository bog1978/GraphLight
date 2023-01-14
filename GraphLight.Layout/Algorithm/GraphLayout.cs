using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public abstract class GraphLayout
    {
        public INodeMeasure NodeMeasure { get; set; }

        public IGraph<IGraphData, IVertexData, IEdgeData> Graph { get; set; }

        public abstract void Layout();
    }
}