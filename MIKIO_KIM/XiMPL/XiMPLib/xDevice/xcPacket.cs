using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.xDevice.xFatAnalyser;
using XiMPLib.xType;

namespace XiMPLib.xDevice {

    public class xcPacket {
        public const string KEY_START = "start";
        public const string KEY_END = "end";

        private byte[] Data {
            get;
            set;
        }

        private int DataLength {
            get;
            set;
        }

        public byte Start {
            get;
            set;
        }

        public byte End {
            get;
            set;
        }

        private int Length {
            get {
                int length = 0;
                foreach(KeyValuePair<string, int> pair in Elements){
                    length += pair.Value;
                }
                return length;
            }
        }

        public int StartIndex {
            get {
                int packetLength = Length;
                // Data length should be larger than packet length
                if (Data == null || Data.Length < packetLength)
                    return -1;
                for (int i = 0; i < Data.Length; i++) {
                    if (i + packetLength > Data.Length) {
                        break;
                    }
                    if (Data[i] == Start) {
                        if (Data[i + Length - 1] == End)
                            return i;
                    }
                }
                return -1;
            }
        }

        public bool isPacket {
            get {
                return StartIndex >= 0;
            }
        }

        public xcBpRecord BpRecord {
            get {
                return new xcBpRecord(getInt(xcBpRecord.KEY_SYSTOLIC, 0), getInt(xcBpRecord.KEY_DIASTOLIC, 0), getInt(xcBpRecord.KEY_PULSE, 0));
            }
        }

        public xcFatRecord FatRecord {
            get {
                return new xcFatRecord(getFloat(xcFatRecord.KEY_HEIGHT, 0), getFloat(xcFatRecord.KEY_WEIGHT, 0));
            }
        }

        public xcFatCompositionRecord FatCompositionRecord{
            get {
                return new xcFatCompositionRecord(this);
            }
        }

        private List<KeyValuePair<string, int>> Elements = new List<KeyValuePair<string, int>>();

        public xcPacket() {
        }

        public void setPacketData(byte[] packetData, int length) {
            this.Data = packetData;
            this.DataLength = length;
        }

        public void addElement(string key, int length) {
            Elements.Add(new KeyValuePair<string, int>(key, length));
        }

        public void addElement(string key) {
            Elements.Add(new KeyValuePair<string, int>(key, 1));
        }

        public string get(string key) {
            int index = StartIndex;
            if (index == -1)
                return null;

            int length = 0;
            foreach (KeyValuePair<string, int> pair in Elements) {
                if (!pair.Key.Equals(key)) {
                    index += pair.Value;
                } else {
                    length = pair.Value;
                    break;
                }
            }

            return Data != null & Data.Length >= index + length ? Encoding.ASCII.GetString(Data, index, length) : null;
        }

        public int getInt(string key, int defaultValue = -1) {
            string value = get(key);
            return string.IsNullOrEmpty(value) ? defaultValue : (int)xcDecimal.Parse(value);
        }

        public float getFloat(string key, int defaultValue = -1) {
            string value = get(key);
            return string.IsNullOrEmpty(value) ? defaultValue : (float)xcDecimal.Parse(value);
        }

        public char getChar(string key, char defaultValue='M') {
            string value = get(key);
            return string.IsNullOrEmpty(value) ? defaultValue : value[0];
        }
    }
}
