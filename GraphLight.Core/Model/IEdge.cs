namespace GraphLight.Model
{
    public interface IEdge<out V, out E> : IElement<E>
    {
        bool IsRevert { get; }
        V Dst { get; }
        V Src { get; }
    }
}