namespace GraphLight.Graph
{
    public interface IElement<out TData>
    {
        TData Data { get; }
    }
}