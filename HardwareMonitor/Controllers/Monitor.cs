using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace HardwareMonitor
{
    public class Monitor
    {
        private Thread hwUpdateThread;
        HardwareInfo hwInfo = new HardwareInfo();

        public Monitor()
        {
            hwUpdateThread = new Thread(new ThreadStart(MonitorHardware)) { };
        }

        public void Start()
        {
            ArduinoConnector.RefreshSerialPorts();
            hwUpdateThread.Start();
        }

        /// <summary>
        /// Gets the system info once every second
        /// </summary>
        private void MonitorHardware()
        {
            while (true)
            {
                hwInfo.RefreshSystemInfo();

                // FIXME: Update UI
                //mainWindow.Dispatcher.Invoke(new Action(() => UpdateMonitorUI()));

                // Update Arduino
                ArduinoConnector.UpdateSerialDevice(hwInfo.CurrentCPULoad, hwInfo.CurrentCPUTemp);

                Thread.Sleep(1000);
            }
        }
    }
}
