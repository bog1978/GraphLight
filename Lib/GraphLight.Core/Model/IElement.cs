namespace GraphLight.Model;

public interface IElement<out TData>
    where TData : notnull
{
    TData Data { get; }
}