using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Management;

namespace XiMPLib.xDevice {
    public class xcPnpDevice {
        public static string[] getSerialPorts(){
            return SerialPort.GetPortNames();
        }

        public static string[] getModemPort() {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_POTSModem");
            int count = mos.Get().Count;
            if (count > 0) {
                int i = 0;
                string[] ports = new string[count];
                foreach (ManagementObject mo in mos.Get()) {
                    // for modem name
                    string caption = mo["Caption"].ToString();
                    string to = mo["AttachedTo"].ToString();
                    ports[i++] = to;
                }
                return ports;
            } else {
                return null;
            }
        }

        public static xcPnpDeviceInfo[] getPnpDevices() {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            int count = mos.Get().Count;
            if (count == 0) {
                return null;
            }
            xcPnpDeviceInfo[] devices = new xcPnpDeviceInfo[count];
            int i=0;
            foreach (ManagementObject mo in mos.Get()) {
                devices[i++] = new xcPnpDeviceInfo(mo);
            }
            return devices;
        }
    }
}
