using System.Windows;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Drawing;
using GraphLight.Graph;

namespace GraphLight.Tools
{
    public class VertexTool : GraphTool
    {
        private object _last;

        public VertexTool(GraphControl viewModel) : base(viewModel) { }

        #region IGraphTool Members

        public override void HandleLButtonUp(object sender, MouseButtonEventArgs e)
        {
            Model.SelectedElement = e.OriginalSource.GetDataContext<IVertex>();
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
                highlight(Model.SelectedNode, false);
                Model.Graph.RemoveVertex(Model.SelectedNode);
                Model.SelectedElement = null;
            }
        }

        public override void Cancel()
        {
            Model.SelectedElement = null;
            setHighlight(_last, false);
            _last = null;
        }

        public override bool HandleDragQuery(IDragDropOptions options)
        {
            var vertex = options.Source.DataContext as IVertex;
            if (vertex == null)
                return false;
            options.Payload = new Point(vertex.Left, vertex.Top);
            return vertex.IsSelected;
        }

        public override void HandleDropInfo(IDragDropOptions options)
        {
            var vertex = options.Source.DataContext as IVertex;
            if (vertex == null)
                return;
            switch (options.Mode)
            {
                case DragDropMode.DragExisting:
                    var p = (Point)options.Payload;
                    vertex.Left = p.X + options.DeltaX;
                    vertex.Top = p.Y + options.DeltaY;
                    vertex.Update();
                    break;
                case DragDropMode.DragCopy:
                    if (options.Status == DragDropStatus.Completed)
                    {
                        var v = Model.Graph.AddVertex(vertex.Data);
                        v.Left = options.Relative.X - vertex.Width / 2;
                        v.Top = options.Relative.Y - vertex.Height / 2;
                        v.Label = vertex.Label;
                        v.Category = vertex.Category;
                    }
                    break;
            }
        }

        #endregion

        private static void setHighlight(object obj, bool isHighlighted)
        {
            if (obj == null)
                return;
            var node = obj.GetDataContext<IVertex>();
            if (node != null)
                highlight(node, isHighlighted);
        }

        private static void highlight(IVertex node, bool isHighlighted)
        {
            if (!node.IsSelected)
                node.IsHighlighted = isHighlighted;

            foreach (var edge in node.InEdges)
            {
                if (!edge.IsSelected)
                    edge.IsHighlighted = isHighlighted;
                if (!edge.Src.IsSelected)
                    edge.Src.IsHighlighted = isHighlighted;
            }
            foreach (var edge in node.OutEdges)
            {
                if (!edge.IsSelected)
                    edge.IsHighlighted = isHighlighted;
                if (!edge.Dst.IsSelected)
                    edge.Dst.IsHighlighted = isHighlighted;
            }
            foreach (IEdge edge in node.SelfEdges)
                if (!edge.IsSelected)
                    edge.IsHighlighted = isHighlighted;
        }
    }
}