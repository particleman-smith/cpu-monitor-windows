using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HardwareMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Monitor hardwareMonitor = new Monitor();
        public MainWindow()
        {
            InitializeComponent();

            SetupUI();

            hardwareMonitor.Start();
        }

        private void SetupUI()
        {
            RefreshSerialPorts();
            connectBtn.IsEnabled = false;
            disconnectBtn.IsEnabled = false;
        }

        private void UpdateMonitorUI()
        {
            cpuLoadLabel.Content = currentCPULoad.ToString() + "%";
            cpuTempLabel.Content = currentCPUTemp.ToString() + "°";
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            serial.Close();
            portSelect.IsEnabled = true;
            connectBtn.IsEnabled = true;
            disconnectBtn.IsEnabled = false;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serial != null && serial.IsOpen)
                serial.Close();

            hwUpdateThread.Abort();
        }
    }   
}
