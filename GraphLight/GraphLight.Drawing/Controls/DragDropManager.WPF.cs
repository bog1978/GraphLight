using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphLight.Controls
{
    public partial class DragDropManager
    {
        private static UIElement getDropTarget(UIElement element)
        {
            DependencyObject hit = null;
            VisualTreeHelper.HitTest(element,
                target => GetAllowDrop(target)
                    ? HitTestFilterBehavior.ContinueSkipChildren
                    : HitTestFilterBehavior.ContinueSkipSelf,
                hitTestResult =>
                {
                    hit = hitTestResult.VisualHit;
                    return HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(_options.Current));
            return (UIElement)hit;
        }

        partial class DragDropOptions
        {
            private Popup createPopup()
            {
                return new Popup
                {
                    IsHitTestVisible = false,
                    AllowsTransparency = true,
                    PlacementTarget = Source.GetVisualTreeRoot(),
                    Placement = PlacementMode.Relative,
                    Child = PopupContent ?? new Rectangle
                    {
                        Width = Source.ActualWidth,
                        Height = Source.ActualHeight,
                        Fill = new VisualBrush { Visual = Source },
                        Opacity = 0.5,
                    },
                };
            }
        }
    }
}