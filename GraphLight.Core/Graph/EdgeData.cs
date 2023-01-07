using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class EdgeData : CommonData, IEdgeData
    {
        public EdgeData(string? label, string? category) : base(label, category)
        {
            Points = new ObservableCollection<Point2D>();
        }

        public IList<Point2D> Points { get; }
    }
}