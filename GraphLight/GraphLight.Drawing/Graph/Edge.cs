namespace GraphLight.Graph
{
    public class Edge<TVertex, TEdge> : IEdge<TVertex, TEdge>
    {
        private TEdge _data;
        private IVertex<TVertex, TEdge> _dst, _src;
        private double _weight = 1;

        public Edge(TEdge data)
        {
            _data = data;
        }

        #region IEdge<TVertex,TEdge> Members

        public virtual TEdge Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public virtual double Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public virtual IVertex<TVertex, TEdge> Src
        {
            get { return _src; }
            set
            {
                var oldValue = _src;
                if (oldValue == value)
                    return;
                _src = value;
                if (oldValue != null)
                    oldValue.RegisterEdge(this);
                if (value != null)
                    value.RegisterEdge(this);
            }
        }

        public virtual IVertex<TVertex, TEdge> Dst
        {
            get { return _dst; }
            set
            {
                var oldValue = _dst;
                if (oldValue == value)
                    return;
                _dst = value;
                if (oldValue != null)
                    oldValue.RegisterEdge(this);
                if (value != null)
                    value.RegisterEdge(this);
            }
        }

        public virtual void Revert()
        {
            var tmp = Src;
            Src = Dst;
            Dst = tmp;
        }

        #endregion
    }
}