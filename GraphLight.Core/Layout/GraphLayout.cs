using GraphLight.Graph;

namespace GraphLight.Layout
{
    public interface INodeMeasure
    {
        void Measure(IVertex vertex);
    }

    public abstract class GraphLayout
    {
        public INodeMeasure NodeMeasure { get; set; }

        public IGraph Graph { get; set; }

        public abstract void Layout();
    }
}