using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xDevice;

namespace XiMPLib.xDevice {
    public class xcDeviceList : List<xcDevice>{
        
        public void add(xcDevice device) {
            xcDevice curDevice = null;
            foreach (xcDevice deviceItem in this) {
                if (device.Name.Equals(deviceItem.Name)) {
                    curDevice = deviceItem;
                    break;
                }
            }

            if (curDevice == null) {
                Add(device);
            } else {
                curDevice.PortName = device.PortName;
                curDevice.BaudRate = device.BaudRate;
                curDevice.Parity = device.Parity;
                curDevice.DataBits = device.DataBits;
                curDevice.StopBits = device.StopBits;
            }
        }

        public void remove(xcDevice device) {
            xcDevice deviceIn = null;
            foreach (xcDevice deviceItem in this) {
                if (device.Name.Equals(deviceItem.Name)) {
                    deviceIn = device;
                }
            }
            if (deviceIn != null)
                this.Remove(deviceIn);
        }


        public bool contains(xcDevice device) {
            foreach (xcDevice deviceItem in this) {
                if (device.Name.Equals(deviceItem.Name))
                    return true;
            }
            return false;
        }
    }
}
