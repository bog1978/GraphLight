using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphLight.Graph;
using GraphLight.Layout;
using static System.Math;

namespace GraphLight.Drawing
{
    public class GraphPanel : Panel
    {
        #region Graph

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register(nameof(Graph), typeof(IGraph), typeof(GraphPanel));

        public IGraph? Graph
        {
            get => (IGraph?)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Graph == null)
                return new Size();

            // 1. ��������� ������� ������.
            foreach (UIElement child in Children)
            {
                if (child is VertexControl vertex)
                {
                    vertex.Measure(availableSize);
                    var data = vertex.Data;
                    if (data != null)
                    {
                        data.Width = child.DesiredSize.Width;
                        data.Height = child.DesiredSize.Height;
                    }
                }
            }

            // 2. ��������� �������� ������� �����.
            Layout();

            // NOTE: �����������, ������ ����� ���������� ���� ������ 0.
            var minLeft = Graph.Vertices.Min(x => x.Data.Left);

            // 3. ��������� ������� ����� �����.
            var w = 0.0;
            var h = 0.0;
            foreach (UIElement child in Children)
            {
                switch (child)
                {
                    case VertexControl vertex:
                        {
                            // ����� ������� ����� ���������� ������ ��������� ������, �� �� �� ������.
                            var data = vertex.Data;
                            if (data != null)
                            {
                                w = Max(w, data.Right);
                                h = Max(h, data.Bottom);
                            }
                            break;
                        }
                    case EdgeControl edge:
                        {
                            // ����� ������� ����� ���������� ��������� �����, � ������ � �� ������.
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

            Graph.Width = w;
            Graph.Height = h;

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
                                vertex.Arrange(new Rect(data.Left, data.Top, data.Width, data.Height));
                            break;
                        }
                    case EdgeControl edge:
                        {
                            // ����� ������� ����� ���������� ��������� �����, � ������ � �� ������.
                            var data = edge.Data;
                            if (data != null)
                                edge.Arrange(new Rect(0,0,edge.DesiredSize.Width, edge.DesiredSize.Height));
                            break;
                        }
                }
            }

            return new Size(Graph.Width, Graph.Height);
        }

        public void Layout()
        {
            if (Graph == null)
                return;

            var layout = new GraphVizLayout<IVertexData, IEdgeData>
            {
                NodeMeasure = new DummyNodeMeasure<IVertexData, IEdgeData>(),
                Graph = Graph
            };

            layout.Layout();
        }
    }
}