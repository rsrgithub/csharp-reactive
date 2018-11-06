using System.Windows;

namespace RxDragDrop
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
