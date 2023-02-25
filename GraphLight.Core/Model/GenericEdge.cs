using System;

namespace GraphLight.Model
{
    internal class GenericEdge<V, E> : IEdge<V, E>
        where V : IEquatable<V>
    {
        public GenericEdge(V src, V dst, E data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Src = src ?? throw new ArgumentNullException(nameof(src));
            Dst = dst ?? throw new ArgumentNullException(nameof(dst));
        }

        public E Data { get; }

        public bool IsRevert { get; internal set; }

        public V Src { get; internal set; }

        public V Dst { get; internal set; }

        public override string ToString() => $"{Src} -> {Dst}: {Data}";
    }
}