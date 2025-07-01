using GraphLight.Model;

namespace GraphLight.Algorithm
{
    public abstract class GraphLayout(INodeMeasure nodeMeasure, IGraph<IGraphData, IVertexData, IEdgeData> graph)
    {
        public INodeMeasure NodeMeasure => nodeMeasure;

        public IGraph<IGraphData, IVertexData, IEdgeData> Graph => graph;

        public abstract void Layout();
    }
}