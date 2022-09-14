using System.Collections.Generic;
using System.Linq;

namespace GraphLight.Graph
{
    public class Vertex : BaseVertex<IVertexData, IEdgeData>, IVertex
    {
        public Vertex(IVertexData data) : base(data)
        {
        }

        IEnumerable<IEdge> IVertex.Edges => Edges.Cast<IEdge>();

        IEnumerable<IEdge> IVertex.InEdges => InEdges.Cast<IEdge>();

        IEnumerable<IEdge> IVertex.OutEdges => OutEdges.Cast<IEdge>();

        IEnumerable<IEdge> IVertex.SelfEdges => SelfEdges.Cast<IEdge>();
    }
}