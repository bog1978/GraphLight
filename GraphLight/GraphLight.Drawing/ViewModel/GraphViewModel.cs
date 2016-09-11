using System.Linq;
using System.Windows;
using GraphLight.Controls;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.ViewModel
{
    public class GraphViewModel : BaseViewModel
    {
        #region Graph

        private IGraph _graph;

        public IGraph Graph
        {
            get { return _graph; }
            set
            {
                SelectedEdge = null;
                SelectedNode = null;
                _graph = value;
                RaisePropertyChanged("Graph");
            }
        }

        #endregion

        #region Selections

        private IEdge _selectedEdge;

        private IVertex _selectedNode;

        public IEdge SelectedEdge
        {
            get { return _selectedEdge; }
            set
            {
                if (_selectedNode != null)
                {
                    _selectedNode.IsSelected = false;
                    _selectedNode = null;
                }
                if (_selectedEdge != null)
                    _selectedEdge.IsSelected = false;
                _selectedEdge = value;
                RaisePropertyChanged("SelectedEdge");
                if (_selectedEdge != null)
                {
                    _selectedEdge.IsSelected = true;
                    bringToTop(value);
                }
            }
        }

        public IVertex SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (SelectedEdge != null)
                {
                    SelectedEdge.IsSelected = false;
                    SelectedEdge = null;
                }
                if (_selectedNode != null)
                    _selectedNode.IsSelected = false;
                _selectedNode = value;
                RaisePropertyChanged("SelectedNode");
                if (_selectedNode != null)
                {
                    _selectedNode.IsSelected = true;
                    bringToTop(value);
                }
            }
        }

        #endregion

        #region Drag & Drop

        internal bool OnDragQuery(IDragDropOptions options)
        {
            var vertex = options.Source.DataContext as IVertex;
            var point = options.Source.DataContext as Point2D;
            if (vertex != null)
            {
                options.Payload = new Point(vertex.Left, vertex.Top);
                return vertex.IsSelected;
            }
            if (point != null && SelectedEdge != null)
            {
                var points = SelectedEdge.Points;
                if (points.First() == point || points.Last() == point)
                    return false;
                options.Payload = new Point(point.X, point.Y);
                return true;
            }
            return false;
        }

        internal bool OnDropQuery(IDragDropOptions options)
        {
            return true;
        }

        internal void OnDropInfo(IDragDropOptions options)
        {
            var vertex = options.Source.DataContext as IVertex;
            var point = options.Source.DataContext as Point2D;
            if (vertex != null && options.Mode == DragDropMode.DragExisting)
            {
                var p = (Point)options.Payload;
                vertex.Left = p.X + options.DeltaX;
                vertex.Top = p.Y + options.DeltaY;
                vertex.Update();
            }
            if (vertex != null && options.Mode == DragDropMode.DragCopy
                && options.Status == DragDropStatus.Completed)
            {
                var v = Graph.AddVertex();
                v.Left = options.Relative.X - vertex.Width / 2;
                v.Top = options.Relative.Y - vertex.Height / 2;
                v.Label = vertex.Label;
                v.Category = vertex.Category;
            }
            if (point != null)
            {
                var p = (Point)options.Payload;
                if (SelectedEdge != null)
                {
                    using (SelectedEdge.DeferRefresh())
                    {
                        point.X = p.X + options.DeltaX;
                        point.Y = p.Y + options.DeltaY;
                        SelectedEdge.UpdatePoint(point);
                    }
                }
            }
        }

        #endregion

        private void bringToTop(object item)
        {
            var z = 0;
            foreach (var element in Graph.Elements)
            {
                if (ReferenceEquals(element, item))
                    continue;
                z = setZIndex(element, z);
            }
            setZIndex(item, z);
        }

        private static int setZIndex(object element, int z)
        {
            var node = element as IVertex;
            if (node != null)
                node.ZIndex = z++;
            var edge = element as IEdge;
            if (edge != null)
                edge.ZIndex = z++;
            return z;
        }

        public void Highlight(IVertex node, bool isHighlighted)
        {
            if (!node.IsSelected)
                node.IsHighlighted = isHighlighted;

            foreach (IEdge edge in node.InEdges)
            {
                if (!edge.IsSelected)
                    edge.IsHighlighted = isHighlighted;
                if (!edge.Src.IsSelected)
                    edge.Src.IsHighlighted = isHighlighted;
            }
            foreach (IEdge edge in node.OutEdges)
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

        public void Highlight(IEdge edge, bool isHighlighted)
        {
            if (!edge.IsSelected)
                edge.IsHighlighted = isHighlighted;
            if (!edge.Src.IsSelected)
                edge.Src.IsHighlighted = isHighlighted;
            if (!edge.Dst.IsSelected)
                edge.Dst.IsHighlighted = isHighlighted;
        }
    }
}