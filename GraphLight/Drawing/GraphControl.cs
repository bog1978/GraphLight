using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Geometry;
using GraphLight.Graph;
using GraphLight.Tools;

namespace GraphLight.Drawing
{
    public class GraphControl : Control
    {
        #region Константы и поля

        private readonly GraphTool _edgeTool;
        private readonly GraphVizLayout _layout;
        private readonly DummyNodeMeasure _measure;
        private readonly GraphTool _pointTool;
        private readonly GraphTool _vertexTool;

        private GraphTool _currentTool;
        private Panel _graphPanel;
        private bool _isLoaded;
        private Grid _mainGrid;

        #endregion

        #region Конструкторы

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);

            _measure = new DummyNodeMeasure();
            _layout = new GraphVizLayout {NodeMeasure = _measure};

            _edgeTool = new EdgeTool(this);
            _vertexTool = new VertexTool(this);
            _pointTool = new ControlPointTool(this);

            LayoutCommand = new DelegateCommand(Layout);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Зависимые свойства

        #region Graph

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof(IGraph), typeof(GraphControl),
                new PropertyMetadata(OnGraphChanged));

        public IGraph Graph
        {
            get => (IGraph) GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldVal = (IGraph) e.OldValue;
            var newVal = (IGraph) e.NewValue;
            var control = (GraphControl) d;
            control.OnGraphChanged(oldVal, newVal);
        }

        #endregion

        #region LayoutCommand

        public static readonly DependencyProperty LayoutCommandProperty =
            DependencyProperty.Register("LayoutCommand", typeof(ICommand), typeof(GraphControl), null);

        public ICommand LayoutCommand
        {
            get => (ICommand) GetValue(LayoutCommandProperty);
            private set => SetValue(LayoutCommandProperty, value);
        }

        #endregion

        #region SelectedEdge

        public static readonly DependencyProperty SelectedEdgeProperty = DependencyProperty.Register(
            "SelectedEdge", typeof(IEdge), typeof(GraphControl), new PropertyMetadata(default(IEdge)));

        public IEdge SelectedEdge
        {
            get => (IEdge) GetValue(SelectedEdgeProperty);
            private set => SetValue(SelectedEdgeProperty, value);
        }

        #endregion

        #region SelectedElement

        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register(
            "SelectedElement", typeof(IElement), typeof(GraphControl), new PropertyMetadata(OnSelectedElementPropertyChanged));

        public IElement SelectedElement
        {
            get => (IElement) GetValue(SelectedElementProperty);
            set => SetValue(SelectedElementProperty, value);
        }

        private static void OnSelectedElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GraphControl) d;
            var oldVal = (IElement) e.OldValue;
            var newVal = (IElement) e.NewValue;
            ctrl.OnSelectedElementPropertyChanged(oldVal, newVal);
        }

        #endregion

        #region SelectedNode

        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register(
            "SelectedNode", typeof(IVertex), typeof(GraphControl), new PropertyMetadata(default(IVertex)));

        public IVertex SelectedNode
        {
            get => (IVertex) GetValue(SelectedNodeProperty);
            private set => SetValue(SelectedNodeProperty, value);
        }

        #endregion

        #region VertexStyle

        public static readonly DependencyProperty VertexStyleProperty = DependencyProperty.Register(
            "VertexStyle", typeof(Style), typeof(GraphControl), null);

        public Style VertexStyle
        {
            get => (Style) GetValue(VertexStyleProperty);
            set => SetValue(VertexStyleProperty, value);
        }

        #endregion

        #region VertexTemplateDictionary

        public static readonly DependencyProperty VertexTemplateDictionaryProperty = DependencyProperty.Register(
            "VertexTemplateDictionary", typeof(DataTemplateDictionary), typeof(GraphControl), null);

        public DataTemplateDictionary VertexTemplateDictionary
        {
            get => (DataTemplateDictionary) GetValue(VertexTemplateDictionaryProperty);
            set => SetValue(VertexTemplateDictionaryProperty, value);
        }

        #endregion

        #endregion

        #region FrameworkElement

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _graphPanel = GetTemplateChild<Panel>("graphCanvas");
            _mainGrid = GetTemplateChild<Grid>("mainGrid");
            var layoutRoot = GetTemplateChild<FrameworkElement>("LayoutRoot");
            // Теперь это просто маркер
            layoutRoot.DataContext = new GraphViewModel();
            _mainGrid.MouseMove += OnMouseMove;
            _mainGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
            _mainGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
            KeyUp += OnKeyUp;

            DragDropManager.AddDropQueryHandler(_mainGrid, OnDropQuery);
            DragDropManager.AddDropInfoHandler(_mainGrid, OnDropInfo);
            DragDropManager.AddDragQueryHandler(_mainGrid, OnDragQuery);
        }

        #endregion

        #region Обработчики событий

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            _currentTool?.HandleKeyUp(sender, e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                Layout();
            }
            else
            {
                FillVertices();
                FillEdges();
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectTool(e);
            _currentTool?.HandleLButtonDown(sender, e);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectTool(e);
            _currentTool?.HandleLButtonUp(sender, e);
            if (_currentTool != null && !_currentTool.IsInProgress)
                SelectTool(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _currentTool?.HandleMouseMove(sender, e);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            ClearAllItems();
        }

        #endregion

        #region Другое

        private static int SetZIndex(object element, int z)
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

        public void Layout()
        {
            if (!_isLoaded || Graph == null)
                return;
            ClearAllItems();
            FillVertices();
            _layout.Graph = Graph;
            _layout.NodeMeasure = _measure;
            _layout.Layout();
            Shift();
            FillEdges();
        }

        private void ClearAllItems()
        {
            if (_graphPanel == null || Graph == null)
                return;
            _graphPanel.Children.Clear();
        }

        private void FillEdges()
        {
            if (_graphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Edges)
                AddEdge(vertex);
        }

        private void FillVertices()
        {
            if (_graphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Vertices)
                AddVertex(vertex);
        }

        private T GetTemplateChild<T>(string childName)
            where T : DependencyObject
        {
            var child = (T) GetTemplateChild(childName);
            if (child == null)
                throw new ArgumentOutOfRangeException($"Template does not contain child <{childName}>");
            return child;
        }

        private void AddEdge(IEdge edge)
        {
            var presenter = new EdgeControl {Content = edge, DataContext = edge};
            _graphPanel.Children.Add(presenter);
        }

        private void AddVertex(IVertex vertex)
        {
            DataTemplate vertexTemplate = null;
            if (VertexTemplateDictionary != null)
            {
                if (string.IsNullOrWhiteSpace(vertex.Category))
                    vertexTemplate = VertexTemplateDictionary.DefaultTemplate;
                else if (!VertexTemplateDictionary.TryGetValue(vertex.Category, out vertexTemplate))
                    vertexTemplate = VertexTemplateDictionary.DefaultTemplate;
            }

            var presenter = new NodeControl {Content = vertex, DataContext = vertex};
            if (VertexStyle != null)
                presenter.Style = VertexStyle;
            if (vertexTemplate != null)
                presenter.ContentTemplate = vertexTemplate;

            _graphPanel.Children.Add(presenter);
            // Update desired size of new UIElement.
            presenter.UpdateLayout();
            // Update properties of vertex model.
            vertex.Width = presenter.DesiredSize.Width;
            vertex.Height = presenter.DesiredSize.Height;
        }

        private void BringToTop(IElement item)
        {
            var z = 0;
            foreach (var element in Graph.Elements)
            {
                if (ReferenceEquals(element, item))
                    continue;
                z = SetZIndex(element, z);
            }

            SetZIndex(item, z);
        }

        private bool OnDragQuery(IDragDropOptions options) => _currentTool != null && _currentTool.HandleDragQuery(options);

        private void OnDropInfo(IDragDropOptions options)
        {
            _currentTool?.HandleDropInfo(options);
        }

        private bool OnDropQuery(IDragDropOptions options) => _currentTool != null && _currentTool.HandleDropQuery(options);

        private void OnGraphChanged(IGraph oldVal, IGraph newVal)
        {
            if (oldVal != null)
                ClearAllItems();

            if (newVal != null)
                Layout();
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
                BringToTop(newElement);
        }

        private void SelectTool(RoutedEventArgs e)
        {
            if (_currentTool != null && _currentTool.IsInProgress)
                return;

            var element = (FrameworkElement) e.OriginalSource;
            var data = element.DataContext;

            switch (data)
            {
                case IVertex _:
                    _currentTool = _vertexTool;
                    break;
                case IEdge _:
                    _currentTool = _edgeTool;
                    break;
                case Point2D _:
                    _currentTool = _pointTool;
                    break;
                case GraphViewModel _:
                {
                    SelectedElement = null;
                    if (_currentTool != null)
                    {
                        _currentTool.Cancel();
                        _currentTool = null;
                    }
                    break;
                }
            }
        }

        private void Shift()
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

            var graphWidth = maxX - minX;
            var graphHeight = maxY - minY;

            foreach (var vertex in Graph.Vertices)
            {
                vertex.Left -= minX;
                vertex.Top -= minY;
            }

            foreach (var edge in Graph.Edges)
            foreach (var point2D in edge.DraggablePoints)
            {
                point2D.X -= minX;
                point2D.Y -= minY;
            }

            Graph.Width = graphWidth;
            Graph.Height = graphHeight;
        }

        #endregion
    }
}