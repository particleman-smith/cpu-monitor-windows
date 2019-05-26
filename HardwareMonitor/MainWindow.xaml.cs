using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using OpenHardwareMonitor.Hardware;

namespace HardwareMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TERM_CHAR = "$";

        private string[] ports;
        private SerialPort serial;

        private static int currentCPULoad = 0;
        private static int currentCPUTemp = 0;

        private Thread hwUpdateThread;

        public MainWindow()
        {
            InitializeComponent();

            SetupUI();

            SetupHardwareMonitor();
        }

        private void SetupUI()
        {
            RefreshSerialPorts();
            connectBtn.IsEnabled = false;
            disconnectBtn.IsEnabled = false;
        }

        private void SetupHardwareMonitor()
        {
            hwUpdateThread = new Thread(new ThreadStart(MonitorHardware)) { };
            hwUpdateThread.Start();
        }

        /// <summary>
        /// Gets the system info once every second
        /// </summary>
        private void MonitorHardware()
        {
            while (true)
            {
                GetSystemInfo();

                // Update UI
                mainWindow.Dispatcher.Invoke(new Action(() => UpdateMonitorUI()));

                // Update Arduino
                UpdateSerialDevice();

                Thread.Sleep(1000);
            }
        }

        private void UpdateMonitorUI()
        {
            cpuLoadLabel.Content = currentCPULoad.ToString() + "%";
            cpuTempLabel.Content = currentCPUTemp.ToString() + "°";
        }

        private void UpdateSerialDevice()
        {
            if (serial != null && serial.IsOpen)
            {
                DelegateSendMessage(string.Format("C{0}c{1}", currentCPULoad, currentCPUTemp));
            }
        }

        /// <summary>
        /// Gets all available serial ports and adds them to the list on the UI
        /// </summary>
        private void RefreshSerialPorts()
        {
            for (int i = 0; i < portSelect.Items.Count; i++)
            {
                portSelect.Items.RemoveAt(i);
            }

            ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                portSelect.Items.Add(port);
            }
        }

        private void PortSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serial = new SerialPort((string)portSelect.SelectedItem, 9600, Parity.None, 8, StopBits.One);
            connectBtn.IsEnabled = true;
        }

        private bool TryConnectToArduino()
        {
            try
            {
                serial.Open();
                serial.DataReceived += new SerialDataReceivedEventHandler(Serial_OnDataReceive);
                return true;
            }
            catch (Exception ex)
            {
                //OutputException(ex);
                return false;
            }
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

        private void DelegateSendMessage(string message)
        {
            Thread messageSender = new Thread(new ParameterizedThreadStart(SendMessage));
            messageSender.Start(message);
        }

        private void SendMessage(object text)
        {
            if (serial.IsOpen)
            {
                try
                {
                    // Send the binary data out the port
                    byte[] hexstring = Encoding.ASCII.GetBytes(text + TERM_CHAR); // Append terminating character
                    foreach (byte hexval in hexstring)
                    {
                        byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                                    // to byte[] to write
                        serial.Write(_hexval, 0, 1);
                        Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    //OutputException(ex);
                }
            }
        }

        private void Serial_OnDataReceive(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort sp = (SerialPort)sender;
            string received = sp.ReadExisting();
            //OutputMessage(received);
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            serial.Close();
            portSelect.IsEnabled = true;
            connectBtn.IsEnabled = true;
            disconnectBtn.IsEnabled = false;
        }

        static void GetSystemInfo()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.Accept(updateVisitor);
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        ISensor currentSensor = computer.Hardware[i].Sensors[j];
                        if (currentSensor.SensorType == SensorType.Temperature && currentSensor.Name == "CPU Package")
                            currentCPUTemp = (int)(currentSensor.Value + 0.5);
                        else if (currentSensor.SensorType == SensorType.Load && currentSensor.Name == "CPU Total")
                            currentCPULoad = (int)(currentSensor.Value + 0.5);
                    }
                }
            }
            computer.Close();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serial != null && serial.IsOpen)
                serial.Close();

            hwUpdateThread.Abort();
        }
    }   
}
