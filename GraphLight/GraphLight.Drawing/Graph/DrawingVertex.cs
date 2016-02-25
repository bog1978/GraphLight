using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace GraphLight.Graph
{
    [DebuggerDisplay("{DebugString}")]
    public class DrawingVertex : Vertex<VertexAttrs, EdgeAttrs>, INotifyPropertyChanged
    {
        #region Constructors

        public DrawingVertex(VertexAttrs data)
            : base(data)
        {
            base.Data = data;
            ShapeData = "M 0,1 A 1,1 0 1 0 2,1 A 1,1 0 1 0 0,1"; // Эллипс
            //ShapeData = "M 0,0 H 1 V 1 H 0 Z"; // Прямоугольник.
        }

        #endregion

        public string DebugString
        {
            get { return Id; }
        }

        public double CenterY
        {
            get { return Data.Top + Data.Height/2; }
        }

        public string ShapeData
        {
            get { return Data.ShapeData; }
            set
            {
                Data.ShapeData = value;
                RaisePropertyChanged("ShapeData");
            }
        }

        public override string Id
        {
            get { return Data != null ? Data.Id : null; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void Update()
        {
            foreach (DrawingEdge e in Edges)
            {
                var pts = e.Data.Points;
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
                    var first = e.Data.Points.First();
                    var last = e.Data.Points.Last();
                    e.FixDraggablePoints(first);
                    e.FixDraggablePoints(last);
                }
            }
        }

        protected override ICollection<T> CreateCollection<T>()
        {
            return new ObservableCollection<T>();
        }
    }
}