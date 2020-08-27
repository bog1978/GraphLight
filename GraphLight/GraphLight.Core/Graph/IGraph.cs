using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IGraph
    {
        IEnumerable<IElement> Elements { get; }
        double Width { get; set; }
        double Height { get; set; }
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IVertex> Vertices { get; }
        IVertex this[object key] { get; }
        IVertex InsertVertex(IEdge edge);
        IVertex AddVertex();
        IVertex AddVertex(object data);
        void RemoveEdge(IEdge edge);
        void RemoveVertex(IVertex vertex);
        IEdge AddEdge(object src, object dst);
        IEdge AddEdge(object src, object dst, object data);
    }
}