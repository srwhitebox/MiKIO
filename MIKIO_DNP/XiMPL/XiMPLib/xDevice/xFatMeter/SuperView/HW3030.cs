using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xFatMeter.SuperView {
    public class HW3030 : xcSerialDevice{
        /// --- COM Settings ---
        /// Buad Rate = 9600
        /// Databit = 8
        /// Parity = None
        /// Start Bit = 1
        /// Stop Bit = 1
        ///
        /// --- Packet Format ---
        /// Start byte 0x57('W', 1 byte)
        /// Weight     5 bytes
        /// Height_Header 0x48('H', 1 byte)
        /// Weight     5 bytes
        /// Space      1 byte
        /// End        0x5E(1 byte)
        
        public HW3030(string portName = "Any")
            : base("SuperView", "HW3030", DEVICE_TYPE.FAT_METER) {
            initPacket();

            this.PortName = portName;
            this.BaudRate = 9600;
            this.Parity = System.IO.Ports.Parity.None;
            this.DataBits = 8;
            this.StopBits = System.IO.Ports.StopBits.One;
        }

        private void initPacket() {
            Packet.addElement(xcPacket.KEY_START, 1);
            Packet.addElement(xcFatRecord.KEY_WEIGHT, 5);
            Packet.addElement("height_header", 1);
            Packet.addElement(xcFatRecord.KEY_HEIGHT, 5);
            Packet.addElement("space", 1);
            Packet.addElement(xcPacket.KEY_END, 1);

            Packet.Start = 0x57; // 'W'
            Packet.End = 0x0D;  // CR
        }
    }
}
