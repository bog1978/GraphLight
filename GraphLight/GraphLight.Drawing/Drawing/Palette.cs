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
            Graph.Width = Graph.Verteces.Max(x => x.Width);
            Graph.Height = Graph.Verteces.Sum(x => x.Height);
            var top = 0.0;
            foreach (var vertex in Graph.Verteces)
            {
                vertex.Left = (Graph.Width - vertex.Width) / 2;
                vertex.Top = top;
                top += vertex.Height;
            }
            Width = Graph.Width;
            Height = Graph.Height;
        }
    }
}
