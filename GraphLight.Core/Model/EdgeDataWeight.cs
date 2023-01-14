namespace GraphLight.Model
{
    public class EdgeDataWeight : IEdgeDataWeight
    {
        private EdgeDataWeight(double weight) => Weight = weight;

        public double Weight { get; set; }
        
        public override string ToString() => $"{Weight:F3}";

        public static implicit operator EdgeDataWeight(double w) => new EdgeDataWeight(w);
    }
}