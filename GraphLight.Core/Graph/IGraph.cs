using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IGraph : IGraph<IVertexData, IEdgeData>
    {
        double Width { get; set; }
        double Height { get; set; }
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IVertex> Vertices { get; }
    }
}