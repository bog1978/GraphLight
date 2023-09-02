using System;

namespace GraphLight.Model
{
    public interface IEdge<out V, out E> : IElement<E>
    where V : IEquatable<V>
    {
        bool IsRevert { get; }
        V Dst { get; }
        V Src { get; }
    }
}