using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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

namespace RxConcurrency
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDisposable _subscription;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnProcess(object sender, RoutedEventArgs e)
        {            
            _subscription?.Dispose();

            ResultTextBox.Clear();
            var enumerable = Enumerable.Range(int.Parse(StartTextBox.Text), int.Parse(CountTextBox.Text))
                                        .Select(x =>
                                        {
                                            Thread.Sleep(50);
                                            return x;
                                        })
                                        .ToObservable();

            _subscription = enumerable.SubscribeOn(ThreadPoolScheduler.Instance).ObserveOn(Dispatcher).Subscribe((x) =>
            {
                ResultTextBox.Text = $"{ResultTextBox.Text}\r\n{x}".Trim();
                ResultTextBox.ScrollToEnd();
            });
        }
    }
}
