using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xBpMeter.Omron {
    /// --- COM Settings ---
    /// Buad Rate = 2400
    /// Databit = 8
    /// Parity = Even
    /// Stop Bit = 1    
    /// 
    /// --- Packet Format ---
    /// HEADER     0x3F, 0x3F, 0x00, 0x3F, 0x00, 0x00(6 bytes)
    /// Start      STX(0x02, 1 byte)
    /// Space      1 byte(0x20)
    /// Year       2 bytes
    /// Separator  1 byte(0x3f('?'))
    /// Month      2 bytes
    /// Day        2 bytes
    /// Space      1 byte(0x20)
    /// Hour       2 bytes
    /// Separator  1 byte(0x3f('?'))
    /// Minute     2 bytes
    /// Space      1 byte(0x20)
    /// Systolic   3 bytes
    /// Space      1 byte(0x20)
    /// Separator  1 byte(0x3f('?'))
    /// Diastolic  3 bytes
    /// Space      1 byte(0x20)
    /// Pulse      3 bytes
    /// Separator  1 byte(0x3f('?'))
    /// End        ETX(0x03, 1 byte)
    /// Unknown    1 byte
    
     //02 20 
     //31 35 30 35 30 32 
     //20 
     //30 39 34 33 
     //20 
     //31 32 35 
     //20 
     //30 37 35 
     //20 
     //30 39 39 
     //03

    public class HBP9020 : xcSerialDevice {
        public HBP9020(string portName="Any")
            : base("Omron", "HBP9020", DEVICE_TYPE.BP_METER) {
            initPacket();

            this.PortName = portName;
            this.BaudRate = 2400;
            this.Parity = System.IO.Ports.Parity.Even;
            this.DataBits = 8;
            this.StopBits = System.IO.Ports.StopBits.Two;
            
        }

        private void initPacket() {
            const string comma = "comma";
            const string slash = "slash";
            const string colon = "colon";
            const string year = "year";
            const string month = "month";
            const string day = "day";
            const string hour = "hour";
            const string minute = "min";
            const string id = "id";

 

            Packet.addElement(xcPacket.KEY_START, 2);
            Packet.addElement(comma, 1);
            Packet.addElement(id, 20);
            Packet.addElement(comma, 1);
            Packet.addElement(year, 4);
            Packet.addElement(slash, 1);
            Packet.addElement(month, 2);
            Packet.addElement(slash, 1);
            Packet.addElement(day, 2);
            Packet.addElement(comma, 1);
            Packet.addElement(hour, 2);
            Packet.addElement(colon, 1);
            Packet.addElement(minute, 2);
            Packet.addElement(comma, 1);
            Packet.addElement(xcBpRecord.KEY_SYSTOLIC, 3);
            Packet.addElement(comma, 1);
            Packet.addElement(xcBpRecord.KEY_AVERAGE, 3);
            Packet.addElement(comma, 1);
            Packet.addElement(xcBpRecord.KEY_DIASTOLIC, 3);
            Packet.addElement(comma, 1);
            Packet.addElement(xcBpRecord.KEY_PULSE, 3);
            Packet.addElement(comma, 1);
            Packet.addElement("errCode", 1);
            Packet.addElement(xcPacket.KEY_END, 1);

            Packet.Start = 0x62;    // Start with "bp,"
            Packet.End = xcSerialDevice.CR;
        }

        public void simulate() {
            String packetData = "bp,12345678901234567890,2016/01/25,01:05,100,075,050,065,0\r";
            byte[] bufferData = System.Text.Encoding.ASCII.GetBytes(packetData);
            Packet.setPacketData(bufferData, bufferData.Length);

            OnPacketReceived();
        }

    }
}
