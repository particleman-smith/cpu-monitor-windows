using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace HardwareMonitor
{
    public class HardwareInfo
    {
        public int CurrentCPULoad { get { return _currentCPULoad; } }
        public int CurrentCPUTemp { get { return _currentCPUTemp; } }

        private int _currentCPULoad = 0;
        private int _currentCPUTemp = 0;

        public void RefreshSystemInfo()
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
                            _currentCPUTemp = (int)(currentSensor.Value + 0.5);
                        else if (currentSensor.SensorType == SensorType.Load && currentSensor.Name == "CPU Total")
                            _currentCPULoad = (int)(currentSensor.Value + 0.5);
                    }
                }
            }
            computer.Close();
        }
    }
}
