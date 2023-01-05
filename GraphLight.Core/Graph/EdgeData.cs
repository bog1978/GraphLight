using System.Collections.Generic;
using System.Collections.ObjectModel;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class EdgeData : CommonData, IEdgeData
    {
        public EdgeData()
        {
            Points = new ObservableCollection<Point2D>();
        }

        public IList<Point2D> Points { get; }
    }
}