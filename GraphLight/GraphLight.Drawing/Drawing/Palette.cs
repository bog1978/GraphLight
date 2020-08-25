using System.Linq;
using GraphLight.Controls;
using GraphLight.Graph;

namespace GraphLight.Drawing
{
    public class Palette : BaseGraphControl
    {
        public Palette()
        {
            DefaultStyleKey = typeof(Palette);
            DragDropManager.AddDragQueryHandler(this, OnDragQuery);
        }

        private bool OnDragQuery(IDragDropOptions arg) => true;

        protected override void OnGraphChanged(IGraph oldVal, IGraph newVal)
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
            Graph.Width = Graph.Vertices.Max(x => x.Width);
            Graph.Height = Graph.Vertices.Sum(x => x.Height);
            var top = 0.0;
            foreach (var vertex in Graph.Vertices)
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
