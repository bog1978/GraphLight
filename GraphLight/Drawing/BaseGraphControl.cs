using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public abstract class BaseGraphControl : Control
    {
        private Panel _graphPanel;
        protected bool _isLoaded;
        private readonly IDictionary<object, FrameworkElement> _elementMap;

        protected BaseGraphControl()
        {
            _elementMap = new Dictionary<object, FrameworkElement>();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            ClearAllItems();
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _graphPanel = GetTemplateChild<Panel>("graphCanvas");
        }

        #region VertexTemplateDictionary

        public static readonly DependencyProperty VertexTemplateDictionaryProperty = DependencyProperty.Register(
            "VertexTemplateDictionary", typeof(DataTemplateDictionary), typeof(BaseGraphControl),
            new PropertyMetadata(OnVertexTemplateDictionaryPropertyChanged));

        public DataTemplateDictionary VertexTemplateDictionary
        {
            get => (DataTemplateDictionary)GetValue(VertexTemplateDictionaryProperty);
            set => SetValue(VertexTemplateDictionaryProperty, value);
        }

        private static void OnVertexTemplateDictionaryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: Добавить реализацию.
        }

        #endregion

        #region Graph

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof(IGraph), typeof(BaseGraphControl),
                new PropertyMetadata(OnGraphChanged));

        public IGraph Graph
        {
            get => (IGraph)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldVal = (IGraph)e.OldValue;
            var newVal = (IGraph)e.NewValue;
            var control = (BaseGraphControl)d;
            control.OnGraphChanged(oldVal, newVal);
        }

        protected virtual void OnGraphChanged(IGraph oldVal, IGraph newVal)
        {
            if (oldVal != null)
            {
                if (oldVal.Edges is INotifyCollectionChanged edges)
                    edges.CollectionChanged -= OnEdgesCollectionChanged;
                if (oldVal.Vertices is INotifyCollectionChanged vertices)
                    vertices.CollectionChanged -= OnVertexCollectionChanged;
                ClearAllItems();
            }

            if (newVal != null)
            {
                if (newVal.Edges is INotifyCollectionChanged edges)
                    edges.CollectionChanged += OnEdgesCollectionChanged;
                if (newVal.Vertices is INotifyCollectionChanged vertices)
                    vertices.CollectionChanged += OnVertexCollectionChanged;
            }
        }

        #endregion

        #region VertexStyle

        public static readonly DependencyProperty VertexStyleProperty = DependencyProperty.Register(
            "VertexStyle", typeof(Style), typeof(BaseGraphControl),
            new PropertyMetadata(OnVertexStylePropertyChanged));

        public Style VertexStyle
        {
            get => (Style)GetValue(VertexStyleProperty);
            set => SetValue(VertexStyleProperty, value);
        }

        private static void OnVertexStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: Добавить реализацию.
        }

        #endregion

        private void OnVertexCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var vertex in e.NewItems.OfType<IVertex>())
                    AddVertex(vertex);
            if (e.OldItems != null)
                foreach (var vertex in e.OldItems)
                    DelItem(vertex);
        }

        private void OnEdgesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var edge in e.NewItems.OfType<IEdge>())
                    AddEdge(edge);
            if (e.OldItems != null)
                foreach (var edge in e.OldItems)
                    DelItem(edge);
        }

        private void AddEdge(IEdge<IVertexData, IEdgeData> edge)
        {
            var presenter = new Edge { Content = edge, DataContext = edge };
            _graphPanel.Children.Add(presenter);
            _elementMap.Add(edge, presenter);
        }

        private void AddVertex(IVertex<IVertexData, IEdgeData> vertex)
        {
            DataTemplate vertexTemplate = null;
            if (VertexTemplateDictionary != null)
            {
                if (string.IsNullOrWhiteSpace(vertex.Data.Category))
                    vertexTemplate = VertexTemplateDictionary.DefaultTemplate;
                else if (!VertexTemplateDictionary.TryGetValue(vertex.Data.Category, out vertexTemplate))
                    vertexTemplate = VertexTemplateDictionary.DefaultTemplate;
            }
            var presenter = new Node { Content = vertex, DataContext = vertex };
            if (VertexStyle != null)
                presenter.Style = VertexStyle;
            if (vertexTemplate != null)
                presenter.ContentTemplate = vertexTemplate;

            _graphPanel.Children.Add(presenter);
            // Update desired size of new UIElement.
            presenter.UpdateLayout();
            // Update properties of vertex model.
            vertex.Data.Width = presenter.DesiredSize.Width;
            vertex.Data.Height = presenter.DesiredSize.Height;
            _elementMap.Add(vertex, presenter);
        }

        private void DelItem(object item)
        {
            if (_elementMap.TryGetValue(item, out var elt))
                _graphPanel.Children.Remove(elt);
        }

        protected T GetTemplateChild<T>(string childName)
            where T : DependencyObject
        {
            var child = (T)GetTemplateChild(childName);
            if (child == null)
                throw new ArgumentOutOfRangeException($"Template does not contain child <{childName}>");
            return child;
        }

        protected void ClearAllItems()
        {
            if (_graphPanel == null || Graph == null)
                return;
            _graphPanel.Children.Clear();
            _elementMap.Clear();
        }

        protected void FillVertices()
        {
            if (_graphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Vertices)
                AddVertex(vertex);
        }

        protected void FillEdges()
        {
            if (_graphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Edges)
                AddEdge(vertex);
        }

        public abstract void Layout();
    }
}