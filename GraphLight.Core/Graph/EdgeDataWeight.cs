namespace GraphLight.Graph
{
    public class EdgeDataWeight : IEdgeDataWeight
    {
        private EdgeDataWeight(double weight) => Weight = weight;
        public double Weight { get; set; }
        public static implicit operator EdgeDataWeight(double w) => new EdgeDataWeight(w);
    }
}