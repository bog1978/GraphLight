using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GraphLight.Geometry;

namespace GraphLight.ViewModel
{
    public class ControlPointTool : GraphTool
    {
        public ControlPointTool(GraphViewModel viewModel)
            : base(viewModel)
        {
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        public override void HandleLButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = GetOriginalDataContext<Point2D>(e);
            if (point == null)
                return;

            var points = Model.SelectedEdge.Data.Points;
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
                var draggablePoints = (IList<Point2D>)Model.SelectedEdge.DraggablePoints;
                var i = draggablePoints.IndexOf(point);
                points.Insert((i + 1) / 2, point);
            }
        }

        public override void HandleMouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void HandleKeyUp(object sender, KeyEventArgs e)
        {
        }

        public override void Cancel() { }

        #endregion
    }
}