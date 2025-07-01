using System;
using System.Collections.Generic;

namespace GraphLight.Model;

internal class GenericVertex<V, E>(V data)
    where V : IEquatable<V>
    where E : notnull
{
    public V Data => data;

    public List<IEdge<V, E>> Edges { get; } = new();

    public List<IEdge<V, E>> InEdges { get; } = new();

    public List<IEdge<V, E>> OutEdges { get; } = new();

    public List<IEdge<V, E>> SelfEdges { get; } = new();

    public override string? ToString() => Data.ToString();

    public override bool Equals(object? obj) =>
        obj is GenericVertex<V, E> other && Equals(Data, other.Data);

    public override int GetHashCode() =>
        Data.GetHashCode();
}