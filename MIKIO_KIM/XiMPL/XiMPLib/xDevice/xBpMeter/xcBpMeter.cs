using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace XiMPLib.xDevice.xBpMeter {
    public abstract class xcBpMeter : INotifyPropertyChanged {
        public const uint STATUS_DISCONNECTED = 0x0000;
        public const uint STATUS_CONNECTED = 0x0001;
        public const uint STATUS_NEW_RECORD = 0x0011;
        public const uint STATUS_UNKNOWN = 0xffff;


        public abstract string ModelName {
            get;
        }

        public abstract string SerialNumber {
            get;
        }

        public abstract DateTime DateTime {
            get;
            set;
        }

        public DateTime RecordedDateTime {
            get {
                return Record.MeasuredAt;
            }
        }

        public int Systolic {
            get {
                return Record == null ? 0 : Record.Systolic;
            }
        }

        public int Diastolic {
            get {
                return Record == null ? 0 : Record.Diastolic;
            }
        }

        public int Pulse {
            get {
                return Record == null ? 0 : Record.Pulse;
            }
        }

        public string Comment {
            get {
                string[] comment = { "正常範圍", "高血壓前期", "高血壓" };
                int index = 0;
                if (Systolic < 120 && Diastolic < 80) {
                    index = 0;
                } else if (Systolic >= 120 && Systolic < 140 && Diastolic >= 80 && Diastolic < 90) {
                    index = 1;
                } else { // Systolic >= 140 && Diastolic >= 90
                    index = 2;
                }
                return comment[index];
            }
        }

        public xcBpRecord Record {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void statusChangedCallback(string readerName, uint readerStatus, xcBpRecord record);

        private Thread mStatusChangedThread;
        private statusChangedCallback mStatusChangedCallback;
        private Boolean mIsMonitoring = false;
        private uint mPrevStatus = STATUS_UNKNOWN;
        private xcBpRecord mPrevRecord;

        public void startMonitor(statusChangedCallback statusChangedCallback) {
            stopMonitor();
            mStatusChangedCallback = statusChangedCallback;
            mStatusChangedThread = new Thread(statusMonitor);
            mIsMonitoring = true;
            mStatusChangedThread.Start();
        }

        public void stopMonitor() {
            mIsMonitoring = false;
            mStatusChangedCallback = null;

            if (mStatusChangedThread != null) {
                mStatusChangedThread.Abort();
                mStatusChangedThread.Join();
                mStatusChangedThread = null;
            }
        }

        protected void statusMonitor(){
            while(mIsMonitoring){
                if (open()) {
                    Record = getLatestRecord();
                    if (mPrevRecord == null) {
                        if (Record != null) {
                            if (mStatusChangedCallback!=null)
                                mStatusChangedCallback(ModelName, STATUS_NEW_RECORD, Record);
                            notifyChanged();
                        } else {
                            if (mPrevStatus != STATUS_CONNECTED) {
                                mStatusChangedCallback(ModelName, STATUS_CONNECTED, null);
                                notifyChanged();
                            }
                        }
                    } else {
                        if (Record != null) {
                            if (!Record.Equals(mPrevRecord)) {
                                if (mStatusChangedCallback!=null)
                                    mStatusChangedCallback(ModelName, STATUS_NEW_RECORD, mPrevRecord = Record);
                                notifyChanged();
                            } else {
                                if (mPrevStatus != STATUS_CONNECTED) {
                                    if (mStatusChangedCallback != null)
                                        mStatusChangedCallback(ModelName, STATUS_CONNECTED, null);
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
                        if (mStatusChangedCallback != null)
                            mStatusChangedCallback(null, mPrevStatus = STATUS_DISCONNECTED, null);
                        Record = null;
                        notifyChanged();
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void notifyChanged() {
            OnPropertyChanged("Systolic");
            OnPropertyChanged("Diastolic");
            OnPropertyChanged("Pulse");
            OnPropertyChanged("Comment");
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public abstract Boolean open();
        public abstract void close();
        public abstract xcBpRecord getLatestRecord();
        public abstract void clearAllRecord();
    }
}
