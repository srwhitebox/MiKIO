using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xFatMeter;

namespace XiMPLib.xDevice {
    public static class xcSerialPort {
        private static List<SerialPort> SerialPorts = new List<SerialPort>();
        public static xcBpRecord BpRecord = new xcBpRecord();
        public static xcFatRecord FatRecord = new xcFatRecord();
        public static bool IsMonitorStarted = false;
        public static string[] getAvailablePorts() {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports) {
            }
            return ports;
        }

        public static void startSerialPortMonitor() {
            if (IsMonitorStarted)
                return;

            IsMonitorStarted = true;
            if (SerialPorts == null)
                SerialPorts = new List<SerialPort>();

            string[] ports = getAvailablePorts();
            foreach (string portName in ports) {
                SerialPort port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                try {
                    port.DataReceived += port_DataReceived;
                    port.Open();
                    SerialPorts.Add(port);
                } catch (UnauthorizedAccessException ex) {

                }
            }
        }

        private static bool isReading = false;
        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort port = (SerialPort)sender;
            if (isReading)
                return;
            isReading = true;
            byte[] buffer = new byte[256];
            int i = 0;
            while (port.BytesToRead > 0) {
                buffer[i++] = (byte)port.ReadByte();
                System.Threading.Thread.Sleep(5);
            }
            port.Read(buffer, 0, port.BytesToRead);

            xcBpRecord record = AMPall_BP868F.getRecord(buffer);
            if (record != null){
                BpRecord.copyFrom(record);
                BpRecord.notifyChanged();
            }else{
                xcFatRecord fatRecord = GTech_GL150.getRecord(buffer);
                if (fatRecord!=null){
                    FatRecord.copyFrom(fatRecord);
                    FatRecord.notifyChanged();
                }
            }

            isReading = false;
        }

    }
}
