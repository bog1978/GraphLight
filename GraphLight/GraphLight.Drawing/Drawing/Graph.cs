using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GraphLight.Controls;
using GraphLight.ViewModel;
using GraphLight.Geometry;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class GraphControl : BaseGraphControl
    {
        #region Private fields

        private readonly GraphViewModel _viewModel;
        private readonly GraphVizLayout _layout;
        private readonly DummyNodeMeasure _measure;
        private Grid _mainGrid;
        private ScrollViewer _scrollViewer;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private GraphTool _edgeDrawingTool;
        private GraphTool _edgeTool;
        private GraphTool _vertexTool;
        private GraphTool _pointTool;

        #endregion

        #region Initialization

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
            _measure = new DummyNodeMeasure();
            _layout = new GraphVizLayout { NodeMeasure = _measure };
            _viewModel = new GraphViewModel();
            _edgeDrawingTool = new DrawEdgeTool(_viewModel);
            _edgeTool = new EdgeTool(_viewModel);
            _vertexTool = new VertexTool(_viewModel);
            _pointTool = new ControlPointTool(_viewModel);

            LayoutCommand = new DelegateCommand(Layout);

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += onTimerTick;
        }

        void onTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            clearAllItems();
            shift();
            fillVerteces();
            fillEdges();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mainGrid = GetTemplateChild<Grid>("mainGrid");
            _scrollViewer = GetTemplateChild<ScrollViewer>("scrollViewer");
            var layoutRoot = GetTemplateChild<FrameworkElement>("LayoutRoot");
            layoutRoot.DataContext = _viewModel;
            _mainGrid.MouseMove += onMouseMove;
            _mainGrid.MouseLeftButtonDown += onMouseLeftButtonDown;
            _mainGrid.MouseLeftButtonUp += onMouseLeftButtonUp;
            _mainGrid.SizeChanged += onSizeChanged;
            KeyUp += onKeyUp;

            DragDropManager.AddDropQueryHandler(_mainGrid, _viewModel.OnDropQuery);
            DragDropManager.AddDropInfoHandler(_mainGrid, _viewModel.OnDropInfo);
            DragDropManager.AddDragQueryHandler(_mainGrid, _viewModel.OnDragQuery);
        }

        void onSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isLoaded || Graph == null)
                return;
            _timer.Stop();
            _timer.Start();
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
            var viewWidth = _scrollViewer.ViewportWidth - 1;
            var viewHeight = _scrollViewer.ViewportHeight - 1;
            if (double.IsNaN(viewWidth) || double.IsNaN(viewHeight))
                return;

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
            var graphWidth = maxX - minX;
            var graphHeight = maxY - minY;

            var leftSpace = graphWidth < viewWidth
                ? (viewWidth - graphWidth) / 2
                : 0.0;

            var topSpace = (graphHeight < viewHeight)
                ? (viewHeight - graphHeight) / 2
                : 0.0;

            var leftShift = leftSpace - minX;
            var topShift = topSpace - minY;

            if (leftShift != 0 || topShift != 0)
            {
                foreach (var vertex in Graph.Verteces)
                {
                    vertex.Left += leftShift;
                    vertex.Top += topShift;
                }
                foreach (var edge in Graph.Edges)
                {
                    foreach (var point2D in edge.DraggablePoints)
                    {
                        point2D.X += leftShift;
                        point2D.Y += topShift;
                    }
                }

                Graph.Width = Math.Max(graphWidth, viewWidth);
                Graph.Height = Math.Max(graphHeight, viewHeight);
            }
        }

        #endregion

        #region Graph event handlers

        private GraphTool _currentTool;

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

            if (vertex != null)
                _currentTool = !vertex.IsSelected
                    ? _edgeDrawingTool
                    : _vertexTool;
            else if (edge != null)
                _currentTool = _edgeTool;
            else if (point != null)
                _currentTool = _pointTool;
            else if (vm != null)
            {
                _viewModel.SelectedEdge = null;
                _viewModel.SelectedNode = null;
                if (_currentTool != null)
                {
                    _currentTool.Cancel();
                    _currentTool = null;
                }
            }
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

        protected override void OnGraphChanged(IGraph oldVal, IGraph newVal)
        {
            base.OnGraphChanged(oldVal, newVal);
            _viewModel.Graph = newVal;
            if (newVal != null)
                Layout();
        }
    }
}