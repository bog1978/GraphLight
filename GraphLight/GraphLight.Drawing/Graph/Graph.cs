using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    public class Graph<TVertex, TEdge> : BaseViewModel//, IGraph<TVertex, TEdge>
        where TEdge : new()
    {
        private readonly ICollection<Edge<TVertex, TEdge>> _edges;
        private readonly ICollection<object> _elements;
        private readonly ICollection<Vertex<TVertex, TEdge>> _verteces;

        private readonly IDictionary<TVertex, Vertex<TVertex, TEdge>> _vertexMap =
            new Dictionary<TVertex, Vertex<TVertex, TEdge>>();

        private double _width;
        private double _height;

        public Graph()
        {
            _elements = new ObservableCollection<object>();
            _verteces = new ObservableCollection<Vertex<TVertex, TEdge>>();
            _edges = new ObservableCollection<Edge<TVertex, TEdge>>();
        }

        public IEnumerable<object> Elements
        {
            get { return _elements; }
        }

        #region IGraph<TVertex,TEdge> Members

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value, "Width"); }
        }

        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value, "Height"); }
        }

        public IEnumerable<Vertex<TVertex, TEdge>> Verteces
        {
            get { return _verteces; }
        }

        public IEnumerable<Edge<TVertex, TEdge>> Edges
        {
            get { return _edges; }
        }

        public Vertex<TVertex, TEdge> this[TVertex key]
        {
            get { return _vertexMap[key]; }
        }

        public Edge<TVertex, TEdge> AddEdge(TVertex src, TVertex dst, TEdge data = default(TEdge))
        {
            var src1 = AddVertex(src);
            var dst1 = AddVertex(dst);
            var edge = CreateEdge(data);
            edge.Src = src1;
            edge.Dst = dst1;
            _elements.Add(edge);
            _edges.Add(edge);
            return edge;
        }

        public Vertex<TVertex, TEdge> AddVertex(TVertex data)
        {
            Vertex<TVertex, TEdge> vertex;
            if (!_vertexMap.TryGetValue(data, out vertex))
            {
                vertex = CreateVertex(data);
                _elements.Add(vertex);
                _verteces.Add(vertex);
                _vertexMap.Add(data, vertex);
            }
            return vertex;
        }

        public void RemoveEdge(Edge<TVertex, TEdge> edge)
        {
            _elements.Remove(edge);
            _edges.Remove(edge);
            edge.Src.UnregisterEdge(edge);
            edge.Dst.UnregisterEdge(edge);
        }

        public void RemoveVertex(Vertex<TVertex, TEdge> vertex)
        {
            if (vertex == null)
                return;
            var edges = vertex.Edges.ToArray();
            foreach (var edge in edges)
                RemoveEdge(edge);
            _elements.Remove(vertex);
            _verteces.Remove(vertex);
            _vertexMap.Remove(vertex.Data);
        }

        public Vertex<TVertex, TEdge> InsertVertex(Edge<TVertex, TEdge> edge, TVertex vertexData)
        {
            if (!Edges.Contains(edge))
                throw new Exception("Данное ребро не принадлежит графу");
            var newEdge = AddEdge(vertexData, edge.Dst.Data, new TEdge());
            edge.Dst = newEdge.Src;
            return newEdge.Src;
        }

        public Edge<TVertex, TEdge> AddEdge(Vertex<TVertex, TEdge> src, Vertex<TVertex, TEdge> dst,
            TEdge data = default(TEdge))
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst == null)
                throw new ArgumentNullException("dst");

            Vertex<TVertex, TEdge> existingSrc, existingDst;

            _vertexMap.TryGetValue(src.Data, out existingSrc);
            _vertexMap.TryGetValue(dst.Data, out existingDst);

            if (existingSrc != src)
                throw new ArgumentOutOfRangeException("src");

            if (existingDst != dst)
                throw new ArgumentOutOfRangeException("dst");

            var edge = CreateEdge(data);
            edge.Src = src;
            edge.Dst = dst;
            _elements.Add(edge);
            _edges.Add(edge);
            return edge;
        }

        #endregion

        protected virtual Edge<TVertex, TEdge> CreateEdge(TEdge data)
        {
            return new Edge<TVertex, TEdge>(data);
        }

        protected virtual Vertex<TVertex, TEdge> CreateVertex(TVertex data)
        {
            return new Vertex<TVertex, TEdge>(data);
        }
    }
}