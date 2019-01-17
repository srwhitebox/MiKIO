using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xFatMeter.GTech {
    public class GL150 : xcSerialDevice{
        /// --- COM Settings ---
        /// Buad Rate = 9600
        /// Databit = 8
        /// Parity = None
        /// Start Bit = 1
        /// Stop Bit = 1
        ///
        /// --- Packet Format ---
        /// STX(0x02)   1 byte
        /// Height      5 byte(xxx.x)
        /// Comma       1 byte(,)
        /// Weight      5 byte(xx.xx)
        /// Comma       1 byte(,)
        /// Gender      1 byte(M-mail/F-female/C-children)
        /// Comma       1 byte(,)
        /// Obesity     5 byte(xxx.x)
        /// Comma       1 byte(,)
        /// BMI         5 byte(xxx.x)
        /// ETX(0x03)   1 byte
        
        public GL150(string portName="Any")
            : base("GTech", "GL150", DEVICE_TYPE.FAT_METER) {
            initPacket();

            this.PortName = portName;
            this.BaudRate = 9600;
            this.Parity = System.IO.Ports.Parity.None;
            this.DataBits = 8;
            this.StopBits = System.IO.Ports.StopBits.One;
        }

        private void initPacket() {
            const string comma = "comma";

            Packet.addElement(xcPacket.KEY_START, 1);
            Packet.addElement(xcFatRecord.KEY_HEIGHT, 5);
            Packet.addElement(comma, 1);
            Packet.addElement(xcFatRecord.KEY_WEIGHT, 5);
            Packet.addElement(comma, 1);
            Packet.addElement(xcFatRecord.KEY_GENDER, 1);
            Packet.addElement(comma, 1);
            Packet.addElement(xcFatRecord.KEY_OBESITY, 5);
            Packet.addElement(comma, 1);
            Packet.addElement(xcFatRecord.KEY_BMI, 5);
            Packet.addElement(xcPacket.KEY_END, 1);

            Packet.Start = xcSerialDevice.STX;
            Packet.End = xcSerialDevice.ETX;
        }

        public void simulate()
        {
            byte[] bufferData = {
                STX,
                (byte)'1', (byte)'5', (byte)'8', (byte)'.', (byte)'5',
                (byte)',',
                (byte)'6', (byte)'0', (byte)'.', (byte)'5', (byte)'4',
                (byte)',',
                (byte)'M',
                (byte)',',
                (byte)'1', (byte)'2', (byte)'3', (byte)'.', (byte)'5',
                (byte)',',
                (byte)'6', (byte)'7', (byte)'8', (byte)'.', (byte)'9',
            ETX };

            Packet.setPacketData(bufferData, bufferData.Length);

            OnPacketReceived();
        }
    }
}
