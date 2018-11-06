using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RxDragDrop
{
    public class ImperativeWay
    {
        private readonly UIElement _target;
        private readonly IInputElement _container;
        private Point _mouseDownPoint;

        public ImperativeWay(UIElement target, IInputElement container)
        {
            _target = target;
            _container = container;

        }

        public void Subscribe()
        {
            _target.MouseLeftButtonDown += MouseDown;
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisposeExceptMouseDown();
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            DisposeExceptMouseDown();
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {            
            Point mouseMovePoint = e.GetPosition(_container);

            Canvas.SetLeft(_target, mouseMovePoint.X - _mouseDownPoint.X);
            Canvas.SetTop(_target, mouseMovePoint.Y - _mouseDownPoint.Y);
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterForEvents();
            _mouseDownPoint = e.GetPosition(_target);
        }

        private void DisposeExceptMouseDown()
        {
            _container.MouseMove -= MouseMove;
            _container.MouseLeave -= MouseLeave;
            _container.MouseLeftButtonUp -= MouseLeftButtonUp;
        }

        private void RegisterForEvents()
        {
            _container.MouseMove += MouseMove;
            _container.MouseLeave += MouseLeave;
            _container.MouseLeftButtonUp += MouseLeftButtonUp;
        }


        public void Dispose()
        {
            _target.MouseLeftButtonDown -= MouseDown;
            _container.MouseMove -= MouseMove;
            _container.MouseLeave -= MouseLeave;
            _container.MouseLeftButtonUp -= MouseLeftButtonUp;
        }
    }
}
