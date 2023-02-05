﻿using System;

namespace GraphLight.Model
{
    internal class GenericEdge<V, E> : IEdge<V, E>
    {
        public GenericEdge(IVertex<V> src, IVertex<V> dst, E data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Src = src ?? throw new ArgumentNullException(nameof(src));
            Dst = dst ?? throw new ArgumentNullException(nameof(dst));
        }

        public E Data { get; }

        public bool IsRevert { get; internal set; }

        public IVertex<V> Src { get; internal set; }

        public IVertex<V> Dst { get; internal set; }

        public override string ToString() => $"{Src} -> {Dst}: {Data}";

        public override int GetHashCode() => Data.GetHashCode();

        public override bool Equals(object obj) => 
            obj is GenericEdge<V, E> edge && Data.Equals(edge.Data);
    }
}