using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemystifyingObservables
{
    class Program
    {
        static void Main(string[] args)
        {
            var num = 0;
            var myObservable = new Observable<int>((observer) =>
            {
                var dataSource = new DataSource();
                var safeObs = new SafeObserver<int>(observer);
                dataSource.OnData = o => safeObs.Next(o);
                dataSource.OnError = (err) => safeObs.Error(err);
                dataSource.OnComplete = () => safeObs.Complete();
                safeObs.Unsub = () => dataSource.Destroy();
                return safeObs.Unsubscribe;
            });



            var subscription = myObservable.Select(x => x * 2).Subscribe(new MyObserver<int>
            {
                Next = Console.WriteLine,
                Error = Console.WriteLine,
                Complete = () => Console.WriteLine("Complete")
            });            
            
            Console.ReadKey();
        }
    }

    public class MyObserver<T> : IObserver<T>
    {
        public Action<T> Next { get; set; }

        public Action<T> Error { get; set; }

        public Action Complete { get; set; }
    }

    public class DataSource
    {
        private readonly Timer _timer;
        public Action<int> OnData;
        public Action<int> OnError;
        public Action OnComplete;

        public DataSource()
        {
            int i = 0;
            _timer = new Timer(state => Emit(i++), null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private void Emit(int i)
        {
            const int limit = 10;

            OnData?.Invoke(i);

            if (i == limit)
            {
                OnComplete?.Invoke();
                Destroy();
            }
        }

        public void Destroy()
        {
            _timer.Dispose();
        }
    }

    public interface IObserver<T>
    {
        Action<T> Next { get; set; }

        Action<T> Error { get; set; }

        Action Complete { get; set; }
    }

    public class SafeObserver<T>
    {
        private bool _isUnsubscribed;
        public Action Unsub;
        private readonly IObserver<T> _target;
        public Action Unsubscribe;

        public SafeObserver(IObserver<T> target)
        {
            _target = target;
            Unsubscribe = () =>
            {
                _isUnsubscribed = true;

                Unsub();
            };
        }

        public void Next(T value)
        {
            if (!_isUnsubscribed)
            {
                try
                {
                    _target.Next(value);
                }
                catch (Exception e)
                {
                    Unsubscribe();
                    throw;
                }
            }
        }

        public void Error(T err)
        {
            if (!_isUnsubscribed)
            {
                try
                {
                    _target.Error(err);
                }
                catch (Exception e)
                {
                    Unsubscribe();
                    throw;
                }
            }

            Unsubscribe();
        }

        public void Complete()
        {
            if (!_isUnsubscribed)
            {
                try
                {
                    _target.Complete();
                }
                catch (Exception e)
                {
                    Unsubscribe();
                    throw;
                }
            }

            Unsubscribe();
        }
    }

    public class Observable<T>
    {
        private Func<IObserver<T>, Action> _observer;
        public Observable(Func<IObserver<T>, Action> observer)
        {
            _observer = observer;
        }

        public Action Subscribe(IObserver<T> observer)
        {
            return _observer(observer);
        }


        public Observable<T> Select(Func<T, T> projectFunc)
        {
            return new Observable<T>((observer) =>
            {
                IObserver<T> newObserver = new MyObserver<T>
                {
                    Next = (v) => observer.Next(projectFunc(v)),
                    Error = (e) => observer.Error(e),
                    Complete = () => observer.Complete()
                };

                return Subscribe(newObserver);
            });
        }
    }
}
