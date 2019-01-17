using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xBpMeter {

    /// COM Settings
    /// Buad Rate = 9600
    /// Databit = 8
    /// Parity = None
    /// Start Bit = 1
    /// Stop Bit = 1
    ///
    /// <summary>
    /// Start byte 0x5E(1 byte)
    /// Systolic   3 bytes
    /// Average    3 bytes
    /// Diastolic  3 bytes
    /// Pulse      3 bytes
    /// Arrhythmia 1 byte
    /// End        0x5E(1 byte)
    public static class AMPall_BP868F {
        public const string VENDOR = "AMPall";
        public const string MODEL = "BP868(F)";

        //public static byte[] packet = new byte[] 
        //    { START_END, 
        //        (byte)'1', (byte)'2', (byte)'0',
        //        (byte)'0', (byte)'9', (byte)'6',
        //        (byte)'0', (byte)'7', (byte)'2',
        //        (byte)'0', (byte)'6', (byte)'8',
        //        (byte)'1', 
        //    START_END };



        private const byte START_END = 0x5E;
        private const int PACKET_LENGTH = 15;



        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static xcBpRecord getRecord(byte[] packet) {
            int startIndex = getPacketIndex(packet);
            if (startIndex == -1)
                return null;
            startIndex++;
            string systolic = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            string average = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            string diastolic = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            string pulse = Encoding.ASCII.GetString(packet, startIndex, 3);
            startIndex += 3;
            string arrhythmia = Encoding.ASCII.GetString(packet, startIndex, 1);
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
                if (packet.Length - i >= PACKET_LENGTH && packet[i] == START_END && packet[i + PACKET_LENGTH - 1] == START_END) {
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
