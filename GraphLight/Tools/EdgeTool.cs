using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Drawing;
using GraphLight.Graph;

namespace GraphLight.Tools
{
    public class EdgeTool : GraphTool
    {
        private object _last;

        public EdgeTool(GraphControl viewModel)
            : base(viewModel)
        {
        }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            Model.SelectedElement = e.OriginalSource.GetDataContext<IEdge>();
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
                Model.SelectedElement = null;
            }
        }

        public override void Cancel()
        {
            Model.SelectedElement = null;
            setHighlight(_last, false);
            _last = null;
        }

        #endregion

        private static void setHighlight(object obj, bool isHighlighted)
        {
            if (obj == null)
                return;
            var edge = obj.GetDataContext<IEdge>();
            if (edge != null)
                highlight(edge, isHighlighted);
        }

        private static void highlight(IEdge edge, bool isHighlighted)
        {
            if (!edge.IsSelected)
                edge.IsHighlighted = isHighlighted;
            if (edge.Src != null && !((IVertex)edge.Src).IsSelected)
                ((IVertex)edge.Src).IsHighlighted = isHighlighted;
            if (edge.Dst != null && !((IVertex)edge.Dst).IsSelected)
                ((IVertex)edge.Dst).IsHighlighted = isHighlighted;
        }
    }
}