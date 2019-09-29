using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareMonitor
{
    public static class ArduinoConnector
    {
        const string TERM_CHAR = "$"; // Terminaton character used

        private static SerialPort serial; // The active connection
        private static string[] ports; // Available ports

        public static void UpdateSerialDevice(int currentCPULoad, int currentCPUTemp)
        {
            if (serial != null && serial.IsOpen)
            {
                DelegateSendMessage(string.Format("C{0}c{1}", currentCPULoad, currentCPUTemp));
            }
        }

        private static void DelegateSendMessage(string message)
        {
            Thread messageSender = new Thread(new ParameterizedThreadStart(SendMessage));
            messageSender.Start(message);
        }

        private static void SendMessage(object text)
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

        private static void Serial_OnDataReceive(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort sp = (SerialPort)sender;
            string received = sp.ReadExisting();
            //OutputMessage(received);
        }

        /// <summary>
        /// Gets all available serial ports and adds them to the list on the UI
        /// </summary>
        public static void RefreshSerialPorts()
        {
            // FIXME: Update UI port list
            //for (int i = 0; i < portSelect.Items.Count; i++)
            //{
            //    portSelect.Items.RemoveAt(i);
            //}

            ports = SerialPort.GetPortNames();

            // FIXME: Update UI port list
            //foreach (string port in ports)
            //{
            //    portSelect.Items.Add(port);
            //}
        }

        private static bool TryConnectToArduino()
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
    }  
}
