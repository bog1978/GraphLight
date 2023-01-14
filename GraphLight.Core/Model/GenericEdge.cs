using System;

namespace GraphLight.Model
{
    internal class GenericEdge<V, E> : IEdge<V, E>
    {
        private IVertex<V, E> _src;
        private IVertex<V, E> _dst;

        public GenericEdge(IVertex<V, E> src, IVertex<V, E> dst, E data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            _src = src ?? throw new ArgumentNullException(nameof(src));
            _dst = dst ?? throw new ArgumentNullException(nameof(dst));
            _src.RegisterEdge(this);
            _dst.RegisterEdge(this);
        }

        public E Data { get; }

        public bool IsRevert { get; private set; }

        public IVertex<V, E> Src
        {
            get => _src;
            set
            {
                if (_src == value)
                    return;
                var oldValue = _src;
                _src = value;
                OnEdgeChanged(oldValue, value);
            }
        }

        public IVertex<V, E> Dst
        {
            get => _dst;
            set
            {
                var oldValue = _dst;
                if (oldValue == value)
                    return;
                _dst = value;
                OnEdgeChanged(oldValue, value);
            }
        }

        public void Revert()
        {
            if (IsRevert)
                throw new Exception("Edge is already reverted.");
            var tmp = Src;
            Src = Dst;
            Dst = tmp;
            IsRevert = !IsRevert;
        }

        public override string ToString() => $"{Src} -> {Dst}: {Data}";

        private void OnEdgeChanged(IVertex<V, E>? oldVertex, IVertex<V, E>? newVertex)
        {
            oldVertex?.UnRegisterEdge(this);
            newVertex?.RegisterEdge(this);
        }

        public override int GetHashCode() => Data.GetHashCode();

        public override bool Equals(object obj) => 
            obj is GenericEdge<V, E> edge && Data.Equals(edge.Data);
    }
}