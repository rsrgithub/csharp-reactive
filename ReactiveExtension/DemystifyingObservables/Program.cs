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
            var myObservable = new Observable((observer) =>
            {
                var dataSource = new DataSource();                
                var safeObs = new SafeObserver(observer);
                dataSource.OnData = o =>  safeObs.Next(o);
                dataSource.OnError = (err) => safeObs.Error(err);
                dataSource.OnComplete = () => safeObs.Complete();
                safeObs.Unsub = () => dataSource.Destroy();
                return safeObs.Unsubscribe;
            });



            myObservable.Subscribe(new MyObserver());

            Console.ReadKey();
        }        
    }

    public class MyObserver : IObserver
    {
        public void Next(object value)
        {
            Console.WriteLine(value);
        }

        public void Error(object error)
        {
            Console.WriteLine(error);
        }

        public void Complete()
        {
            Console.WriteLine("Complete!");
        }
    }

    public class DataSource
    {
        private readonly Timer _timer;
        public Action<object> OnData;
        public Action<object> OnError;
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

    public interface IObserver
    {
        void Next(object value);
        void Error(object error);
        void Complete();
    }

    public class SafeObserver
    {
        private bool _isUnsubscribed;
        public Action Unsub;
        private readonly IObserver _target;
        public Action Unsubscribe;

        public SafeObserver(IObserver target)
        {
            _target = target;
            Unsubscribe = () =>
            {
                _isUnsubscribed = true;

                Unsub();
            };
        }

        public void Next(object value)
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

        public void Error(object err)
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

    public class Observable
    {
        private Func<IObserver, object> _observer;
        public Observable(Func<IObserver, object> observer)
        {
            _observer = observer;
        }

        public object Subscribe(IObserver observer)
        {
            return _observer(observer);
        }
    }
}
