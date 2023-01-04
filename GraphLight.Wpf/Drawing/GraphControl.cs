using System;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Collections;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class GraphControl : Control
    {
        private bool _needLayout = true;
        private GraphPanel? _graphPanel;

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _graphPanel = GetTemplateChild<GraphPanel>("graphPanel");
            Layout();
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
            {
                control._needLayout = true;
                control.Layout();
            }
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
            if (Graph == null || _graphPanel == null || !_needLayout)
                return;
            _graphPanel?.Children.Clear();
            Graph.Vertices.Iter(AddVertex);
            Graph.Edges.Iter(AddEdge);
            _needLayout = false;
        }

        private void AddEdge(IEdge<IVertexData, IEdgeData> edge)
        {
            if (_graphPanel == null)
                return;
            var presenter = new EdgeControl
            {
                Content = edge,
                DataContext = edge,
                Data = edge.Data,
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
                ContentTemplate = VertexTemplateDictionary.FindTemplate(vertex.Data.Category),
                Data = vertex.Data,
            };
            _graphPanel.Children.Add(presenter);
        }

        private T GetTemplateChild<T>(string childName)
            where T : DependencyObject =>
            GetTemplateChild(childName) switch
            {
                null => throw new ArgumentOutOfRangeException($"Template does not contain child [{childName}]."),
                T t => t,
                _ => throw new ArgumentOutOfRangeException($"Template contains child [{childName}] of different type."),
            };
    }
}