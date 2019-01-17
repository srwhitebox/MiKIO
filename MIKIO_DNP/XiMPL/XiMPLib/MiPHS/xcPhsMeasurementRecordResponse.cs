using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace XiMPLib.MiPHS {
    public class xcPhsMeasurementRecordResponse : JArray{

        public bool IsSucceed {
            get {
                return this.Count > 0;
            }
        }

        List<xcPhsMeasurementRecord> recordList = new List<xcPhsMeasurementRecord>();

        public List<xcPhsMeasurementRecord> List {
            get {
                return recordList;
            }
        }

        public xcPhsMeasurementRecordResponse(string dataSource, string response) {
            if (String.IsNullOrEmpty(response))
                return;
            switch (dataSource) {
                case "mihealth":
                    JObject jResponse = JObject.Parse(response);
                    int responseCode = int.Parse(jResponse["code"].ToString());
                    if (responseCode < 400) {
                        JArray jArray = (JArray)JObject.Parse(response)["data"];
                        foreach(JObject jRecord in jArray) {
                            xcPhsMeasurementRecord record = new xcPhsMeasurementRecord(dataSource, jRecord);
                            recordList.Add(record);
                        }
                    }
                    break;
            }
        }

        public xcPhsMeasurementRecordResponse(string response)
            : base(JArray.Parse(response)) {
                dispatch();
        }

        private void dispatch() {
            foreach (JToken token in this) {
                JObject jRecord = token.ToObject<JObject>();
                xcPhsMeasurementRecord record = new xcPhsMeasurementRecord(jRecord);
                recordList.Add(record);
            }
        }
    }
}
