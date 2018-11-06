using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitSearch.API;
using GitSearch.Models;

namespace GitSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GitHubApi _gitHubApi;
        private IDisposable _subscription;
        private IObservable<List<GitHubUser>> _textChangedObservable;
        public MainWindow()
        {
            InitializeComponent();
            _gitHubApi = new GitHubApi();

            SetupObservable();
        }

        //private void SetupObservable()
        //{
        //    _subscription?.Dispose();

        //    _subscription =
        //        Observable.FromEventPattern<TextChangedEventArgs>(SearchTextBox, "TextChanged")
        //            .Select(args =>
        //            {
        //                TextBox tb = args.EventArgs.Source as TextBox;
        //                string str = tb.Text;
        //                return str;
        //            })
        //            .Throttle(TimeSpan.FromSeconds(1))
        //            .DistinctUntilChanged()
        //            .Select(str =>
        //            {
        //                Console.WriteLine($"Managed Thread Id----{Thread.CurrentThread.ManagedThreadId}");

        //                return _gitHubApi.SearchGitHubUsers(str).SubscribeOn(ThreadPoolScheduler.Instance);
        //            })
        //            .Switch()
        //            .Subscribe(x =>
        //            {
        //                x.ForEach(p => Console.WriteLine(p.Login));
        //            })
        //            ;
        //}        

        private void SetupObservable()
        {
            _subscription?.Dispose();

            _subscription =
                Observable.FromEventPattern<TextChangedEventArgs>(SearchTextBox, "TextChanged")
                    .Select(args => ((TextBox)args.Sender).Text)
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    .DistinctUntilChanged()
                    .Where(str => !string.IsNullOrWhiteSpace(str.Trim()))
                    .Select(str =>
                    {
                        Console.WriteLine($"Managed Thread Id----{Thread.CurrentThread.ManagedThreadId}");

                        return _gitHubApi.SearchGitHubUsersAsync(str);
                    })
                    .Switch()
                    .ObserveOn(Dispatcher)
                    .Subscribe(x => { ListBox.ItemsSource = x; });
        }
    }
}
