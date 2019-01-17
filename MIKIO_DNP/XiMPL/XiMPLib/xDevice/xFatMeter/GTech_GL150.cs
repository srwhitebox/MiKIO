using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xFatMeter {
    /// <summary>
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
    /// 
    /// Total Length : 27
    /// </summary>
    public static class GTech_GL150 {
        public static string VENDOR = "GTech";
        public static string MODEL = "GL150";

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

        private const byte STX = 0x02;
        private const byte ETX = 0x03;
        private const int PACKET_LENGTH = 27;
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static xcFatRecord getRecord(byte[] packet) {
            int startIndex = getPacketIndex(packet);
            if (startIndex == -1)
                return null;

            startIndex++;
            string height = Encoding.ASCII.GetString(packet, startIndex, 5);
            startIndex += 1 + 5;
            string weight = Encoding.ASCII.GetString(packet, startIndex, 5);
            startIndex += 1 + 5;
            string gender = Encoding.ASCII.GetString(packet, startIndex, 1);
            startIndex += 1 + 1;
            string obesity = Encoding.ASCII.GetString(packet, startIndex, 5);
            startIndex += 1 + 5;
            string bmi = Encoding.ASCII.GetString(packet, startIndex, 5);
           return new xcFatRecord(float.Parse(height), float.Parse(weight));
        }

        /// <summary>
        /// return packet start index, if it doesn't have the packet, it'll return -1.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static int getPacketIndex(byte[] packet) {
            int startIndex = -1;

            for (int i = 0; i < packet.Length; i++) {
                if (packet.Length - i >= PACKET_LENGTH && packet[i] == STX && packet[i + PACKET_LENGTH-1] == ETX) {
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
