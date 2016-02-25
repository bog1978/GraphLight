using System.Windows;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.ViewModel
{
    public class DrawNodeTool : GraphTool
    {
        private static int _num = 1;
        private readonly GraphViewModel _viewModel;

        public DrawNodeTool(GraphViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        #region IGraphTool Members

        public override string Name
        {
            get { return "Node"; }
        }

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        public override void HandleLButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = GetPoint(e);
            var vm = GetOriginalDataContext<GraphViewModel>(e);
            if (vm == _viewModel)
            {
                var data = new VertexAttrs("_v_" + _num);
                data.Label = "New vertex " + _num;
                _num++;
                var node = (DrawingVertex)_viewModel.Graph.AddVertex(data);
                // After adding vertex corresponding UIElement is added into Canvas.
                // All sizes are already calculated.
                node.Data.Left = point.X - node.Data.Width / 2;
                node.Data.Top = point.Y - node.Data.Height / 2;
                _viewModel.SelectedEdge = null;
                _viewModel.SelectedNode = node;
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