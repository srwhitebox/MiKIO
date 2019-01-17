using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaiDoc.PcLink.Connection;
using TaiDoc.PcLink.Meter;
using TaiDoc.PcLink.Meter.Record;

namespace XiMPLib.xDevice.xBpMeter.Fora {
    public class D40 : xcDevice{
        public const uint STATUS_DISCONNECTED = 0x0000;
        public const uint STATUS_CONNECTED = 0x0001;
        public const uint STATUS_NEW_RECORD = 0x0011;
        public const uint STATUS_UNKNOWN = 0xffff;

        private GenBgmAndBpmMeter Meter {
            get;
            set;
        }

        private SiliconLabHIDConnection HidConnection {
            get;
            set;
        }

        public xcBpRecord Record {
            get;
            set;
        }

        public D40()
            : base("Fora", "D40", DEVICE_TYPE.BP_METER, DEVICE_INTERFACE.USB) {
        }

        public delegate void OnStatusChangedCallback(string readerName, uint readerStatus, xcBpRecord record);

        private Thread mStatusChangedThread;
        private OnStatusChangedCallback StatusChangedCallback;
        private Boolean mIsMonitoring = false;
        private uint mPrevStatus = STATUS_UNKNOWN;
        private xcBpRecord mPrevRecord;
        private AbstractConnection Connection;

        public override void Open() {
        }

        public override void Close() {
        }


        public Boolean open() {
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
            } catch {
                return false;
            }
            return true;
        }

        public void close() {
            if (Meter != null) {
                Meter.Disconnect();
                Meter = null;
            }
            if (Connection != null) {
                Connection.DisconnectMeter();
                Connection = null;
            }
        }

        public xcBpRecord getLatestRecord() {
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

        private xcBpRecord dispatchRecord(AbstractRecord record) {
            return new xcBpRecord(record.MeasureDateTime, record.RecordRawBytes[4], record.RecordRawBytes[6], record.RecordRawBytes[7]);
        }


        public void clearAllRecord() {
            Meter.ClearAllRecords(MeterUserNo.CurrentUser);
        }

        public void startMonitor(OnStatusChangedCallback statusChangedCallback) {
            stopMonitor();
            StatusChangedCallback = statusChangedCallback;
            mStatusChangedThread = new Thread(statusMonitor);
            mIsMonitoring = true;
            mStatusChangedThread.Start();
        }

        public void stopMonitor() {
            mIsMonitoring = false;
            StatusChangedCallback = null;

            if (mStatusChangedThread != null) {
                mStatusChangedThread.Abort();
                mStatusChangedThread.Join();
                mStatusChangedThread = null;
            }
        }

        protected void statusMonitor() {
            while (mIsMonitoring) {
                if (open()) {
                    Record = getLatestRecord();
                    if (mPrevRecord == null) {
                        if (Record != null) {
                            if (StatusChangedCallback != null)
                                StatusChangedCallback(this.Name, STATUS_NEW_RECORD, Record);
                            Record.notifyChanged();
                        } else {
                            if (mPrevStatus != STATUS_CONNECTED) {
                                StatusChangedCallback(Name, STATUS_CONNECTED, null);
                                Record.notifyChanged();
                            }
                        }
                    } else {
                        if (Record != null) {
                            if (!Record.Equals(mPrevRecord)) {
                                if (StatusChangedCallback != null)
                                    StatusChangedCallback(Name, STATUS_NEW_RECORD, mPrevRecord = Record);
                                Record.notifyChanged();
                            } else {
                                if (mPrevStatus != STATUS_CONNECTED) {
                                    if (StatusChangedCallback != null)
                                        StatusChangedCallback(Name, STATUS_CONNECTED, null);
                                }
                            }
                        }
                    }
                    mPrevRecord = Record;
                    mPrevStatus = STATUS_CONNECTED;
                    close();
                    System.Threading.Thread.Sleep(1000);
                    continue;
                } else {
                    if (mPrevStatus != STATUS_DISCONNECTED) {
                        if (StatusChangedCallback != null)
                            StatusChangedCallback(null, mPrevStatus = STATUS_DISCONNECTED, null);
                        mPrevStatus = STATUS_DISCONNECTED;
                        Record = new xcBpRecord();
                        Record.notifyChanged();
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
