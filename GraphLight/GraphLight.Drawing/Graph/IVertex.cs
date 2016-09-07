using System.Collections.Generic;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public interface IVertex : IBinaryHeapItem<double>
    {
        int Rank { get; set; }
        int Position { get; set; }
        bool IsTmp { get; set; }
        string Category { get; set; }
        string Label { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
        double CenterX { get; set; }
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
    }

    public interface IVertex<TVertex, TEdge> : IVertex
    {
        TVertex Data { get; set; }
        IEnumerable<IEdge<TVertex, TEdge>> Edges { get; }
        IEnumerable<IEdge<TVertex, TEdge>> InEdges { get; }
        IEnumerable<IEdge<TVertex, TEdge>> OutEdges { get; }
        IEnumerable<IEdge<TVertex, TEdge>> SelfEdges { get; }
        void RegisterEdge(IEdge<TVertex, TEdge> edge);
        void UnregisterEdge(IEdge<TVertex, TEdge> edge);
    }
}