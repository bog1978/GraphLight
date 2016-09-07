using System.Diagnostics;
using System.Linq;

namespace GraphLight.Graph
{
    [DebuggerDisplay("{DebugString}")]
    public class DrawingVertex : Vertex<VertexAttrs, EdgeAttrs>
    {
        private string _shapeData;

        #region Constructors

        public DrawingVertex(VertexAttrs data)
            : base(data)
        {
            Data = data;
            ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1"; // Эллипс
            //ShapeData = "M 0,0 H 1 V 1 H 0 Z"; // Прямоугольник.
        }

        #endregion

        public string DebugString
        {
            get { return Data.ToString(); }
        }

        public double CenterY
        {
            get { return Top + Height / 2; }
        }

        public string ShapeData
        {
            get { return _shapeData; }
            set { SetProperty(ref _shapeData, value, "ShapeData"); }
        }

        public void Update()
        {
            foreach (DrawingEdge e in Edges)
            {
                var pts = e.Points;
                using (e.DeferRefresh())
                {
                    if (pts.Count == 2 || e.Src == e.Dst)
                    {
                        e.UpdateSrcPort();
                        e.UpdateDstPort();
                    }
                    else if (e.Src == this)
                        e.UpdateSrcPort();
                    else
                        e.UpdateDstPort();
                    var first = e.Points.First();
                    var last = e.Points.Last();
                    e.FixDraggablePoints(first);
                    e.FixDraggablePoints(last);
                }
            }
        }
    }
}