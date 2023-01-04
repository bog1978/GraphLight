using System;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Collections;
using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class GraphControl : Control
    {
        private readonly GraphVizLayout<IVertexData, IEdgeData> _layout;
        private readonly INodeMeasure<IVertexData, IEdgeData> _measure;
        private Panel? _graphPanel;

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
            _measure = new DummyNodeMeasure<IVertexData, IEdgeData>();
            _layout = new GraphVizLayout<IVertexData, IEdgeData> { NodeMeasure = _measure };
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => ClearAllItems();

        private void OnLoaded(object sender, RoutedEventArgs e) => Layout();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _graphPanel = GetTemplateChild<Panel>("graphCanvas");
        }

        #region VertexTemplateDictionary

        public static readonly DependencyProperty VertexTemplateDictionaryProperty = DependencyProperty.Register(
            nameof(VertexTemplateDictionary), typeof(DataTemplateDictionary), typeof(GraphControl));

        public DataTemplateDictionary VertexTemplateDictionary
        {
            get => (DataTemplateDictionary)GetValue(VertexTemplateDictionaryProperty);
            set => SetValue(VertexTemplateDictionaryProperty, value);
        }

        #endregion

        #region Graph

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register(nameof(Graph), typeof(IGraph), typeof(GraphControl),
                new PropertyMetadata(OnGraphChanged));

        public IGraph? Graph
        {
            get => (IGraph?)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GraphControl control)
                control.Layout();
        }

        #endregion

        #region VertexStyle

        public static readonly DependencyProperty VertexStyleProperty = DependencyProperty.Register(
            nameof(VertexStyle), typeof(Style), typeof(GraphControl));

        public Style VertexStyle
        {
            get => (Style)GetValue(VertexStyleProperty);
            set => SetValue(VertexStyleProperty, value);
        }

        #endregion

        public void Layout()
        {
            ClearAllItems();
            if (!IsLoaded || Graph == null || _graphPanel == null)
                return;

            Graph.Vertices.Iter(AddVertex);
            _layout.Graph = Graph;
            _layout.NodeMeasure = _measure;
            _layout.Layout();
            Graph.Edges.Iter(AddEdge);
        }

        private void AddEdge(IEdge<IVertexData, IEdgeData> edge)
        {
            if (_graphPanel == null)
                return;
            var presenter = new EdgeControl
            {
                Content = edge,
                DataContext = edge
            };
            _graphPanel.Children.Add(presenter);
        }

        private void AddVertex(IVertex<IVertexData, IEdgeData> vertex)
        {
            if (_graphPanel == null)
                return;
            var presenter = new VertexControl
            {
                Content = vertex,
                DataContext = vertex,
                Style = VertexStyle,
                ContentTemplate = VertexTemplateDictionary.FindTemplate(vertex.Data.Category)
            };
            _graphPanel.Children.Add(presenter);
            presenter.UpdateLayout();
            vertex.Data.Width = presenter.DesiredSize.Width;
            vertex.Data.Height = presenter.DesiredSize.Height;
        }

        private T GetTemplateChild<T>(string childName)
            where T : DependencyObject =>
            GetTemplateChild(childName) switch
            {
                null => throw new ArgumentOutOfRangeException($"Template does not contain child [{childName}]."),
                T t => t,
                _ => throw new ArgumentOutOfRangeException($"Template contains child [{childName}] of different type."),
            };

        private void ClearAllItems() => _graphPanel?.Children.Clear();
    }
}