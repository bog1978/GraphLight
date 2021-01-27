using System.Collections.Generic;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public interface IVertex : IElement, IBinaryHeapItem<double>
    {
        int Rank { get; set; }
        int Position { get; set; }
        bool IsTmp { get; set; }
        string Label { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
        double CenterX { get; set; }
        string ShapeData { get; set; }
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IEdge> InEdges { get; }
        IEnumerable<IEdge> OutEdges { get; }
        IEnumerable<IEdge> SelfEdges { get; }
        void Update();
    }

    public interface IVertex<V,E> : IVertex
    {
        new IEnumerable<IEdge<V, E>> Edges { get; }
        new IEnumerable<IEdge<V, E>> InEdges { get; }
        new IEnumerable<IEdge<V, E>> OutEdges { get; }
        new IEnumerable<IEdge<V, E>> SelfEdges { get; }
        new V Data { get; }
    }
}