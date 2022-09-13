using System;

namespace GraphLight.Graph
{
    public class BaseEdge<V, E> : BaseViewModel, IEdge<V, E>
    {
        private readonly E _data;
        private IVertex<V, E> _src;
        private IVertex<V, E> _dst;
        private double _weight = 1;
        private bool _isRevert;

        public BaseEdge(E data) => _data = data;

        public E Data => _data;

        public double Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }

        public bool IsRevert
        {
            get => _isRevert;
            private set => SetProperty(ref _isRevert, value);
        }

        public IVertex<V, E> Src
        {
            get => _src;
            set
            {
                if (_src == value)
                    return;
                var oldValue = _src;
                _src = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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

        public event EventHandler<EdgeChangedEventArgs<V, E>> EdgeChanged;

        public override string ToString() => $"{Src} -> {Dst}: {Data}";

        private void OnEdgeChanged(IVertex<V, E> oldVertex, IVertex<V, E> newVertex) =>
            EdgeChanged?.Invoke(this, new EdgeChangedEventArgs<V, E>(oldVertex, newVertex));
    }
}