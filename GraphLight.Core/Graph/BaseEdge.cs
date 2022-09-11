﻿using System;

namespace GraphLight.Graph
{
    public abstract class BaseEdge<V, E> : BaseViewModel, IEdge<V, E>
    {
        private IVertex<V, E> _src;
        private IVertex<V, E> _dst;
        private E _data;
        private double _weight = 1;

        public E Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public double Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
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

        public event EventHandler<EdgeChangedEventArgs<V, E>> EdgeChanged;

        public override string ToString() => $"{Src} -> {Dst}: {Data}";

        private void OnEdgeChanged(IVertex<V, E> oldVertex, IVertex<V, E> newVertex) =>
            EdgeChanged?.Invoke(this, new EdgeChangedEventArgs<V, E>(oldVertex, newVertex));
    }
}