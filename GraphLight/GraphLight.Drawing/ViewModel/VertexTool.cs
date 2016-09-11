using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Drawing;
using GraphLight.Graph;

namespace GraphLight.ViewModel
{
    public class VertexTool : GraphTool
    {
        private object _last;

        public VertexTool(GraphViewModel viewModel) : base(viewModel)
        {
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            Model.SelectedNode = e.OriginalSource.GetDataContext<IVertex>();
            if (_last == e.OriginalSource)
                return;
            setHighlight(_last, false);
            setHighlight(e.OriginalSource, true);
            _last = e.OriginalSource;
        }

        public override void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && Model.SelectedNode != null)
            {
                Model.Highlight(Model.SelectedNode, false);
                Model.Graph.RemoveVertex(Model.SelectedNode);
                Model.SelectedNode = null;
            }
        }

        public override void Cancel()
        {
            Model.SelectedNode = null;
            setHighlight(_last, false);
            _last = null;
        }

        #endregion

        private void setHighlight(object obj, bool isHighlighted)
        {
            if (obj == null)
                return;
            var node = obj.GetDataContext<IVertex>();
            if (node != null)
                Model.Highlight(node, isHighlighted);
        }
    }
}