namespace GraphLight.Prefomance
{
    public interface IPerfomanceTest
    {
        void Warmup();
        void Test();
        int IterCount { get; set; }
    }
}