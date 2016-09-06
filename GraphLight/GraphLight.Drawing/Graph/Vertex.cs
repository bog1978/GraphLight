using System.Collections.Generic;
using GraphLight.Collections;

namespace GraphLight.Graph
{
    public class Vertex<TVertex, TEdge> : IVertex<TVertex, TEdge>
    {
        #region Private fields

        private readonly ICollection<IEdge<TVertex, TEdge>> _edges;
        private readonly ICollection<IEdge<TVertex, TEdge>> _inEdges;
        private readonly ICollection<IEdge<TVertex, TEdge>> _outEdges;
        private readonly ICollection<IEdge<TVertex, TEdge>> _selfEdges;
        private TVertex _data;
        private static int _cnt;

        #endregion

        #region Constructors

        public Vertex(TVertex data)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            _edges = CreateCollection<IEdge<TVertex, TEdge>>();
            _inEdges = CreateCollection<IEdge<TVertex, TEdge>>();
            _outEdges = CreateCollection<IEdge<TVertex, TEdge>>();
            _selfEdges = CreateCollection<IEdge<TVertex, TEdge>>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            _data = data;
            Id = string.Format("V{0}", _cnt++);
        }

        #endregion

        #region IVertex<TVertex,TEdge> Members

        public virtual TVertex Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public virtual string Id { get; private set; }

        public IEnumerable<IEdge<TVertex, TEdge>> Edges
        {
            get { return _edges; }
        }

        public IEnumerable<IEdge<TVertex, TEdge>> InEdges
        {
            get { return _inEdges; }
        }

        public IEnumerable<IEdge<TVertex, TEdge>> OutEdges
        {
            get { return _outEdges; }
        }

        public IEnumerable<IEdge<TVertex, TEdge>> SelfEdges
        {
            get { return _selfEdges; }
        }

        void IVertex<TVertex, TEdge>.RegisterEdge(IEdge<TVertex, TEdge> edge)
        {
            var collection = edge.Src == this && edge.Dst == this
                ? _selfEdges
                : edge.Src == this
                    ? _outEdges
                    : edge.Dst == this
                        ? _inEdges
                        : null;

            if (collection == null)
            {
                _edges.Remove(edge);
                _selfEdges.Remove(edge);
                _inEdges.Remove(edge);
                _outEdges.Remove(edge);
            }
            else if (_edges.Contains(edge))
            {
                if (collection.Contains(edge))
                    return;
                _selfEdges.Remove(edge);
                _inEdges.Remove(edge);
                _outEdges.Remove(edge);
                collection.Add(edge);
            }
            else
            {
                _edges.Add(edge);
                collection.Add(edge);
            }
        }

        void IVertex<TVertex, TEdge>.UnregisterEdge(IEdge<TVertex, TEdge> edge)
        {
            _edges.Remove(edge);
            _selfEdges.Remove(edge);
            _inEdges.Remove(edge);
            _outEdges.Remove(edge);
        }

        int IBinaryHeapItem<double>.HeapIndex { get; set; }

        double IBinaryHeapItem<double>.HeapKey { get; set; }

        #endregion

        protected virtual ICollection<T> CreateCollection<T>()
        {
            return new List<T>();
        }
    }
}