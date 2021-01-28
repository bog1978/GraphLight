using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphLight.Drawing
{
    public class NodeControl : ContentControl
    {
        public NodeControl()
        {
            DefaultStyleKey = typeof(NodeControl);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseEnter", true);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseLeave", true);
            base.OnMouseLeave(e);
        }

        #region IsSelected

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty
            .Register("IsSelected", typeof(bool), typeof(NodeControl), null);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion

        #region IsHighlighted

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty
            .Register("IsHighlighted", typeof(bool), typeof(NodeControl), null);

        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        #endregion
    }
}