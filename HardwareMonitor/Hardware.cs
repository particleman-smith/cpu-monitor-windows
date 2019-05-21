using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareMonitor
{
    public class Hardware
    {
        private SerialPort _serialPort = null;

        public Hardware()
        {
            
        }

        public Hardware(string portName, int baudRate)
        {
            SetupPort(portName, baudRate);
        }

        public void SetupPort(string portName, int baudRate)
        {
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(portName, baudRate);
            }
            else
            {
                _serialPort.PortName = portName;
                _serialPort.BaudRate = baudRate;
            }
        }

        public string[] getPortNames()
        {
            return SerialPort.GetPortNames();
        }
    }
}
