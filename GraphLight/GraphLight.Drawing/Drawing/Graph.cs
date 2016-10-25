using System.Linq;
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

        private readonly GraphVizLayout _layout;
        private readonly DummyNodeMeasure _measure;
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
            _measure = new DummyNodeMeasure();
            _layout = new GraphVizLayout { NodeMeasure = _measure };
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
            fillVerteces();
            _layout.Graph = Graph;
            _layout.NodeMeasure = _measure;
            _layout.Layout();
            shift();
            fillEdges();
        }

        private void shift()
        {
            var minX = double.MaxValue;
            var maxX = double.MinValue;
            var minY = double.MaxValue;
            var maxY = double.MinValue;

            foreach (var vertex in Graph.Verteces)
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

            foreach (var vertex in Graph.Verteces)
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

        #endregion

        #region SelectedEdge

        public static readonly DependencyProperty SelectedEdgeProperty = DependencyProperty.Register(
            "SelectedEdge", typeof (IEdge), typeof (GraphControl), new PropertyMetadata(OnSelectedEdgePropertyChanged));

        public IEdge SelectedEdge
        {
            get { return (IEdge) GetValue(SelectedEdgeProperty); }
            private set { SetValue(SelectedEdgeProperty, value); }
        }

        private static void OnSelectedEdgePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GraphControl)d;
            var oldVal = (IEdge)e.OldValue;
            var newVal = (IEdge)e.NewValue;
            ctrl.OnSelectedEdgePropertyChanged(oldVal,newVal);
        }

        private void OnSelectedEdgePropertyChanged(IEdge oldEdge, IEdge newEdge)
        {
        }

        #endregion

        #region SelectedNode

        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register(
            "SelectedNode", typeof (IVertex), typeof (GraphControl), new PropertyMetadata(OnSelectedNodePropertyChanged));

        public IVertex SelectedNode
        {
            get { return (IVertex) GetValue(SelectedNodeProperty); }
            private set { SetValue(SelectedNodeProperty, value); }
        }

        private static void OnSelectedNodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GraphControl)d;
            var oldVal = (IVertex)e.OldValue;
            var newVal = (IVertex)e.NewValue;
            ctrl.OnSelectedNodePropertyChanged(oldVal, newVal);
        }

        private void OnSelectedNodePropertyChanged(IVertex oldVertex, IVertex newVertex)
        {
        }

        #endregion

        #region SelectedElement

        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register(
            "SelectedElement", typeof (IElement), typeof (GraphControl), new PropertyMetadata(OnSelectedElementPropertyChanged));

        public IElement SelectedElement
        {
            get { return (IElement) GetValue(SelectedElementProperty); }
            set { SetValue(SelectedElementProperty, value); }
        }

        private static void OnSelectedElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GraphControl)d;
            var oldVal = (IElement)e.OldValue;
            var newVal = (IElement)e.NewValue;
            ctrl.OnSelectedElementPropertyChanged(oldVal, newVal);
        }

        private void OnSelectedElementPropertyChanged(IElement oldElement, IElement newElement)
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

        private static readonly DependencyProperty _currentToolProperty = DependencyProperty.Register(
            "CurrentTool", typeof(GraphTool), typeof(GraphControl),
            new PropertyMetadata(onCurrentToolPropertyChanged));

        public GraphTool CurrentTool
        {
            get { return (GraphTool)GetValue(_currentToolProperty); }
            private set { SetValue(_currentToolProperty, value); }
        }

        private static void onCurrentToolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (GraphTool)e.OldValue;
            var newValue = (GraphTool)e.NewValue;
            var control = (GraphControl)d;
            // TODO: Добавить реализацию.
        }

        #endregion

        #region EdgeDrawingTool

        public static readonly DependencyProperty EdgeDrawingToolProperty = DependencyProperty.Register(
            "EdgeDrawingTool", typeof(GraphTool), typeof(GraphControl), null);

        public GraphTool EdgeDrawingTool
        {
            get { return (GraphTool)GetValue(EdgeDrawingToolProperty); }
            set { SetValue(EdgeDrawingToolProperty, value); }
        }

        #endregion

        #region DraggingTool

        public static readonly DependencyProperty DraggingToolProperty = DependencyProperty.Register(
            "DraggingTool", typeof(GraphTool), typeof(GraphControl), null);

        public GraphTool DraggingTool
        {
            get { return (GraphTool)GetValue(DraggingToolProperty); }
            set { SetValue(DraggingToolProperty, value); }
        }

        #endregion

        #region PanningTool

        public static readonly DependencyProperty PanningToolProperty = DependencyProperty.Register(
            "PanningTool", typeof(GraphTool), typeof(GraphControl), null);

        public GraphTool PanningTool
        {
            get { return (GraphTool)GetValue(PanningToolProperty); }
            set { SetValue(PanningToolProperty, value); }
        }

        #endregion

        #region LayoutCommand

        public static readonly DependencyProperty LayoutCommandProperty =
            DependencyProperty.Register("LayoutCommand", typeof(ICommand), typeof(GraphControl), null);

        public ICommand LayoutCommand
        {
            get { return (ICommand)GetValue(LayoutCommandProperty); }
            private set { SetValue(LayoutCommandProperty, value); }
        }

        #endregion

        #region Drag & Drop

        private bool onDragQuery(IDragDropOptions options)
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

        private bool onDropQuery(IDragDropOptions options)
        {
            return true;
        }

        private void onDropInfo(IDragDropOptions options)
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

        protected override void OnGraphChanged(IGraph oldVal, IGraph newVal)
        {
            base.OnGraphChanged(oldVal, newVal);
            if (newVal != null)
                Layout();
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
            var node = element as IVertex;
            if (node != null)
                node.ZIndex = z++;
            var edge = element as IEdge;
            if (edge != null)
                edge.ZIndex = z++;
            return z;
        }
    }
}