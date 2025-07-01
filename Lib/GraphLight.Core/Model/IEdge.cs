using System;

namespace GraphLight.Model;

public interface IEdge<out V, out E> : IElement<E>
    where V : IEquatable<V>
    where E : notnull
{
    bool IsRevert { get; }
    V Dst { get; }
    V Src { get; }
}