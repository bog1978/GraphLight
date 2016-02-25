using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Graph;

namespace GraphLight.ViewModel
{
    public class EdgeTool : GraphTool
    {
        private object _last;

        public EdgeTool(GraphViewModel viewModel)
            : base(viewModel)
        {
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            Model.SelectedEdge = e.OriginalSource.GetDataContext<DrawingEdge>();
            if (_last == e.OriginalSource)
                return;
            setHighlight(_last, false);
            setHighlight(e.OriginalSource, true);
            _last = e.OriginalSource;
        }

        public override void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && Model.SelectedEdge != null)
            {
                Model.Graph.RemoveEdge(Model.SelectedEdge);
                Model.SelectedEdge = null;
            }
        }

        public override void Cancel()
        {
            Model.SelectedEdge = null;
            setHighlight(_last, false);
            _last = null;
        }

        #endregion

        private void setHighlight(object obj, bool isHighlighted)
        {
            if (obj == null)
                return;
            var edge = obj.GetDataContext<DrawingEdge>();
            if (edge != null)
                Model.Highlight(edge, isHighlighted);
        }
    }
}