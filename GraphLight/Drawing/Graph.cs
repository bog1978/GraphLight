using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphLight.Graph;
using GraphLight.Layout;
using GraphLight.Tools;

namespace GraphLight.Drawing
{
    public class GraphControl : BaseGraphControl
    {
        #region Private fields

        private readonly GraphVizLayout<IVertexData, IEdgeData> _layout;
        private readonly DummyNodeMeasure<IVertexData, IEdgeData> _measure;

        #endregion

        #region Initialization

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
            _measure = new DummyNodeMeasure<IVertexData, IEdgeData>();
            _layout = new GraphVizLayout<IVertexData, IEdgeData> { NodeMeasure = _measure };
            LayoutCommand = new DelegateCommand(Layout);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var layoutRoot = GetTemplateChild<FrameworkElement>("LayoutRoot");
            // Теперь это просто маркер
            layoutRoot.DataContext = new GraphViewModel();
        }

        #endregion

        #region API

        public override void Layout()
        {
            if (!_isLoaded || Graph == null)
                return;
            clearAllItems();
            fillVertices();
            _layout.Graph = Graph;
            _layout.NodeMeasure = _measure;
            _layout.Layout();
            shift();
            fillEdges();
        }

        protected override void OnGraphChanged(IGraph oldVal, IGraph newVal)
        {
            base.OnGraphChanged(oldVal, newVal);
            if (newVal != null)
                Layout();
        }

        #endregion

        #region SelectedEdge

        public static readonly DependencyProperty SelectedEdgeProperty = DependencyProperty.Register(
            "SelectedEdge", typeof(IEdge), typeof(GraphControl), new PropertyMetadata(default(IEdge)));

        public IEdge SelectedEdge
        {
            get => (IEdge)GetValue(SelectedEdgeProperty);
            private set => SetValue(SelectedEdgeProperty, value);
        }

        #endregion

        #region SelectedNode

        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register(
            "SelectedNode", typeof(IVertex), typeof(GraphControl), new PropertyMetadata(default(IVertex)));

        public IVertex SelectedNode
        {
            get => (IVertex)GetValue(SelectedNodeProperty);
            private set => SetValue(SelectedNodeProperty, value);
        }

        #endregion

        #region SelectedElement

        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register(
            "SelectedElement", typeof(object), typeof(GraphControl), new PropertyMetadata(onSelectedElementPropertyChanged));

        public object SelectedElement
        {
            get => GetValue(SelectedElementProperty);
            set => SetValue(SelectedElementProperty, value);
        }

        private static void onSelectedElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GraphControl)d;
            ctrl.onSelectedElementPropertyChanged(e.OldValue, e.NewValue);
        }

        private void onSelectedElementPropertyChanged(object oldElement, object newElement)
        {
            switch(oldElement)
            {
                case IVertex v:
                    v.Data.IsSelected = false;
                    break;
                case IEdge e:
                    e.Data.IsSelected = false;
                    break;
            }

            switch (newElement)
            {
                case IVertex v:
                    v.Data.IsSelected = true;
                    break;
                case IEdge e:
                    e.Data.IsSelected = true;
                    break;
            }

            SelectedNode = newElement as IVertex;
            SelectedEdge = newElement as IEdge;

            if (newElement != null)
                bringToTop(newElement);
        }

        #endregion

        #region LayoutCommand

        public static readonly DependencyProperty LayoutCommandProperty =
            DependencyProperty.Register("LayoutCommand", typeof(ICommand), typeof(GraphControl), null);

        public ICommand LayoutCommand
        {
            get => (ICommand)GetValue(LayoutCommandProperty);
            private set => SetValue(LayoutCommandProperty, value);
        }

        #endregion

        private void shift()
        {
            var minX = double.MaxValue;
            var maxX = double.MinValue;
            var minY = double.MaxValue;
            var maxY = double.MinValue;

            foreach (var vertex in Graph.Vertices)
            {
                if (vertex.Data.Left < minX)
                    minX = vertex.Data.Left;
                if (vertex.Data.Right > maxX)
                    maxX = vertex.Data.Right;
                if (vertex.Data.Top < minY)
                    minY = vertex.Data.Top;
                if (vertex.Data.Bottom > maxY)
                    maxY = vertex.Data.Bottom;
            }

            foreach (var edge in Graph.Edges)
            {
                foreach (var point2D in edge.Data.DraggablePoints)
                {
                    if (point2D.X < minX)
                        minX = point2D.X;
                    if (point2D.X > maxX)
                        maxX = point2D.X;
                    if (point2D.Y < minY)
                        minY = point2D.Y;
                    if (point2D.Y > maxY)
                        maxY = point2D.Y;
                }
            }

            var graphWidth = maxX - minX;
            var graphHeight = maxY - minY;

            foreach (var vertex in Graph.Vertices)
            {
                vertex.Data.Left -= minX;
                vertex.Data.Top -= minY;
            }

            foreach (var edge in Graph.Edges)
            {
                foreach (var point2D in edge.Data.DraggablePoints)
                {
                    point2D.X -= minX;
                    point2D.Y -= minY;
                }
            }

            Graph.Width = graphWidth;
            Graph.Height = graphHeight;
        }

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
            switch (element)
            {
                case IVertex node:
                    node.Data.ZIndex = z++;
                    break;
                case IEdge edge:
                    edge.Data.ZIndex = z++;
                    break;
            }

            return z;
        }
    }
}