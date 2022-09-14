using System.Linq;
using System.Windows.Input;
using GraphLight.Drawing;
using GraphLight.Geometry;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Tools
{
    public class DrawEdgeTool : GraphTool
    {
        private IEdge _newEdge;
        private IVertex _srcNode;

        public DrawEdgeTool(GraphControl viewModel)
            : base(viewModel)
        {
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = GetPoint(e);
            var node = GetOriginalDataContext<IVertex>(e);
            if (_newEdge != null && node == _srcNode && _newEdge.Points.Count <= 2)
            {
                Cancel();
                Model.SelectedElement = node;
            }
            else if (node == null && _newEdge != null)
                addNewControlPoint(point);
            else if (node != null && _newEdge != null)
                endDrawEdge(point, node);
        }

        public override void HandleLButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = GetPoint(e);
            var node = GetOriginalDataContext<IVertex>(e);
            if (node != null && _newEdge == null)
                beginDrawEdge(point, node);
        }

        private void beginDrawEdge(Point2D point, IVertex srcNode)
        {
            var p = srcNode.CenterPoint();
            _srcNode = srcNode;
            _newEdge = (IEdge)Model.Graph.AddEdge(srcNode.Data, srcNode.Data, null);
            _newEdge.Points.Add(p);
            _newEdge.RaisePointsChanged();
            IsInProgress = true;
        }

        private void addNewControlPoint(Point2D point)
        {
            _newEdge.Points.Add(point);
            if (_newEdge.Points.Count == 2)
            {
                _newEdge.Data.IsSelected = true;
                _newEdge.UpdateSrcPort();
                _newEdge.Data.FixDraggablePoints(point);
            }
            _newEdge.RaisePointsChanged();
        }

        private void endDrawEdge(Point2D point, IVertex dstNode)
        {
            //_newEdge.Points.Add(point);
            _newEdge.Dst = dstNode;
            if (_newEdge.Points.Count == 2)
                _newEdge.UpdateSrcPort();
            _newEdge.Data.IsSelected = false;
            _newEdge.UpdateDstPort();
            _newEdge.Data.FixDraggablePoints(_newEdge.Points.Last());
            _newEdge.RaisePointsChanged();
            _newEdge = null;
            IsInProgress = false;
        }

        public override void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_newEdge == null)
                return;

            var node = GetOriginalDataContext<IVertex>(e);
            if (node != null)
            {
                if (node == _srcNode)
                    return;

                var p1 = _newEdge.Points[_newEdge.Points.Count - 1];
                var p2 = _newEdge.Points[_newEdge.Points.Count - 2];
                var p = node.GetShapePort(p2);
                if (p1 != p)
                {
                    p1.X = p.X;
                    p1.Y = p.Y;
                    _newEdge.Data.FixDraggablePoints(p1);
                }
                _newEdge.RaisePointsChanged();
                return;
            }

            var vm = GetOriginalDataContext<GraphViewModel>(e);
            if (vm == null)
                return;

            var point = GetPoint(e);
            if (_newEdge.Points.Count == 1)
                addNewControlPoint(point);
            else
            {
                var lastPoint = _newEdge.Points.Last();
                lastPoint.X = point.X;
                lastPoint.Y = point.Y;
                if (_newEdge.Points.Count == 2)
                    _newEdge.UpdateSrcPort();
                _newEdge.Data.FixDraggablePoints(lastPoint);
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