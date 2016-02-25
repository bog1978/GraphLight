using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Controls;
using GraphLight.Drawing;

namespace GraphLight.Controls
{
    public class DragDropBehaviorBase : Behavior<FrameworkElement>
    {
        #region Поля

        private Point _start;
        private bool _isStarted;
        private const double MIN_DELTA = 0;
        private UIElement _relativeTo;

        #endregion

        private UIElement RelativeTo
        {
            get
            {
                if (_relativeTo == null)
                    _relativeTo = AssociatedObject.GetParent<GraphControl>();
                return _relativeTo;
            }
        }

        #region События

        public delegate void DragDropHandler(object sender, DragDropEventArgs args);

        public event EventHandler<DragDropEventArgs> DragStarted;
        public event EventHandler<DragDropEventArgs> DragFinished;
        public event EventHandler<DragDropEventArgs> Dragging;

        #endregion

        #region Переопределение базовых методов

        protected override void OnAttached()
        {
            AssociatedObject.MouseLeftButtonDown += onMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= onMouseLeftButtonDown;
        }

        #endregion

        #region Обработчики событий мыши

        private void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _start = e.GetPosition(RelativeTo);
            OnDragStarting();
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            var current = e.GetPosition(RelativeTo);
            var dx = current.X - _start.X;
            var dy = current.Y - _start.Y;
            if (!_isStarted)
                OnDragStarted(dx, dy);
            else
                OnDragging(dx, dy);
        }

        private void onMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
        }

        private void onLostMouseCapture(object sender, MouseEventArgs e)
        {
            var current = e.GetPosition(RelativeTo);
            var dx = current.X - _start.X;
            var dy = current.Y - _start.Y;
            OnDragFinished(dx, dy);
        }

        #endregion

        #region Виртуальные методы

        protected virtual bool OnDragStarting()
        {
            var arg = new DragDropEventArgs(0, 0, AssociatedObject.DataContext);
            if (DragStartedCommand != null && !DragStartedCommand.CanExecute(arg))
                return false;

            if (!AssociatedObject.CaptureMouse())
                throw new Exception("Не удалось захватить мышь");

            AssociatedObject.MouseMove += onMouseMove;
            AssociatedObject.LostMouseCapture += onLostMouseCapture;
            AssociatedObject.MouseLeftButtonUp += onMouseLeftButtonUp;

            return true;
        }

        protected virtual void OnDragStarted(double dx, double dy)
        {
            _isStarted = true;
            var arg = new DragDropEventArgs(dx, dy, AssociatedObject.DataContext);
            if (DragStarted != null)
                DragStarted(AssociatedObject, arg);

            if (DragStartedCommand != null && DragStartedCommand.CanExecute(arg))
                DragStartedCommand.Execute(arg);
        }

        protected virtual void OnDragging(double dx, double dy)
        {
            var arg = new DragDropEventArgs(dx, dy, AssociatedObject.DataContext);
            if (Dragging != null)
                Dragging(AssociatedObject, arg);

            if (DraggingCommand != null && DraggingCommand.CanExecute(arg))
                DraggingCommand.Execute(arg);
        }

        protected virtual void OnDragFinished(double dx, double dy)
        {
            AssociatedObject.MouseMove -= onMouseMove;
            AssociatedObject.LostMouseCapture -= onLostMouseCapture;
            AssociatedObject.MouseLeftButtonUp -= onMouseLeftButtonUp;
            _isStarted = false;

            var arg = new DragDropEventArgs(dx, dy, AssociatedObject.DataContext);
            if (_isStarted && DragFinished != null)
                DragFinished(AssociatedObject, arg);

            if (DragFinishedCommand != null && DragFinishedCommand.CanExecute(arg))
                DragFinishedCommand.Execute(arg);
        }

        #endregion

        #region Команды

        public ICommand DragStartedCommand
        {
            get { return (ICommand)GetValue(DragStartedCommandProperty); }
            set { SetValue(DragStartedCommandProperty, value); }
        }

        public static readonly DependencyProperty DragStartedCommandProperty = DependencyProperty
            .Register("DragStartedCommand", typeof(ICommand), typeof(DragDropBehaviorBase), null);

        public ICommand DragFinishedCommand
        {
            get { return (ICommand)GetValue(DragFinishedCommandProperty); }
            set { SetValue(DragFinishedCommandProperty, value); }
        }

        public static readonly DependencyProperty DragFinishedCommandProperty = DependencyProperty
            .Register("DragFinishedCommand", typeof(ICommand), typeof(DragDropBehaviorBase), null);

        public ICommand DraggingCommand
        {
            get { return (ICommand)GetValue(DraggingCommandProperty); }
            set { SetValue(DraggingCommandProperty, value); }
        }

        public static readonly DependencyProperty DraggingCommandProperty = DependencyProperty
            .Register("DraggingCommand", typeof(ICommand), typeof(DragDropBehaviorBase), null);

        #endregion
    }

    public class DragDropEventArgs : EventArgs
    {
        public DragDropEventArgs(double dx, double dy, object dataContext)
        {
            DeltaX = dx;
            DeltaY = dy;
            Payload = dataContext;
        }

        public double DeltaX { get; private set; }

        public double DeltaY { get; private set; }

        public object Payload { get; private set; }
    }

    public class DragDropBehavior : DragDropBehaviorBase
    {
        private double _left;
        private double _top;
        private ContentPresenter _сontainer;
        private ModifierKeys _modifier = ModifierKeys.None;

        private ContentPresenter Container
        {
            get
            {
                if (_сontainer == null)
                    _сontainer = (ContentPresenter)VisualTreeHelper.GetParent(AssociatedObject);
                return _сontainer;
            }
        }

        public ModifierKeys Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        protected override bool OnDragStarting()
        {
            if (Keyboard.Modifiers != _modifier)
                return false;
            var result = base.OnDragStarting();
            if (result)
            {
                _left = Canvas.GetLeft(Container);
                _top = Canvas.GetTop(Container);
            }
            return result;
        }

        protected override void OnDragStarted(double dx, double dy)
        {
            if (Keyboard.Modifiers != _modifier)
                return;
            Canvas.SetLeft(Container, _left + dx);
            Canvas.SetTop(Container, _top + dy);
            base.OnDragStarted(dx, dy);
        }

        protected override void OnDragging(double dx, double dy)
        {
            if (Keyboard.Modifiers != _modifier)
                return; 
            Canvas.SetLeft(Container, _left + dx);
            Canvas.SetTop(Container, _top + dy);
            base.OnDragging(dx, dy);
        }

        protected override void OnDragFinished(double dx, double dy)
        {
            // Это вынести в производный класс.
            Canvas.SetLeft(Container, _left + dx);
            Canvas.SetTop(Container, _top + dy);
            base.OnDragFinished(dx, dy);
        }

    }
}
