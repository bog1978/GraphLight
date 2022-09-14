using System.Collections.Specialized;
using GraphLight.Geometry;
using GraphLight.Layout;

namespace GraphLight.Graph
{
    public static class EdgeExtensions
    {
        public static void UpdatePoint<V, E>(this IEdge<V,E> edge, Point2D data)
            where V : IVertexData
            where E : IEdgeData
        {
            var points = edge.Data.Points;
            var i = points.IndexOf(data);
            if (i == 1 && !edge.IsRevert)
                edge.UpdateSrcPort();
            else if (i == 1 && edge.IsRevert)
                edge.UpdateDstPort();
            if (i == points.Count - 2 && !edge.IsRevert)
                edge.UpdateDstPort();
            else if (i == points.Count - 2 && edge.IsRevert)
                edge.UpdateSrcPort();
            edge.Data.FixDraggablePoints(data);
        }

        public static void UpdateSrcPort<V, E>(this IEdge<V, E> edge)
            where V : IVertexData
            where E : IEdgeData
        {
            var points = edge.Data.Points;
            if (points.Count < 2)
                return;
            if (edge.IsRevert)
            {
                var p1 = points[points.Count - 1];
                var p2 = points[points.Count - 2];
                var p = edge.Src.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
            else
            {
                var p1 = points[0];
                var p2 = points[1];
                var p = edge.Src.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
        }

        public static void UpdateDstPort<V, E>(this IEdge<V, E> edge)
            where V : IVertexData
            where E : IEdgeData
        {
            var points = edge.Data.Points;
            if (points.Count < 2)
                return;
            if (edge.IsRevert)
            {
                var p1 = points[0];
                var p2 = points[1];
                var p = edge.Dst.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
            else
            {
                var p1 = points[points.Count - 1];
                var p2 = points[points.Count - 2];
                var p = edge.Dst.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                }
            }
        }

        public static void HandlePointsCollectionChanged<V, E>(this IEdge<V, E> edge, NotifyCollectionChangedEventArgs e)
            where V : IVertexData
            where E : IEdgeData
        {
            var Data = edge.Data;
            var Points = edge.Data.Points;

            Point2D p1, p2, p3, p4, p5;
            int i;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    i = 2 * e.NewStartingIndex;
                    if (e.NewStartingIndex == 0)
                    {
                        // First point. Do nothing.
                        Data.DraggablePoints.Insert(0, Points[0]);
                    }
                    else if (e.NewStartingIndex == Points.Count - 1)
                    {
                        // Last point.
                        p1 = Data.DraggablePoints[i - 2];
                        p3 = Points[e.NewStartingIndex];
                        p2 = p1 + (p3 - p1) / 2;
                        Data.DraggablePoints.Add(p2);
                        Data.DraggablePoints.Add(p3);
                    }
                    else
                    {
                        // Middle point
                        p1 = Data.DraggablePoints[i - 2];
                        p3 = Data.DraggablePoints[i - 1];
                        p5 = Data.DraggablePoints[i];
                        p2 = p1 + (p3 - p1) / 2;
                        p4 = p3 + (p5 - p3) / 2;
                        Data.DraggablePoints.Insert(i - 1, p2);
                        Data.DraggablePoints.Insert(i + 1, p4);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    i = 2 * e.OldStartingIndex;
                    // Сначала фиксим порты, чтобы правильно вычислить координаты средней точки.
                    edge.UpdateSrcPort();
                    edge.UpdateDstPort();
                    p1 = Data.DraggablePoints[i - 2];
                    p2 = Data.DraggablePoints[i - 1];
                    p3 = Data.DraggablePoints[i];
                    p4 = Data.DraggablePoints[i + 1];
                    p5 = Data.DraggablePoints[i + 2];
                    Data.DraggablePoints.Remove(p2);
                    Data.DraggablePoints.Remove(p4);
                    p3.X = (p1.X + p5.X) / 2;
                    p3.Y = (p1.Y + p5.Y) / 2;
                    edge.Data.RaisePointsChanged();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Data.DraggablePoints.Clear();
                    foreach (var point in Points)
                        Data.DraggablePoints.Add(point);
                    break;
            }
        }
    }
}