using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice {
    public delegate void OnNewRecord(object sender, object record);

    public abstract class xcDevice {
        /// <summary>
        /// Device Type Enumeration
        /// </summary>
        public enum DEVICE_TYPE {
            BP_METER,
            FAT_METER,
            FAT_ANALYSER,
            CARD_READER,
            PRINTER,
            DISPLAY,
            KEYBOARD
        }

        /// <summary>
        /// Communication interface enumeration
        /// </summary>
        public enum DEVICE_INTERFACE {
            USB,
            SERIAL,
            BLUETOOTH,
            WIFI
        }

        /// <summary>
        /// Vender / Manufacturer
        /// </summary>
        public string Vendor {
            get;
            set;
        }

        /// <summary>
        /// Model 
        /// </summary>
        public string Name {
            get;
            set;
        }

        public DEVICE_INTERFACE Interface {
            get;
            set;
        }

        public DEVICE_TYPE Type {
            get;
            set;
        }

        public SerialPort SerialPort {
            get;
            set;
        }

        public string PortName {
            get {
                return Interface == DEVICE_INTERFACE.SERIAL ? this.SerialPort.PortName : string.Empty;
            }
            set {
                this.SerialPort.PortName = value;
            }
        }

        public int BaudRate {
            get {
                return Interface == DEVICE_INTERFACE.SERIAL ? this.SerialPort.BaudRate : 0;
            }
            set {
                this.SerialPort.BaudRate = value;
            }
        }

        public Parity Parity {
            get {
                return Interface == DEVICE_INTERFACE.SERIAL ? this.SerialPort.Parity : Parity.None ;
            }
            set {
                this.SerialPort.Parity = value;
            }
        }

        public StopBits StopBits {
            get {
                return Interface == DEVICE_INTERFACE.SERIAL ? this.SerialPort.StopBits : System.IO.Ports.StopBits.None;
            }
            set {
                this.SerialPort.StopBits = value;
            }
        }

        public int DataBits {
            get {
                return Interface == DEVICE_INTERFACE.SERIAL ? this.SerialPort.DataBits : 0;
            }
            set {
                this.SerialPort.DataBits = value;
            }
        }

        public Handshake FlowControl
        {
            get
            {
                return Interface == DEVICE_INTERFACE.SERIAL ? this.SerialPort.Handshake : Handshake.None;
            }
            set
            {
                this.SerialPort.Handshake = value;
            }
        }

        public OnNewRecord OnNewRecord;

        public xcDevice(string vender, string Name, DEVICE_TYPE device_type, DEVICE_INTERFACE deviceInterface) {
            this.Vendor = vender;
            this.Name = Name;
            this.Type = device_type;
            this.Interface = deviceInterface;
        }

        public abstract void Open();

        public abstract void Close();
    }
}
