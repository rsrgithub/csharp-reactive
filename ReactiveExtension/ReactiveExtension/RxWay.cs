using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactiveExtension
{
    public class RxWay
    {
        private readonly UIElement _target;
        private readonly IInputElement _container;
        private IDisposable _subDisposable;
        private IObservable<Point> _dragObservable;

        public RxWay(UIElement target, IInputElement container)
        {
            _target = target;
            _container = container;

            SetupObservables();
        }

        private void SetupObservables()
        {
            var mouseMoveObservable = Observable.FromEventPattern<MouseEventArgs>(_container, "MouseMove")
                                                .Select(x => x.EventArgs.GetPosition(_container));

            var mouseUpObservable = Observable.FromEventPattern<MouseEventArgs>(_container, "MouseLeftButtonUp");

            var mouseLeaveObservable = Observable.FromEventPattern<MouseEventArgs>(_container, "MouseLeave");

            _dragObservable = Observable.FromEventPattern<MouseEventArgs>(_target, "MouseLeftButtonDown")
                                        .Select(x => x.EventArgs.GetPosition(_target))
                                        .SelectMany(mouseDown =>
                                        {
                                            return mouseMoveObservable.TakeUntil(mouseUpObservable.Merge(mouseLeaveObservable))
                                                .Select(mouseMove => new Point(mouseMove.X - mouseDown.X, mouseMove.Y - mouseDown.Y));
                                        });
        }

        public void Subscribe()
        {                                   
            _subDisposable = _dragObservable.Subscribe(value =>
            {
                Canvas.SetLeft(_target, value.X);
                Canvas.SetTop(_target, value.Y);
            });
        }

        public void Dispose()
        {
            _subDisposable?.Dispose();
        }
    }
}