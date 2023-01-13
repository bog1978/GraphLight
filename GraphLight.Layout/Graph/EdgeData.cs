using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class EdgeData : CommonData, IEdgeData
    {
        public EdgeData(string? label = null, string? category = null) : base(label, category)
        {
            Points = new ObservableCollection<Point2D>();
            Weight = 1.0;
        }

        public IList<Point2D> Points { get; }
        public double Weight { get; set; }
    }
}