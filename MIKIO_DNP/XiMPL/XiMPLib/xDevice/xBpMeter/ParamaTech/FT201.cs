using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xBpMeter.ParamaTech {
    /// --- COM Settings ---
    /// Buad Rate = 4800
    /// Databit = 8
    /// Parity = None
    /// Stop Bit = 2
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

    public class FT201 : xcSerialDevice {
        public FT201(string portName="Any")
            : base("ParamaTech", "FT201", DEVICE_TYPE.BP_METER) {
            initPacket();

            this.PortName = portName;
            this.BaudRate = 4800;
            this.Parity = System.IO.Ports.Parity.None;
            this.DataBits = 8;
            this.StopBits = System.IO.Ports.StopBits.Two;
        }

        private void initPacket() {
            const string space = "space";
            const string year = "year";
            const string month = "month";
            const string day = "day";
            const string hour = "hour";
            const string minute = "min";

            const string separator = "separator";

 

            Packet.addElement(xcPacket.KEY_START, 1);
            Packet.addElement(space, 1);
            Packet.addElement(year, 2);
            // Packet.addElement(separator, 1);
            Packet.addElement(month, 2);
            Packet.addElement(day, 2);
            Packet.addElement(space, 1);
            // Packet.addElement(space, 1);
            Packet.addElement(hour, 2);
            // Packet.addElement(separator, 1);
            Packet.addElement(minute, 2);
            Packet.addElement(space, 1);
            Packet.addElement(xcBpRecord.KEY_SYSTOLIC, 3);
            Packet.addElement(space, 1);
            // Packet.addElement(separator, 1);
            Packet.addElement(xcBpRecord.KEY_DIASTOLIC, 3);
            Packet.addElement(space, 1);
            Packet.addElement(xcBpRecord.KEY_PULSE, 3);
            //Packet.addElement(separator, 1);
            Packet.addElement(xcPacket.KEY_END, 1);

            Packet.Start = xcSerialDevice.STX;
            Packet.End = xcSerialDevice.ETX;
        }
    }
}
