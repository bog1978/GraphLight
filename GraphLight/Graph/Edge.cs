using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public class Edge : BaseEdge<IVertexData, IEdgeData>, IEdge
    {
        public Edge(IEdgeData data) : base(data)
        {
            var points = (ObservableCollection<Point2D>)data.Points;
            points.CollectionChanged += pointsCollectionChanged;
        }

        protected void pointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            this.HandlePointsCollectionChanged(e);
    }
}