using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using XiMPLib.xDevice.xBpMeter;

namespace XiMPLib.xDevice {
    public class xcSerialDevice : xcDevice{
        public const string KEY_CR = "cr";
        public const string KEY_LF = "lf";

        public const string KEY_SPACE = "space";
        public const string KEY_YEAR = "year";
        public const string KEY_MONTH = "month";
        public const string KEY_DAY = "day";
        public const string KEY_HOUR = "hour";
        public const string KEY_MINUTE = "min";
        public const string KEY_SEPARATOR = "separator";
        public const string KEY_COLON = "colon";
        public const string KEY_BODY_TYPE = "body_type";

        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte CR = 0x0D;
        public const byte LF = 0x0A;

        protected int BufferSize {
            get;
            set;
        }

        public xcPacket Packet{
            get;
            set;
        }
        
        public xcSerialDevice(string vender, string name, DEVICE_TYPE deviceType, int bufferSize=128) : base(vender, name, deviceType, DEVICE_INTERFACE.SERIAL) {
            this.SerialPort = new SerialPort();
            BufferSize = bufferSize;
            Packet = new xcPacket();
        }

        public override void Open() {
            if (!SerialPort.IsOpen) {
                this.SerialPort.DataReceived += DataReceived;
                try {
                    this.SerialPort.Open();
                } catch {
                    Console.WriteLine(SerialPort.PortName + " is not exist.");
                }
            }
        }

        public override void Close() {
            if (SerialPort.IsOpen) {
                this.SerialPort.DataReceived -= DataReceived;
                this.SerialPort.Close();
            }
        }

        protected virtual void DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort port = (SerialPort)sender;
            byte[] buffer = new byte[512];
            int i = 0;
            while (port.BytesToRead > 0) {
                buffer[i++] = (byte)port.ReadByte();
                System.Threading.Thread.Sleep(10);
            }

            Packet.setPacketData(buffer, i);
            if (Packet.isPacket) {
                OnPacketReceived();
            }
        }

        protected void OnPacketReceived() {
            switch (this.Type) {
                case DEVICE_TYPE.BP_METER:
                    Packet.BpRecord.notifyChanged();
                    if (OnNewRecord != null)
                        OnNewRecord(this, Packet.BpRecord);
                    break;
                default:
                    break;
            }
        }
    }
}
