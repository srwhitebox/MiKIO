using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xBpMeter.AMPAll {
    public class BP868F : xcSerialDevice{
        /// --- COM Settings ---
        /// Buad Rate = 9600
        /// Databit = 8
        /// Parity = None
        /// Start Bit = 1
        /// Stop Bit = 1
        ///
        /// --- Packet Format ---
        /// Start byte 0x5E(1 byte)
        /// Systolic   3 bytes
        /// Average    3 bytes
        /// Diastolic  3 bytes
        /// Pulse      3 bytes
        /// Arrhythmia 1 byte
        /// End        0x5E(1 byte)

        public BP868F(string portName="Any") : base("AMPall", "BP868F", DEVICE_TYPE.BP_METER){
            initPacket();

            this.PortName = portName;
            this.BaudRate = 9600;
            this.Parity = System.IO.Ports.Parity.None;
            this.DataBits = 8;
            this.StopBits = System.IO.Ports.StopBits.One;
     
        }

        private void initPacket() {
            Packet.addElement(xcPacket.KEY_START, 1);
            Packet.addElement(xcBpRecord.KEY_SYSTOLIC, 3);
            Packet.addElement(xcBpRecord.KEY_AVERAGE, 3);
            Packet.addElement(xcBpRecord.KEY_DIASTOLIC, 3);
            Packet.addElement(xcBpRecord.KEY_PULSE, 3);
            Packet.addElement(xcBpRecord.KEY_ARRHYTHMIA, 1);
            Packet.addElement(xcPacket.KEY_END, 1);

            Packet.Start = Packet.End = 0x5E;
        }

        public void simulate()
        {
            byte[] bufferData = {
                0x5E,
                (byte)'1', (byte)'2', (byte)'0',
                (byte)'0', (byte)'9', (byte)'6',
                (byte)'0', (byte)'7', (byte)'2',
                (byte)'0', (byte)'6', (byte)'8',
                (byte)'1',
                0x5E};
                Packet.setPacketData(bufferData, bufferData.Length);

            OnPacketReceived();
        }
    }
}
