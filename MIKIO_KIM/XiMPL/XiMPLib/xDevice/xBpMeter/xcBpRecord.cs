using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using XiMPLib.xBinding;
using XiMPLib.xType;

namespace XiMPLib.xDevice.xBpMeter{
    public class xcBpRecord : INotifyPropertyChanged {
        public const string KEY_DATETIME = "recorded";
        public const string KEY_SYSTOLIC = "systolic";
        public const string KEY_DIASTOLIC = "diastolic";
        public const string KEY_AVERAGE = "average";
        public const string KEY_PULSE = "pulse";
        public const string KEY_ARRHYTHMIA = "arrhythmia";

        public DateTime MeasuredAt {
            get;
            set;
        }

        public int Systolic {
            get;
            set;
        }

        public int Diastolic {
            get;
            set;
        }

        public int Average {
            get {
                return (int)Math.Round(((double)(Systolic + Diastolic)) / 2f);
            }
        }

        public int Pulse {
            get;
            set;
        }

        public int Arrhythmia {
            get;
            set;
        }

        public string Comment {
            get {
                if (Systolic == 0 || Diastolic == 0 || Pulse == 0)
                    return null;
                string[] comment = { "正常範圍", "高血壓前期", "高血壓" };

                int indexSystolic = 0;
                if (Systolic < 120)
                {
                    indexSystolic = 0;
                }
                else if (Systolic >= 120 && Systolic < 140)
                {
                    indexSystolic = 1;
                }
                else
                { // Systolic >= 140 && Diastolic >= 90
                    indexSystolic = 2;
                }

                int indexDiastolic = 0;
                if (Diastolic < 80)
                {
                    indexDiastolic = 0;
                }
                else if (Diastolic >= 80 && Diastolic < 90)
                {
                    indexDiastolic = 1;
                }
                else
                { // Systolic >= 140 && Diastolic >= 90
                    indexDiastolic = 2;
                }

                int index = Math.Max(indexSystolic, indexDiastolic);

                return comment[index];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public xcBpRecord() {

        }


        public xcBpRecord(DateTime recordedDateTime, int systolic, int diastolic, int pulse, int arrhythmia = -1) {
            MeasuredAt = recordedDateTime;
            Systolic = systolic;
            Diastolic = diastolic;
            Pulse = pulse;
            Arrhythmia = arrhythmia;
        }

        public xcBpRecord(int systolic, int diastolic, int pulse, int arrhythmia = -1) {
            MeasuredAt = DateTime.Now;
            Systolic = systolic;
            Diastolic = diastolic;
            Pulse = pulse;
            Arrhythmia = arrhythmia;
        }

        public void copyFrom(xcBpRecord record) {
            if (record == null)
                this.clear();
            else
                xcObject.copyProperties(this, record);
            //MeasuredAt = record.MeasuredAt
            //Systolic = record.Systolic;
            //Diastolic = record.Diastolic;
            //Pulse = record.Pulse;
            //Arrhythmia = record.Arrhythmia;
            notifyChanged();
        }

        public void notifyChanged(){
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }

            //OnPropertyChanged("MeasuredAt");
            //OnPropertyChanged("Systolic");
            //OnPropertyChanged("Diastolic");
            //OnPropertyChanged("Pulse");
            //OnPropertyChanged("Comment");
        }

        public override bool Equals(System.Object obj) {
            return obj != null && Equals(obj as xcBpRecord);
        }

        public bool Equals(xcBpRecord record) {
            return record != null && MeasuredAt == record.MeasuredAt;
        }

        public override int GetHashCode() {
            return (int)MeasuredAt.Ticks;
        }

        public void clear() {
            this.Systolic = 0;
            this.Pulse = 0;
            this.Diastolic = 0;
            notifyChanged();
        }

        public override string ToString() {
            JObject jBpInfo = new JObject();
            jBpInfo.Add(KEY_SYSTOLIC, Systolic);
            jBpInfo.Add(KEY_DIASTOLIC, Diastolic);
            jBpInfo.Add(KEY_PULSE, Pulse);

            return jBpInfo.ToString();
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
