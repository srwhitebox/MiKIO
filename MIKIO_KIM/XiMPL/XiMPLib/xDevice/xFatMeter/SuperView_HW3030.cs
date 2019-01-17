using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xType;

namespace XiMPLib.xDevice.xFatMeter {

    /// COM Settings
    /// Buad Rate = 9600
    /// Databit = 8
    /// Parity = None
    /// Start Bit = 1
    /// Stop Bit = 1
    ///

    /// <summary>
    /// After Power on : JP:2 JP789:0
    /// 
    /// 
    /// W   1 byte
    /// Weight      5 byte(xxx.x)
    /// :           1 byte(,)
    /// H           1 byte(,)
    /// Height      5 byte(xx.xx)
    /// 0x0D        1 byte
    /// 
    /// Total Length : 27
    /// </summary>
    public static class SuperView_HW3030 {
        public const string VENDOR = "SuperView";
        public const string MODEL = "JP789";

        //public static byte[] packet = new byte[] 
        //    { STX, 
        //        (byte)'1', (byte)'7', (byte)'8', (byte)'.', (byte)'5', 
        //        (byte)',',
        //        (byte)'1', (byte)'0', (byte)'5', (byte)'.', (byte)'4', 
        //        (byte)',',
        //        (byte)'M',
        //        (byte)',',
        //        (byte)'1', (byte)'2', (byte)'3', (byte)'.', (byte)'5', 
        //        (byte)',',
        //        (byte)'6', (byte)'7', (byte)'8', (byte)'.', (byte)'9', 
        //    ETX };

        private const byte START = 0x48; //(byte)'H';
        private const byte END = 0x0D;
        private const int PACKET_LENGTH = 13;
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static xcFatRecord getRecord(byte[] packet) {
            int startIndex = getPacketIndex(packet);
            if (startIndex == -1)
                return null;

            startIndex++;
            string weight = Encoding.ASCII.GetString(packet, startIndex, 5);
            startIndex += 1 + 5 +1;
            string height = Encoding.ASCII.GetString(packet, startIndex, 5);
            return new xcFatRecord((float)xcDecimal.Parse(height), (float)xcDecimal.Parse(weight));
        }

        /// <summary>
        /// return packet start index, if it doesn't have the packet, it'll return -1.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static int getPacketIndex(byte[] packet) {
            int startIndex = -1;

            for (int i = 0; i < packet.Length; i++) {
                if (packet.Length - i >= PACKET_LENGTH && packet[i] == START && packet[i + PACKET_LENGTH-1] == END) {
                    return i;
                }
            }
            return startIndex;
        }

        /// <summary>
        /// determine the byte array has the packet
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static bool hasPacket(byte[] packet) {
            return getPacketIndex(packet) >= 0;
        }
    }


}
