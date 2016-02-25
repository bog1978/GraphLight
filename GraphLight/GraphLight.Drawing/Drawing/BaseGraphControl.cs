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
        protected Panel GraphPanel { get; private set; }
        protected bool _isLoaded;
        private readonly IDictionary<object, FrameworkElement>
            _elementMap = new Dictionary<object, FrameworkElement>();

        protected BaseGraphControl()
        {
            Loaded += onLoaded;
            Unloaded += onUnloaded;
        }

        void onUnloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            clearAllItems();
        }

        void onLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                Layout();
            }
            else
            {
                fillVerteces();
                fillEdges();
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GraphPanel = GetTemplateChild<Panel>("graphCanvas");
        }

        #region VertexTemplateDictionary

        public static readonly DependencyProperty VertexTemplateDictionaryProperty = DependencyProperty.Register(
            "VertexTemplateDictionary", typeof(DataTemplateDictionary), typeof(BaseGraphControl),
            new PropertyMetadata(onVertexTemplateDictionaryPropertyChanged));

        public DataTemplateDictionary VertexTemplateDictionary
        {
            get { return (DataTemplateDictionary)GetValue(VertexTemplateDictionaryProperty); }
            set { SetValue(VertexTemplateDictionaryProperty, value); }
        }

        private static void onVertexTemplateDictionaryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (DataTemplateDictionary)e.OldValue;
            var newValue = (DataTemplateDictionary)e.NewValue;
            var control = (BaseGraphControl)d;
            // TODO: Добавить реализацию.
        }

        #endregion

        #region Graph

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof(DrawingGraph), typeof(BaseGraphControl),
                new PropertyMetadata(onGraphChanged));

        public DrawingGraph Graph
        {
            get { return (DrawingGraph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        private static void onGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldVal = (DrawingGraph)e.OldValue;
            var newVal = (DrawingGraph)e.NewValue;
            var control = (BaseGraphControl)d;
            control.OnGraphChanged(oldVal, newVal);
        }

        protected virtual void OnGraphChanged(DrawingGraph oldVal, DrawingGraph newVal)
        {
            if (oldVal != null)
            {
                var edges = oldVal.Edges as INotifyCollectionChanged;
                var verteces = oldVal.Verteces as INotifyCollectionChanged;
                if (edges != null)
                    edges.CollectionChanged -= onEdgesCollectionChanged;
                if (verteces != null)
                    verteces.CollectionChanged -= onVertexCollectionChanged;
                clearAllItems();
            }

            if (newVal != null)
            {
                var edges = newVal.Edges as INotifyCollectionChanged;
                var verteces = newVal.Verteces as INotifyCollectionChanged;
                if (edges != null)
                    edges.CollectionChanged += onEdgesCollectionChanged;
                if (verteces != null)
                    verteces.CollectionChanged += onVertexCollectionChanged;
            }
        }

        #endregion

        #region VertexStyle

        public static readonly DependencyProperty VertexStyleProperty = DependencyProperty.Register(
            "VertexStyle", typeof (Style), typeof (BaseGraphControl),
            new PropertyMetadata(onVertexStylePropertyChanged));

        public Style VertexStyle
        {
            get { return (Style) GetValue(VertexStyleProperty); }
            set { SetValue(VertexStyleProperty, value); }
        }

        private static void onVertexStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (Style) e.OldValue;
            var newValue = (Style) e.NewValue;
            var control = (BaseGraphControl) d;
            // TODO: Добавить реализацию.
        }

        #endregion

        private void onVertexCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var vertex in e.NewItems.OfType<IVertex<VertexAttrs, EdgeAttrs>>())
                    addVertex(vertex);
            if (e.OldItems != null)
                foreach (var vertex in e.OldItems)
                    delItem(vertex);
        }

        private void onEdgesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var edge in e.NewItems.OfType<IEdge<VertexAttrs, EdgeAttrs>>())
                    addEdge(edge);
            if (e.OldItems != null)
                foreach (var edge in e.OldItems)
                    delItem(edge);
        }

        private void addEdge(IEdge<VertexAttrs, EdgeAttrs> edge)
        {
            var presenter = new Edge { Content = edge, DataContext = edge };
            GraphPanel.Children.Add(presenter);
            _elementMap.Add(edge, presenter);
        }

        private void addVertex(IVertex<VertexAttrs, EdgeAttrs> vertex)
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

            GraphPanel.Children.Add(presenter);
            // Update desired size of new UIElement.
            presenter.UpdateLayout();
            // Update properties of vertex model.
            vertex.Data.Width = presenter.DesiredSize.Width;
            vertex.Data.Height = presenter.DesiredSize.Height;
            _elementMap.Add(vertex, presenter);
        }

        private void delItem(object item)
        {
            FrameworkElement elt;
            if (_elementMap.TryGetValue(item, out elt))
                GraphPanel.Children.Remove(elt);
        }

        protected T GetTemplateChild<T>(string childName)
            where T : DependencyObject
        {
            var child = (T)GetTemplateChild(childName);
            if (child == null)
                throw new ArgumentOutOfRangeException(string.Format("Template does not contain child <{0}>", childName));
            return child;
        }

        protected void clearAllItems()
        {
            if (GraphPanel == null || Graph == null)
                return;
            GraphPanel.Children.Clear();
            _elementMap.Clear();
        }

        protected void fillVerteces()
        {
            if (GraphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Verteces)
                addVertex(vertex);
        }

        protected void fillEdges()
        {
            if (GraphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Edges)
                addEdge(vertex);
        }

        public abstract void Layout();
    }
}