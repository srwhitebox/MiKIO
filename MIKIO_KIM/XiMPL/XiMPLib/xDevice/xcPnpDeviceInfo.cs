using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace XiMPLib.xDevice {
    public class xcPnpDeviceInfo {
        public string Caption {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public string DeviceID {
            get;
            set;
        }

        public string PnpDeviceID {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public xcPnpDeviceInfo() {

        }
        public xcPnpDeviceInfo(ManagementObject managementObject) {
            Caption = managementObject["Caption"].ToString();
            DeviceID = managementObject["DeviceID"].ToString();
            Description = managementObject["Description"].ToString();
            Name = managementObject["name"].ToString();
            PnpDeviceID = managementObject["PNPDeviceID"].ToString();
        }

    }
}
