using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IVertex : IVertex<IVertexData, IEdgeData>
    {
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IEdge> InEdges { get; }
        IEnumerable<IEdge> OutEdges { get; }
        IEnumerable<IEdge> SelfEdges { get; }
    }
}