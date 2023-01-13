using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Algorithm;
using GraphLight.Graph;
using static System.Math;

namespace GraphLight.Drawing
{
    public class GraphPanel : Panel
    {
        #region Graph

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register(nameof(Graph), typeof(IGraph<IGraphData, IVertexData, IEdgeData>), typeof(GraphPanel));

        public IGraph<IGraphData, IVertexData, IEdgeData>? Graph
        {
            get => (IGraph<IGraphData, IVertexData, IEdgeData>?)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Graph == null)
                return new Size();

            // 1. Вычисляем размеры вершин.
            foreach (UIElement child in Children)
            {
                if (child is VertexControl vertex)
                {
                    vertex.Measure(availableSize);
                    var data = vertex.Data;
                    data?.Rect.SetSize(child.DesiredSize.FromWpf());
                }
            }

            // 2. Применяем алгоритм укладки графа.
            Layout();

            // NOTE: Разобраться, почему здесь получается чуть меньше 0.
            var minLeft = Graph.Vertices.Min(x => x.Data.Rect.Left);

            // 3. Вычисляем размеры всего графа.
            var w = 0.0;
            var h = 0.0;
            foreach (UIElement child in Children)
            {
                switch (child)
                {
                    case VertexControl vertex:
                        {
                            // После укладки графа изменилось только положение вершин, но не их размер.
                            var data = vertex.Data;
                            if (data != null)
                            {
                                w = Max(w, data.Rect.Right);
                                h = Max(h, data.Rect.Bottom);
                            }
                            break;
                        }
                    case EdgeControl edge:
                        {
                            // После укладки графа изменилось геометрия ребер, а значит и их размер.
                            edge.Measure(availableSize);
                            var data = edge.Data;
                            if (data != null)
                            {
                                w = Max(w, edge.DesiredSize.Width);
                                h = Max(h, edge.DesiredSize.Height);
                            }
                            break;
                        }
                }
            }

            Graph.Data.Width = w;
            Graph.Data.Height = h;

            return new Size(w, h);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Graph == null)
                return finalSize;
            foreach (UIElement child in Children)
            {
                switch (child)
                {
                    case VertexControl vertex:
                        {
                            var data = vertex.Data;
                            if (data != null)
                                vertex.Arrange(data.Rect.ToWpf());
                            break;
                        }
                    case EdgeControl edge:
                        {
                            // После укладки графа изменилось геометрия ребер, а значит и их размер.
                            var data = edge.Data;
                            if (data != null)
                                edge.Arrange(new Rect(0,0,edge.DesiredSize.Width, edge.DesiredSize.Height));
                            break;
                        }
                }
            }

            return new Size(Graph.Data.Width, Graph.Data.Height);
        }

        public void Layout()
        {
            if (Graph == null)
                return;

            var layout = new GraphVizLayout
            {
                NodeMeasure = new DummyNodeMeasure(),
                Graph = Graph
            };

            layout.Layout();
        }
    }
}