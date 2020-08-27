using System.Windows;
using System.Windows.Controls;

namespace GraphLight.Controls
{
    public class ItemsControlEx : ItemsControl
    {
#if SILVERLIGHT

        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty
            .Register("ItemContainerStyle", typeof(Style), typeof(ItemsControl), null);

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var lbi = (FrameworkElement)base.GetContainerForItemOverride();
            if (ItemContainerStyle != null)
                lbi.Style = ItemContainerStyle;
            return lbi;
        }

#endif
    }
}