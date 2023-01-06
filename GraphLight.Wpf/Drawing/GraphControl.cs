using System;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class GraphControl : Control
    {
        private GraphPanel? _graphPanel;

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
            Loaded += OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _graphPanel = (GraphPanel?)GetTemplateChild("graphPanel");
            RefreshPanel();
        }

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
                control.RefreshPanel();
        }

        #endregion

        private void OnLoaded(object sender, RoutedEventArgs e) => RefreshPanel();

        public void RefreshPanel()
        {
            if (!IsLoaded || Graph == null || _graphPanel == null)
                return;
            _graphPanel.Children.Clear();
            foreach (var vertex in Graph.Vertices)
                _graphPanel.Children.Add(new VertexControl { DataContext = vertex, Data = vertex.Data });
            foreach (var edge in Graph.Edges)
                _graphPanel.Children.Add(new EdgeControl { DataContext = edge, Data = edge.Data });
        }
    }
}