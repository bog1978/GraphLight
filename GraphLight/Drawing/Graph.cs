using GraphLight.Graph;
using GraphLight.Layout;

namespace GraphLight.Drawing
{
    public class GraphControl : BaseGraphControl
    {
        #region Private fields

        private readonly GraphVizLayout<IVertexData, IEdgeData> _layout;
        private readonly DummyNodeMeasure<IVertexData, IEdgeData> _measure;

        #endregion

        #region Initialization

        public GraphControl()
        {
            DefaultStyleKey = typeof(GraphControl);
            _measure = new DummyNodeMeasure<IVertexData, IEdgeData>();
            _layout = new GraphVizLayout<IVertexData, IEdgeData> { NodeMeasure = _measure };
        }

        #endregion

        #region API

        public override void Layout()
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

        protected override void OnGraphChanged(IGraph oldVal, IGraph newVal)
        {
            base.OnGraphChanged(oldVal, newVal);
            if (newVal != null)
                Layout();
        }

        #endregion

        private void Shift()
        {
            var minX = double.MaxValue;
            var maxX = double.MinValue;
            var minY = double.MaxValue;
            var maxY = double.MinValue;

            foreach (var vertex in Graph.Vertices)
            {
                if (vertex.Data.Left < minX)
                    minX = vertex.Data.Left;
                if (vertex.Data.Right > maxX)
                    maxX = vertex.Data.Right;
                if (vertex.Data.Top < minY)
                    minY = vertex.Data.Top;
                if (vertex.Data.Bottom > maxY)
                    maxY = vertex.Data.Bottom;
            }

            foreach (var edge in Graph.Edges)
            foreach (var point2D in edge.Data.Points)
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
                vertex.Data.Left -= minX;
                vertex.Data.Top -= minY;
            }

            foreach (var edge in Graph.Edges)
            foreach (var point2D in edge.Data.Points)
            {
                point2D.X -= minX;
                point2D.Y -= minY;
            }

            Graph.Width = graphWidth;
            Graph.Height = graphHeight;
        }
    }
}