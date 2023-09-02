using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Geometry;

namespace GraphLight.Model
{
    public class EdgeData : CommonData, IEdgeData
    {
        public EdgeData(string? label = null, string? category = null, double weight = 1.0) : base(label, category)
        {
            Points = new ObservableCollection<Point2D>();
            Weight = weight;
        }

        public IList<Point2D> Points { get; }
        public double Weight { get; set; }
    }
}