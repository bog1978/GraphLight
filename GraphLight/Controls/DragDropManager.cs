using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphLight.Controls
{
    public partial class DragDropManager
    {
        private static readonly IDictionary<FrameworkElement, Subscription> _subscriptions;
        private static readonly MouseButtonEventHandler _mouseLeftButtonDownEventHandler;
        private static DragDropOptions _options;

        static DragDropManager()
        {
            _subscriptions = new Dictionary<FrameworkElement, Subscription>();
            _mouseLeftButtonDownEventHandler = onMouseLeftButtonDown;
        }

        #region AllowDrag

        public static readonly DependencyProperty AllowDragProperty = DependencyProperty.RegisterAttached(
            "AllowDrag", typeof(bool), typeof(DragDropManager),
            new PropertyMetadata(false, onAllowDragPropertyChanged));

        public static void SetAllowDrag(DependencyObject element, bool value)
        {
            element.SetValue(AllowDragProperty, value);
        }

        public static bool GetAllowDrag(DependencyObject element)
        {
            return (bool)element.GetValue(AllowDragProperty);
        }

        private static void onAllowDragPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AllowDrop

        public static readonly DependencyProperty AllowDropProperty = DependencyProperty.RegisterAttached(
            "AllowDrop", typeof(bool), typeof(DragDropManager),
            new PropertyMetadata(false, onAllowDropPropertyChanged));

        public static void SetAllowDrop(DependencyObject element, bool value)
        {
            element.SetValue(AllowDropProperty, value);
        }

        public static bool GetAllowDrop(DependencyObject element)
        {
            return (bool)element.GetValue(AllowDropProperty);
        }

        private static void onAllowDropPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region KeyboardModifiers

        public static readonly DependencyProperty KeyboardModifiersProperty = DependencyProperty.RegisterAttached(
            "KeyboardModifiers", typeof(ModifierKeys), typeof(DragDropManager),
            new PropertyMetadata(ModifierKeys.None, onKeyboardModifiersPropertyPropertyChanged));

        public static void SetKeyboardModifiers(DependencyObject element, ModifierKeys value)
        {
            element.SetValue(KeyboardModifiersProperty, value);
        }

        public static ModifierKeys GetKeyboardModifiers(DependencyObject element)
        {
            return (ModifierKeys)element.GetValue(KeyboardModifiersProperty);
        }

        private static void onKeyboardModifiersPropertyPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region Mode

        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(
            "Mode", typeof(DragDropMode), typeof(DragDropManager),
            new PropertyMetadata(DragDropMode.DragExisting, onModePropertyPropertyChanged));

        public static void SetMode(DependencyObject element, DragDropMode value)
        {
            element.SetValue(ModeProperty, value);
        }

        public static DragDropMode GetMode(DependencyObject element)
        {
            return (DragDropMode)element.GetValue(ModeProperty);
        }

        private static void onModePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        public static void AddDragQueryHandler(FrameworkElement element, Func<IDragDropOptions, bool> handler)
        {
            if (!_subscriptions.TryGetValue(element, out var subscription))
            {
                subscription = new Subscription(element);
                _subscriptions.Add(element, subscription);
            }
            subscription.DragQuery = handler;
            element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler);
            element.AddHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler, true);
        }

        public static void AddDropQueryHandler(FrameworkElement element, Func<IDragDropOptions, bool> handler)
        {
            if (!_subscriptions.TryGetValue(element, out var subscription))
            {
                subscription = new Subscription(element);
                _subscriptions.Add(element, subscription);
            }
            subscription.DropQuery = handler;
            element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler);
            element.AddHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler, true);
        }

        public static void AddDropInfoHandler(FrameworkElement element, Action<IDragDropOptions> handler)
        {
            if (!_subscriptions.TryGetValue(element, out var subscription))
            {
                subscription = new Subscription(element);
                _subscriptions.Add(element, subscription);
            }
            subscription.DropInfo = handler;
            element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler);
            element.AddHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler, true);
        }

        public static void RemoveAllHandlers(FrameworkElement element)
        {
            if (!_subscriptions.TryGetValue(element, out var subscription))
                return;
            subscription.DropQuery = null;
            subscription.DropInfo = null;
            _subscriptions.Remove(element);
            element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler);
        }

        /// <summary>
        /// Find nearest element in visual tree labeled with AllowDrag=true.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static FrameworkElement getDragSource(object obj)
        {
            var element = obj as FrameworkElement;
            if (element == null || GetAllowDrop(element))
                return null;
            if (GetAllowDrag(element))
                return element;
            return getDragSource(VisualTreeHelper.GetParent(element));
        }

        private static Subscription getSubscription(object key) =>
            key is FrameworkElement element && _subscriptions.TryGetValue(element, out var subscription)
                ? subscription
                : null;

        #region Mouse handlers

        private static void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var subscription = getSubscription(sender);
            if (subscription == null)
                return;

            var sourceElement = getDragSource(e.OriginalSource);
            if (sourceElement == null)
                return;

            var modifierKeys = GetKeyboardModifiers(sourceElement);
            if (modifierKeys != Keyboard.Modifiers)
                return;

            var start = e.GetPosition(null);
            var options = new DragDropOptions(sourceElement, start);

            // Query if drag is possible
            if (subscription.DragQuery != null)
                if (!subscription.DragQuery(options))
                    return;

            if (!sourceElement.CaptureMouse())
                throw new Exception("Не удалось захватить мышь");

            foreach (var element in _subscriptions.Keys)
            {
                element.MouseMove += onMouseMove;
                element.LostMouseCapture += onLostMouseCapture;
                element.MouseLeftButtonUp += onMouseLeftButtonUp;
            }

            if (GetMode(sourceElement) == DragDropMode.DragCopy)
            {
                options.UpdatePopupLocation();
                options.Popup.IsOpen = true;
            }
            _options = options;
        }

        private static void onMouseMove(object sender, MouseEventArgs e)
        {
            _options.Status = DragDropStatus.Dragging;
            publishDropInfo(sender, e);
        }

        private static void onMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as UIElement;
            if (element != null)
                element.ReleaseMouseCapture();
        }

        private static void onLostMouseCapture(object sender, MouseEventArgs e)
        {
            foreach (var element in _subscriptions.Keys)
            {
                element.MouseMove -= onMouseMove;
                element.LostMouseCapture -= onLostMouseCapture;
                element.MouseLeftButtonUp -= onMouseLeftButtonUp;
            }
            _options.Status = DragDropStatus.Completed;
            publishDropInfo(sender, e);
            _options.Popup.IsOpen = false;
            _options = null;
        }

        private static void publishDropInfo(object sender, MouseEventArgs e)
        {
            var current = e.GetPosition(null);
            _options.Current = current;

            if (_options.Mode == DragDropMode.DragCopy)
                _options.UpdatePopupLocation();

            var target = getDropTarget(_options.VisualTreeRoot);
            if (target == null || !GetAllowDrop(target))
                return;

            var subscription = getSubscription(target);
            if (subscription == null)
                return;

            if (subscription.DropQuery != null && !subscription.DropQuery(_options))
                return;

            _options.Target = (FrameworkElement)target;
            _options.Relative = e.GetPosition(subscription.Subscriber);
            subscription.DropInfo(_options);
        }

        #endregion

        #region Nested type: DragDropOptions

        private sealed partial class DragDropOptions : IDragDropOptions
        {
            private Popup _popup;
            private FrameworkElement _source;
            public UIElement VisualTreeRoot { get; private set; }


            public DragDropOptions(FrameworkElement source, Point start)
            {
                Source = source;
                Start = start;
                Current = start;
                Status = DragDropStatus.NotStarted;
            }

            public DragDropStatus Status { get; internal set; }

            internal Popup Popup => _popup ?? (_popup = createPopup());

            public UIElement PopupContent { get; set; }

            #region IDragDropOptions Members

            public FrameworkElement Source
            {
                get => _source;
                private set
                {
                    _source = value;
                    VisualTreeRoot = value.GetVisualTreeRoot();
                }
            }

            public FrameworkElement Target { get; internal set; }
            public object Payload { get; set; }
            public Point Start { get; }
            public Point Current { get; internal set; }
            public Point Relative { get; internal set; }
            public DragDropMode Mode => GetMode(Source);

            public double DeltaX => Current.X - Start.X;

            public double DeltaY => Current.Y - Start.Y;

            #endregion

            internal void UpdatePopupLocation()
            {
                Popup.HorizontalOffset = Current.X - Source.ActualWidth / 2;
                Popup.VerticalOffset = Current.Y - Source.ActualWidth / 2;
            }

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

        #endregion

        #region Nested type: Subscription

        private sealed class Subscription
        {
            public readonly FrameworkElement Subscriber;
            public Func<IDragDropOptions, bool> DragQuery;
            public Action<IDragDropOptions> DropInfo;
            public Func<IDragDropOptions, bool> DropQuery;

            public Subscription(FrameworkElement subscriber)
            {
                Subscriber = subscriber;
            }
        }

        #endregion

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
    }

    public interface IDragDropOptions
    {
        FrameworkElement Source { get; }
        FrameworkElement Target { get; }
        object Payload { get; set; }
        Point Start { get; }
        Point Current { get; }
        Point Relative { get; }
        double DeltaX { get; }
        double DeltaY { get; }
        UIElement PopupContent { get; set; }
        DragDropStatus Status { get; }
        DragDropMode Mode { get; }
    }

    public enum DragDropStatus
    {
        NotStarted,
        Dragging,
        Completed,
    }

    public enum DragDropMode
    {
        DragExisting,
        DragCopy,
    }
}