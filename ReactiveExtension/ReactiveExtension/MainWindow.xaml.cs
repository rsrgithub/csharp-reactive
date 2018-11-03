using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
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

namespace ReactiveExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private RxWay _rxWay;
        private ImperativeWay _imperativeWay;

        public MainWindow()
        {
            InitializeComponent();
        }        

        private void OnRxWayClick(object sender, RoutedEventArgs e)
        {
            _imperativeWay?.Dispose();

            if (_rxWay == null)
            {
                _rxWay = new RxWay(Border, Container);
            }

            _rxWay.Subscribe();
        }

        private void OnImperativeWayClick(object sender, RoutedEventArgs e)
        {
            _rxWay?.Dispose();

            if (_imperativeWay == null)
            {
                _imperativeWay = new ImperativeWay(Border, Container);
            }

            _imperativeWay.Subscribe();
            //e.Handled = true;
        }
    }
}
