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

        private DrawingGraph _graph;

        public DrawingGraph Graph
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

        private DrawingEdge _selectedEdge;

        private DrawingVertex _selectedNode;

        public DrawingEdge SelectedEdge
        {
            get { return _selectedEdge; }
            set
            {
                if (_selectedNode != null)
                {
                    _selectedNode.Data.IsSelected = false;
                    _selectedNode = null;
                }
                if (_selectedEdge != null)
                    _selectedEdge.Data.IsSelected = false;
                _selectedEdge = value;
                RaisePropertyChanged("SelectedEdge");
                if (_selectedEdge != null)
                {
                    _selectedEdge.Data.IsSelected = true;
                    bringToTop(value);
                }

            }
        }

        public DrawingVertex SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (SelectedEdge != null)
                {
                    SelectedEdge.Data.IsSelected = false;
                    SelectedEdge = null;
                }
                if (_selectedNode != null)
                    _selectedNode.Data.IsSelected = false;
                _selectedNode = value;
                RaisePropertyChanged("SelectedNode");
                if (_selectedNode != null)
                {
                    _selectedNode.Data.IsSelected = true;
                    bringToTop(value);
                }
            }
        }

        #endregion

        #region Drag & Drop

        internal bool OnDragQuery(IDragDropOptions options)
        {
            var vertex = options.Source.DataContext as DrawingVertex;
            var point = options.Source.DataContext as Point2D;
            if (vertex != null)
            {
                options.Payload = new Point(vertex.Data.Left, vertex.Data.Top);
                return vertex.Data.IsSelected;
            }
            if (point != null && SelectedEdge != null)
            {
                var points = SelectedEdge.Data.Points;
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
            var vertex = options.Source.DataContext as DrawingVertex;
            var point = options.Source.DataContext as Point2D;
            if (vertex != null && options.Mode == DragDropMode.DragExisting)
            {
                var p = (Point)options.Payload;
                vertex.Data.Left = p.X + options.DeltaX;
                vertex.Data.Top = p.Y + options.DeltaY;
                vertex.Update();
            }
            if (vertex != null && options.Mode == DragDropMode.DragCopy
                && options.Status == DragDropStatus.Completed)
            {
                var clone = (VertexAttrs)vertex.Data.Clone();
                clone.Left = options.Relative.X - vertex.Data.Width / 2;
                clone.Top = options.Relative.Y - vertex.Data.Height / 2;
                Graph.AddVertex(clone);
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
            var node = element as DrawingVertex;
            if (node != null)
                node.Data.ZIndex = z++;
            var edge = element as DrawingEdge;
            if (edge != null)
                edge.Data.ZIndex = z++;
            return z;
        }

        public void Highlight(DrawingVertex node, bool isHighlighted)
        {
            if (!node.Data.IsSelected)
                node.Data.IsHighlighted = isHighlighted;

            foreach (var edge in node.InEdges)
            {
                if (!edge.Data.IsSelected)
                    edge.Data.IsHighlighted = isHighlighted;
                if (!edge.Src.Data.IsSelected)
                    edge.Src.Data.IsHighlighted = isHighlighted;
            }
            foreach (var edge in node.OutEdges)
            {
                if (!edge.Data.IsSelected)
                    edge.Data.IsHighlighted = isHighlighted;
                if (!edge.Dst.Data.IsSelected)
                    edge.Dst.Data.IsHighlighted = isHighlighted;
            }
            foreach (var edge in node.SelfEdges)
                if (!edge.Data.IsSelected)
                    edge.Data.IsHighlighted = isHighlighted;
        }

        public void Highlight(DrawingEdge edge, bool isHighlighted)
        {
            if (!edge.Data.IsSelected)
                edge.Data.IsHighlighted = isHighlighted;
            if (!edge.Src.Data.IsSelected)
                edge.Src.Data.IsHighlighted = isHighlighted;
            if (!edge.Dst.Data.IsSelected)
                edge.Dst.Data.IsHighlighted = isHighlighted;
        }
    }
}