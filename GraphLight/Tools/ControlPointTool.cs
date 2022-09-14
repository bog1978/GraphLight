using System.Linq;
using System.Windows;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Drawing;
using GraphLight.Geometry;

namespace GraphLight.Tools
{
    public class ControlPointTool : GraphTool
    {
        public ControlPointTool(GraphControl viewModel) : base(viewModel) { }

        public override void HandleLButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = GetOriginalDataContext<Point2D>(e);
            if (point == null)
                return;

            var points = Model.SelectedEdge.Points;
            if (points.First() == point || points.Last() == point)
                return;

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (points.Contains(point))
                    points.Remove(point);
                return;
            }
            if (!points.Contains(point))
            {
                var draggablePoints = Model.SelectedEdge.Data.DraggablePoints;
                var i = draggablePoints.IndexOf(point);
                points.Insert((i + 1) / 2, point);
            }
        }

        public override bool HandleDragQuery(IDragDropOptions options)
        {
            var point = options.Source.DataContext as Point2D;
            if (point == null || Model.SelectedEdge == null)
                return false;
            var points = Model.SelectedEdge.Points;
            if (points.First() == point || points.Last() == point)
                return false;
            options.Payload = new Point(point.X, point.Y);
            return true;
        }

        public override void HandleDropInfo(IDragDropOptions options)
        {
            var point = options.Source.DataContext as Point2D;
            if (point == null)
                return;
            var p = (Point)options.Payload;
            if (Model.SelectedEdge != null)
            {
                using (Model.SelectedEdge.DeferRefresh())
                {
                    point.X = p.X + options.DeltaX;
                    point.Y = p.Y + options.DeltaY;
                    Model.SelectedEdge.UpdatePoint(point);
                }
            }
        }
    }
}