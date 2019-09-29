using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HardwareMonitor.Views
{
    /// <summary>
    /// Interaction logic for TaskbarMainView.xaml
    /// </summary>
    public partial class TaskbarMainView : UserControl
    {
        public TaskbarMainView()
        {
            InitializeComponent();
        }

        private void PortSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serial = new SerialPort((string)portSelect.SelectedItem, 9600, Parity.None, 8, StopBits.One);
            connectBtn.IsEnabled = true;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            bool connectSuccess = TryConnectToArduino();
            if (connectSuccess)
            {
                portSelect.IsEnabled = false;
                connectBtn.IsEnabled = false;
                disconnectBtn.IsEnabled = true;
            }
        }


    }
}
