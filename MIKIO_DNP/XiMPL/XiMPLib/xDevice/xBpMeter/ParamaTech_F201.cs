using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xBpMeter {
    /// COM Settings
    /// Buad Rate = 4800
    /// Databit = 8
    /// Parity = None
    /// Stop Bit = 2
    /// 
    /// <summary>
    /// Start byte 0xBF(1 byte)
    /// Year       2 bytes
    /// Month      2 bytes
    /// Day        2 bytes
    /// Hour       2 bytes
    /// Minute     2 bytes
    /// Systolic   3 bytes
    /// Diastolic  3 bytes
    /// Pulse      3 bytes
    /// End        0xC0(1 byte)
    /// End        0x91(1 byte)
    public static class ParamaTech_FT201 {
        public const string VENDOR = "Parama-Tech";
        public const string MODEL = "FT-201";
        //public static byte[] packet = new byte[] 
        //    { START_END, 
        //        (byte)'1', (byte)'2', (byte)'0',
        //        (byte)'0', (byte)'9', (byte)'6',
        //        (byte)'0', (byte)'7', (byte)'2',
        //        (byte)'0', (byte)'6', (byte)'8',
        //        (byte)'1', 
        //    START_END };



        private const byte START = 0xBF;
        private const byte END = 0xC0;
        private const int PACKET_LENGTH = 22;



        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static xcBpRecord getRecord(byte[] packet) {
            int startIndex = getPacketIndex(packet);
            if (startIndex == -1)
                return null;
            startIndex++;
            string date = Encoding.ASCII.GetString(packet, startIndex, 6);
            startIndex += 6;
            string time = Encoding.ASCII.GetString(packet, startIndex, 4);
            startIndex += 4;
            string systolic = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            string diastolic = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            string pulse = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            return new xcBpRecord(DateTime.Now, int.Parse(systolic), int.Parse(diastolic), int.Parse(pulse));
        }

        /// <summary>
        /// return packet start index, if it doesn't have the packet, it'll return -1.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static int getPacketIndex(byte[] packet) {
            int startIndex = -1;

            for (int i = 0; i < packet.Length; i++) {
                if (packet.Length - i >= PACKET_LENGTH && packet[i] == START && packet[i + PACKET_LENGTH - 2] == END) {
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
