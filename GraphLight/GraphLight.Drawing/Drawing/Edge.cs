using System.Windows;
using System.Windows.Controls;

namespace GraphLight.Drawing
{
    public class Edge : ContentControl
    {
        public Edge()
        {
            DefaultStyleKey = typeof (Edge);
        }

        #region IsSelected

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty
            .Register("IsSelected", typeof(bool), typeof(Edge), null);

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        #endregion

        #region IsHighlighted

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty
            .Register("IsHighlighted", typeof(bool), typeof(Edge), null);

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        #endregion
    }
}