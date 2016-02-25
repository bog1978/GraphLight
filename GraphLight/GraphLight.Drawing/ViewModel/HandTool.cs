using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Graph;
using GraphLight.Geometry;

namespace GraphLight.ViewModel
{
    public class HandTool : GraphTool
    {
        private readonly GraphViewModel _viewModel;

        public HandTool(GraphViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = GetOriginalDataContext<Point2D>(e);
            if (point != null)
                return;
            var node = GetOriginalDataContext<DrawingVertex>(e);
            var edge = GetOriginalDataContext<DrawingEdge>(e);
            if (_viewModel.SelectedNode != node)
                _viewModel.SelectedNode = node;
            if (_viewModel.SelectedEdge != edge)
                _viewModel.SelectedEdge = edge;
        }

        public override void HandleLButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ReferenceEquals(sender, e.OriginalSource))
            {
                _viewModel.SelectedEdge = null;
                _viewModel.SelectedNode = null;
                return;
            }
            var point = GetOriginalDataContext<Point2D>(e);
            if (point != null)
            {
                var points = _viewModel.SelectedEdge.Data.Points;
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
                    var draggablePoints = (IList<Point2D>)_viewModel.SelectedEdge.DraggablePoints;
                    var i = draggablePoints.IndexOf(point);
                    points.Insert((i + 1) / 2, point);
                }
            }
        }

        private object _last;
        public override void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_last == e.OriginalSource)
                return;
            if (_last != null)
                setHighlight(_last, false);
            setHighlight(e.OriginalSource, true);
            _last = e.OriginalSource;
        }

        public override void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (_viewModel.SelectedEdge != null)
                {
                    _viewModel.Graph.RemoveEdge(_viewModel.SelectedEdge);
                    _viewModel.SelectedEdge = null;
                }
                if (_viewModel.SelectedNode != null)
                {
                    _viewModel.Graph.RemoveVertex(_viewModel.SelectedNode);
                    _viewModel.SelectedNode = null;
                }
            }
        }

        public override void Cancel() { }

        #endregion

        private void setHighlight(object obj, bool isHighlighted)
        {
            var node = obj.GetDataContext<DrawingVertex>();
            var edge = obj.GetDataContext<DrawingEdge>();
            if (node != null)
                _viewModel.Highlight(node, isHighlighted);
            if (edge != null)
                _viewModel.Highlight(edge, isHighlighted);
        }
    }
}