using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.xDevice.xBpMeter;

namespace XiMPLib.MiHIS.Database {
    public class xcHisPhysiologicaMeasuremenRecord{
        public float Height {
            get;
            set;
        }

        public float Weight {
            get;
            set;
        }

        public float BodyTemp {
            get;
            set;
        }

        public int BodyPresL {
            get;
            set;
        }

        public int BodyPressH {
            get;
            set;
        }

        public int BodyFat {
            get;
            set;
        }

        public int WaistLine {
            get;
            set;
        }

        public int Pulse {
            get;
            set;
        }

        public int BloodSugar {
            get;
            set;
        }

        public string UrlParams {
            get {
                string urlParemeters = String.Format("&Height={0}&Weight={1}&BodyTemp={2}&BodyPresL={3}&BodyPresH={4}&BodyFat={5}&WaistLine={6}&Pulse={7}&BloodSuger={8}", Height, Weight, BodyTemp, BodyPresL, BodyPressH, BodyFat, WaistLine, Pulse, BloodSugar);
                return urlParemeters;
            }
        }

        public xcHisPhysiologicaMeasuremenRecord(object record) {
            initValues();
            if (record.GetType().Equals(typeof(xcBpRecord))) {
                setBpRecord((xcBpRecord)record);
            } else if (record.GetType().Equals(typeof(xcFatRecord))) {
                setFatRecord((xcFatRecord)record);
            }
        }

        public xcHisPhysiologicaMeasuremenRecord(xcBpRecord bpRecord) {
            initValues();
            setBpRecord(bpRecord);
        }

        public xcHisPhysiologicaMeasuremenRecord(xcFatRecord fatRecord) {
            initValues();
            setFatRecord(fatRecord);
        }

        public void initValues() {
            this.Height = this.Weight = this.BodyTemp = this.BloodSugar = this.BodyFat = this.BodyPresL = this.BodyPressH = this.Pulse = this.WaistLine = -1;
        }

        private void setBpRecord(xcBpRecord bpRecord) {
            this.BodyPresL = bpRecord.Diastolic;
            this.BodyPressH = bpRecord.Systolic;
            this.Pulse = bpRecord.Pulse;
        }

        private void setFatRecord(xcFatRecord fatRecord) {
            this.Height = fatRecord.Height;
            this.Weight = fatRecord.Weight;
        }

      //<Height>float</Height>
      //<Weight>float</Weight>
      //<BodyTemp>float</BodyTemp>
      //<BodyPresL>short</BodyPresL>
      //<BodyPresH>short</BodyPresH>
      //<BodyFat>short</BodyFat>
      //<WaistLine>short</WaistLine>
      //<Pulse>short</Pulse>
      //<BloodSuger>short</BloodSuger>
    }
}
