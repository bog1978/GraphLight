using System.Windows;
using System.Windows.Input;
using GraphLight.Controls;
using GraphLight.Geometry;

namespace GraphLight.ViewModel
{
    public abstract class GraphTool
    {
        protected GraphTool(GraphViewModel model)
        {
            Model = model;
            IsInProgress = false;
        }

        public virtual void HandleLButtonUp(object sender, MouseButtonEventArgs e) { }
        public virtual void HandleLButtonDown(object sender, MouseButtonEventArgs e) { }
        public virtual void HandleMouseMove(object sender, MouseEventArgs e) { }
        public virtual void HandleKeyUp(object sender, KeyEventArgs e) { }
        public virtual void Cancel() { }

        protected GraphViewModel Model { get; private set; }

        protected T GetOriginalDataContext<T>(MouseEventArgs e) where T : class
        {
            return e.OriginalSource.GetDataContext<T>();
        }

        protected Point2D GetPoint(MouseEventArgs e)
        {
            var element = (UIElement)e.OriginalSource;
            var point = e.GetPosition(element);
            return new Point2D(point.X, point.Y);
        }

        public bool IsInProgress { get; protected set; }
    }
}