using System.Collections.Generic;

namespace GraphLight.Graph
{
    public interface IGraph<TVertex, TEdge>
    {
        double Width { get; set; }
        double Height { get; set; }
        IEnumerable<Vertex<TVertex, TEdge>> Verteces { get; }
        IEnumerable<Edge<TVertex, TEdge>> Edges { get; }
        Vertex<TVertex, TEdge> this[TVertex key] { get; }
        Edge<TVertex, TEdge> AddEdge(TVertex src, TVertex dst, TEdge data);
        Edge<TVertex, TEdge> AddEdge(Vertex<TVertex, TEdge> src, Vertex<TVertex, TEdge> dst, TEdge data);
        Vertex<TVertex, TEdge> AddVertex(TVertex data);
        void RemoveEdge(Edge<TVertex, TEdge> edge);
        void RemoveVertex(Vertex<TVertex, TEdge> vertex);
        Vertex<TVertex, TEdge> InsertVertex(Edge<TVertex, TEdge> edge, TVertex vertexData);
    }
}
