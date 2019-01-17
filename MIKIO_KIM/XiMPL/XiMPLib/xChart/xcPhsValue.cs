using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.MiPHS;
using XiMPLib.xType;

namespace XiMPLib.xChart {
    public class xcPhsValue {
        public DateTime MeasuredDateTime {
            get;
            set;
        }
        public string Code {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public double Value {
            get;
            set;
        }

        public string Unit {
            get;
            set;
        }

        public string AttrID {
            get;
            set;
        }

        public string CareID {
            get;
            set;
        }


        public xcPhsValue(string fieldCode, xcPhsMeasurementRecord record) {
            this.Code = fieldCode;
            this.MeasuredDateTime = record.Recorded;
            var jarrayValues = (JArray)record["data"];
            foreach (JToken token in jarrayValues) {
                JObject pair = (JObject)token;
                string codeName = pair["code"].ToString();
                if (codeName.Equals(Code)) {
                    Value = double.Parse(pair["value"].ToString());
                } else {
                    string value = pair["value"].ToString();
                    switch (codeName) {
                        case "UNIT":
                            Unit = value;
                            break;
                        case "ATTR_NAME":
                            Name = value;
                            break;
                        case "CARE_ATTR_ID":
                            AttrID = value;
                            break;
                        case "CARE_ID":
                            CareID = value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public xcPhsValue(DateTime dateTime, double value) {
            this.MeasuredDateTime = dateTime;
            this.Value = value;
        }

        public bool Equals(xcPhsValue other) {
            return this.MeasuredDateTime.Equals(other.MeasuredDateTime) && this.Value == other.Value;
        }

        public int CompareTo(xcPhsValue other) {
            return this.MeasuredDateTime.CompareTo(other.MeasuredDateTime);
        }
    }
}
