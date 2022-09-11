using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Geometry;
using GraphLight.Graph;
using GraphLight.Layout;
using GraphLight.Tools;

namespace GraphLight.Drawing
{
    public class GraphControl : BaseGraphControl
    {
        #region Private fields

        private readonly GraphVizLayout<object, object> _layout;
        private readonly DummyNodeMeasure<object, object> _measure;
        private readonly GraphTool _edgeDrawingTool;
        private readonly GraphTool _edgeTool;
        private readonly GraphTool _vertexTool;
        private readonly GraphTool _pointTool;
        private GraphTool _currentTool;
        private Grid _mainGrid;

        #endregion

        #region Initialization

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
            _measure = new DummyNodeMeasure<object, object>();
            _layout = new GraphVizLayout<object, object> { NodeMeasure = _measure };
            _edgeDrawingTool = new DrawEdgeTool(this);
            _edgeTool = new EdgeTool(this);
            _vertexTool = new VertexTool(this);
            _pointTool = new ControlPointTool(this);

            LayoutCommand = new DelegateCommand(Layout);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mainGrid = GetTemplateChild<Grid>("mainGrid");
            var layoutRoot = GetTemplateChild<FrameworkElement>("LayoutRoot");
            // Теперь это просто маркер
            layoutRoot.DataContext = new GraphViewModel();
            _mainGrid.MouseMove += onMouseMove;
            _mainGrid.MouseLeftButtonDown += onMouseLeftButtonDown;
            _mainGrid.MouseLeftButtonUp += onMouseLeftButtonUp;
            KeyUp += onKeyUp;

            DragDropManager.AddDropQueryHandler(_mainGrid, onDropQuery);
            DragDropManager.AddDropInfoHandler(_mainGrid, onDropInfo);
            DragDropManager.AddDragQueryHandler(_mainGrid, onDragQuery);
        }

        #endregion

        #region API

        public override void Layout()
        {
            if (!_isLoaded || Graph == null)
                return;
            clearAllItems();
            fillVertices();
            _layout.Graph = (IGraph<object, object>)Graph;
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

        #region Graph event handlers

        private void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectTool(e);
            if (_currentTool != null)
                _currentTool.HandleLButtonDown(sender, e);
        }

        private void onMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectTool(e);
            if (_currentTool != null)
                _currentTool.HandleLButtonUp(sender, e);
            if (_currentTool != null && !_currentTool.IsInProgress)
                selectTool(e);
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            if (_currentTool != null)
                _currentTool.HandleMouseMove(sender, e);
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (_currentTool != null)
                _currentTool.HandleKeyUp(sender, e);
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
            "SelectedElement", typeof(IElement), typeof(GraphControl), new PropertyMetadata(onSelectedElementPropertyChanged));

        public IElement SelectedElement
        {
            get => (IElement)GetValue(SelectedElementProperty);
            set => SetValue(SelectedElementProperty, value);
        }

        private static void onSelectedElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GraphControl)d;
            var oldVal = (IElement)e.OldValue;
            var newVal = (IElement)e.NewValue;
            ctrl.onSelectedElementPropertyChanged(oldVal, newVal);
        }

        private void onSelectedElementPropertyChanged(IElement oldElement, IElement newElement)
        {
            if (oldElement != null)
                oldElement.IsSelected = false;

            if (newElement != null)
                newElement.IsSelected = true;

            SelectedNode = newElement as IVertex;
            SelectedEdge = newElement as IEdge;

            if (newElement != null)
                bringToTop(newElement);
        }

        #endregion

        #region CurrentTool

        public static readonly DependencyProperty CurrentToolProperty = DependencyProperty.Register(
            "CurrentTool", typeof(GraphTool), typeof(GraphControl), new PropertyMetadata(default(GraphTool)));

        public GraphTool CurrentTool
        {
            get => (GraphTool)GetValue(CurrentToolProperty);
            private set => SetValue(CurrentToolProperty, value);
        }

        #endregion

        #region EdgeDrawingTool

        public static readonly DependencyProperty EdgeDrawingToolProperty = DependencyProperty.Register(
            "EdgeDrawingTool", typeof(GraphTool), typeof(GraphControl), null);

        public GraphTool EdgeDrawingTool
        {
            get => (GraphTool)GetValue(EdgeDrawingToolProperty);
            set => SetValue(EdgeDrawingToolProperty, value);
        }

        #endregion

        #region DraggingTool

        public static readonly DependencyProperty DraggingToolProperty = DependencyProperty.Register(
            "DraggingTool", typeof(GraphTool), typeof(GraphControl), null);

        public GraphTool DraggingTool
        {
            get => (GraphTool)GetValue(DraggingToolProperty);
            set => SetValue(DraggingToolProperty, value);
        }

        #endregion

        #region PanningTool

        public static readonly DependencyProperty PanningToolProperty = DependencyProperty.Register(
            "PanningTool", typeof(GraphTool), typeof(GraphControl), null);

        public GraphTool PanningTool
        {
            get => (GraphTool)GetValue(PanningToolProperty);
            set => SetValue(PanningToolProperty, value);
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

        #region Drag & Drop

        private bool onDragQuery(IDragDropOptions options)
        {
            return _currentTool != null && _currentTool.HandleDragQuery(options);
        }

        private bool onDropQuery(IDragDropOptions options)
        {
            return _currentTool != null && _currentTool.HandleDropQuery(options);
        }

        private void onDropInfo(IDragDropOptions options)
        {
            if (_currentTool != null)
                _currentTool.HandleDropInfo(options);
        }

        #endregion

        private void selectTool(RoutedEventArgs e)
        {
            if (_currentTool != null && _currentTool.IsInProgress)
                return;

            var element = (FrameworkElement)e.OriginalSource;
            var data = element.DataContext;

            var vertex = data as IVertex;
            var edge = data as IEdge;
            var point = data as Point2D;
            var vm = data as GraphViewModel;

            //if (vertex != null)
            //    _currentTool = !vertex.IsSelected
            //        ? _edgeDrawingTool
            //        : _vertexTool;
            if (vertex != null)
                _currentTool = Keyboard.Modifiers == ModifierKeys.Control
                    ? _edgeDrawingTool
                    : _vertexTool;
            else if (edge != null)
                _currentTool = _edgeTool;
            else if (point != null)
                _currentTool = _pointTool;
            else if (vm != null)
            {
                SelectedElement = null;
                if (_currentTool != null)
                {
                    _currentTool.Cancel();
                    _currentTool = null;
                }
            }
        }

        private void shift()
        {
            var minX = double.MaxValue;
            var maxX = double.MinValue;
            var minY = double.MaxValue;
            var maxY = double.MinValue;

            foreach (var vertex in Graph.Vertices)
            {
                if (vertex.Left < minX)
                    minX = vertex.Left;
                if (vertex.Right > maxX)
                    maxX = vertex.Right;
                if (vertex.Top < minY)
                    minY = vertex.Top;
                if (vertex.Bottom > maxY)
                    maxY = vertex.Bottom;
            }

            foreach (var edge in Graph.Edges)
            {
                foreach (var point2D in edge.DraggablePoints)
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
                vertex.Left -= minX;
                vertex.Top -= minY;
            }

            foreach (var edge in Graph.Edges)
            {
                foreach (var point2D in edge.DraggablePoints)
                {
                    point2D.X -= minX;
                    point2D.Y -= minY;
                }
            }

            Graph.Width = graphWidth;
            Graph.Height = graphHeight;
        }

        private void bringToTop(IElement item)
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
                    node.ZIndex = z++;
                    break;
                case IEdge edge:
                    edge.ZIndex = z++;
                    break;
            }

            return z;
        }
    }
}