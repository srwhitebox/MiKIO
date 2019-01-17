using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaiDoc.PcLink.Meter;
using TaiDoc.PcLink.Connection;
using TaiDoc.PcLink.Meter.Record;

namespace XiMPLib.xDevice.xBpMeter {
    public class Fora_D40 : xcBpMeter {
        public const string VENDOR = "FORA";
        public const string MODEL = "D40/TD-3261";

        private GenBgmAndBpmMeter Meter;
        private static AbstractConnection Connection;

        public override string ModelName {
            get {
                string modelName = Meter.GetMeterType().ToString()+Meter.GetMeterInfo().SubModel;
                return modelName.ToUpperInvariant();
            }
        }

        public override string SerialNumber {
            get {
                return Meter.GetSerialNumber();
            }
        }

        public override DateTime DateTime {
            get {                
                return Meter.GetDateTime();
            }
            set {
                Meter.SetDateTime(value);
            }
        }

        public override Boolean open(){
            const ushort meterVID = 0x10C4;
            const ushort meterPID = 0xEA80;
            SiliconLabHIDConnection connection;
            try {
                connection = ConnectionManager.CreateSiliconLabHIDConnection(meterVID, meterPID);
                connection.RetryTimes = 1;
                connection.RxTimeoutInMs = 30;
                if (connection != null) {
                    if (connection.ConnectMeter()) {
                        connection.DisconnectMeter();
                        Connection = connection;
                        Meter = (GenBgmAndBpmMeter)MeterManager.GetConnectedMeter(Connection);
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            } catch{
                return false;
            }
            return true;
        }

        public override void close() {
            if (Meter != null) {
                Meter.Disconnect();
                Meter = null;
            }
            if (Connection != null) {
                Connection.DisconnectMeter();
                Connection = null;
            }
        }

        public override xcBpRecord getLatestRecord() {
            return getLatestRecord(MeterUserNo.CurrentUser);
        }

        public xcBpRecord getLatestRecord(MeterUserNo userNo) {
            if (Meter == null)
                return null;
            int count = Meter.GetRecordAmount(userNo);
            try {
                return count == -1 ? null : dispatchRecord(Meter.GetRecord(0, userNo));
            } catch {
                return null;
            }
        }

        public List<xcBpRecord> getAllRecord() {
            List<xcBpRecord> records = new List<xcBpRecord>();
            var curUserRecords = Meter.GetAllRecord(MeterUserNo.CurrentUser);
            foreach (AbstractRecord record in curUserRecords) {
                records.Add(dispatchRecord(record));
            }
            return records;
        }

        private xcBpRecord dispatchRecord(AbstractRecord record){
            return new xcBpRecord(record.MeasureDateTime, record.RecordRawBytes[4], record.RecordRawBytes[6], record.RecordRawBytes[7]);
        }


        public override void clearAllRecord() {
            Meter.ClearAllRecords(MeterUserNo.CurrentUser);
        }
    }


}
