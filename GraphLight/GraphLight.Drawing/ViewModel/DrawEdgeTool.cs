using System.Linq;
using System.Windows.Input;
using GraphLight.Geometry;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.ViewModel
{
    public class DrawEdgeTool : GraphTool
    {
        private DrawingEdge _newEdge;
        private DrawingVertex _srcNode;

        public DrawEdgeTool(GraphViewModel viewModel)
            : base(viewModel)
        {
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = GetPoint(e);
            var node = GetOriginalDataContext<DrawingVertex>(e);
            if (_newEdge != null && node == _srcNode && _newEdge.Data.Points.Count <= 2)
            {
                Cancel();
                Model.SelectedNode = node;
            }
            else if (node == null && _newEdge != null)
                addNewControlPoint(point);
            else if (node != null && _newEdge != null)
                endDrawEdge(point, node);
        }

        public override void HandleLButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = GetPoint(e);
            var node = GetOriginalDataContext<DrawingVertex>(e);
            if (node != null && _newEdge == null)
                beginDrawEdge(point, node);
        }

        private void beginDrawEdge(Point2D point, DrawingVertex srcNode)
        {
            var p = srcNode.Data.CenterPoint();
            _srcNode = srcNode;
            _newEdge = (DrawingEdge) Model.Graph.AddEdge(srcNode.Data, srcNode.Data, new EdgeAttrs());
            _newEdge.Data.Points.Add(p);
            _newEdge.RaisePointsChanged();
            IsInProgress = true;
        }

        private void addNewControlPoint(Point2D point)
        {
            _newEdge.Data.Points.Add(point);
            if (_newEdge.Data.Points.Count == 2)
            {
                _newEdge.Data.IsSelected = true;
                _newEdge.UpdateSrcPort();
                _newEdge.FixDraggablePoints(point);
            }
            _newEdge.RaisePointsChanged();
        }

        private void endDrawEdge(Point2D point, DrawingVertex dstNode)
        {
            //_newEdge.Points.Add(point);
            _newEdge.Dst = dstNode;
            if (_newEdge.Data.Points.Count == 2)
                _newEdge.UpdateSrcPort();
            _newEdge.Data.IsSelected = false;
            _newEdge.UpdateDstPort();
            _newEdge.FixDraggablePoints(_newEdge.Data.Points.Last());
            _newEdge.RaisePointsChanged();
            _newEdge = null;
            IsInProgress = false;
        }

        public override void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_newEdge == null)
                return;

            var node = GetOriginalDataContext<DrawingVertex>(e);
            if (node != null)
            {
                if (node == _srcNode)
                    return;

                var p1 = _newEdge.Data.Points[_newEdge.Data.Points.Count - 1];
                var p2 = _newEdge.Data.Points[_newEdge.Data.Points.Count - 2];
                var p = node.Data.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                    _newEdge.FixDraggablePoints(p1);
                }
                _newEdge.RaisePointsChanged();
                return;
            }

            var vm = GetOriginalDataContext<GraphViewModel>(e);
            if (vm != Model)
                return;

            var point = GetPoint(e);
            if (_newEdge.Data.Points.Count == 1)
                addNewControlPoint(point);
            else
            {
                var lastPoint = _newEdge.Data.Points.Last();
                lastPoint.X = point.X;
                lastPoint.Y = point.Y;
                if (_newEdge.Data.Points.Count == 2)
                    _newEdge.UpdateSrcPort();
                _newEdge.FixDraggablePoints(lastPoint);
            }
            _newEdge.RaisePointsChanged();
        }

        public override void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Cancel();
        }

        public override void Cancel()
        {
            if (_newEdge == null)
                return;
            Model.Graph.RemoveEdge(_newEdge);
            _newEdge.Src = null;
            _newEdge.Dst = null;
            _newEdge = null;
            _srcNode = null;
            IsInProgress = false;
        }

        #endregion
    }
}