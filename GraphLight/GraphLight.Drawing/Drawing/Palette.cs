using System.Linq;
using GraphLight.Controls;

namespace GraphLight.Drawing
{
    public class Palette : BaseGraphControl
    {
        public Palette()
        {
            DefaultStyleKey = typeof(Palette);
            DragDropManager.AddDragQueryHandler(this, OnDragQuery);
        }

        private bool OnDragQuery(IDragDropOptions arg)
        {
            return true;
        }

        protected override void OnGraphChanged(Graph.DrawingGraph oldVal, Graph.DrawingGraph newVal)
        {
            base.OnGraphChanged(oldVal, newVal);
            Layout();
        }

        public override void Layout()
        {
            if (!_isLoaded || Graph == null)
                return;
            clearAllItems();
            fillVerteces();
            Graph.Width = Graph.Verteces.Max(x => x.Data.Width);
            Graph.Height = Graph.Verteces.Sum(x => x.Data.Height);
            var top = 0.0;
            foreach (var vertex in Graph.Verteces)
            {
                vertex.Data.Left = (Graph.Width - vertex.Data.Width) / 2;
                vertex.Data.Top = top;
                top += vertex.Data.Height;
            }
            Width = Graph.Width;
            Height = Graph.Height;
        }
    }
}
