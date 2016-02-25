using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphLight.Controls
{
    public partial class DragDropManager
    {
        private static UIElement getDropTarget(UIElement element)
        {
            return VisualTreeHelper
                .FindElementsInHostCoordinates(_options.Current, element)
                .FirstOrDefault(GetAllowDrop);
        }

        partial class DragDropOptions
        {
            private Popup createPopup()
            {
                return new Popup
                {
                    IsHitTestVisible = false,
                    Child = PopupContent ?? new Image
                    {
                        Source = new WriteableBitmap(Source, null),
                        Opacity = 0.5,
                        IsHitTestVisible = false,
                    },
                };
            }
        }
    }
}