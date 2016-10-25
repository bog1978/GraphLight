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
            _graphPanel = GetTemplateChild<Panel>("graphCanvas");
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
            DependencyProperty.Register("Graph", typeof(IGraph), typeof(BaseGraphControl),
                new PropertyMetadata(onGraphChanged));

        public IGraph Graph
        {
            get { return (IGraph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        private static void onGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
                foreach (var vertex in e.NewItems.OfType<IVertex>())
                    addVertex(vertex);
            if (e.OldItems != null)
                foreach (var vertex in e.OldItems)
                    delItem(vertex);
        }

        private void onEdgesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var edge in e.NewItems.OfType<IEdge>())
                    addEdge(edge);
            if (e.OldItems != null)
                foreach (var edge in e.OldItems)
                    delItem(edge);
        }

        private void addEdge(IEdge edge)
        {
            var presenter = new Edge { Content = edge, DataContext = edge };
            _graphPanel.Children.Add(presenter);
            _elementMap.Add(edge, presenter);
        }

        private void addVertex(IVertex vertex)
        {
            DataTemplate vertexTemplate = null;
            if (VertexTemplateDictionary != null)
            {
                if (string.IsNullOrWhiteSpace(vertex.Category))
                    vertexTemplate = VertexTemplateDictionary.DefaultTemplate;
                else if (!VertexTemplateDictionary.TryGetValue(vertex.Category, out vertexTemplate))
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
            vertex.Width = presenter.DesiredSize.Width;
            vertex.Height = presenter.DesiredSize.Height;
            _elementMap.Add(vertex, presenter);
        }

        private void delItem(object item)
        {
            FrameworkElement elt;
            if (_elementMap.TryGetValue(item, out elt))
                _graphPanel.Children.Remove(elt);
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
            if (_graphPanel == null || Graph == null)
                return;
            _graphPanel.Children.Clear();
            _elementMap.Clear();
        }

        protected void fillVerteces()
        {
            if (_graphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Verteces)
                addVertex(vertex);
        }

        protected void fillEdges()
        {
            if (_graphPanel == null || Graph == null)
                return;
            foreach (var vertex in Graph.Edges)
                addEdge(vertex);
        }

        public abstract void Layout();
    }
}