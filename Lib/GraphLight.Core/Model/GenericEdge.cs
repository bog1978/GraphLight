using System;

namespace GraphLight.Model;

internal class GenericEdge<V, E>(V src, V dst, E data) : IEdge<V, E>
    where V : IEquatable<V>
    where E : notnull
{
    public E Data => data;

    public bool IsRevert { get; internal set; }

    public V Src { get; internal set; } = src;

    public V Dst { get; internal set; } = dst;

    public override string ToString() => $"{Src} -> {Dst}: {Data}";
}