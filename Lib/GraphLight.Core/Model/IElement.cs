namespace GraphLight.Model
{
    public interface IElement<out TData>
    {
        TData Data { get; }
    }
}