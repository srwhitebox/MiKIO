using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.xDevice.xFatAnalyser;
using XiMPLib.xType;

namespace XiMPLib.MiPHS {
    public class xcPhsMeasurementRecord : JObject{

        public const string KEY_RECORD_DATE = "recordDate";
        public const string KEY_RECORD_HH = "recordHH";
        public const string KEY_RECORD_MM = "recordMM";

        public const string KEY_TEMPERATURE = "TEMPERATURE";    // 體溫
        public const string KEY_BLOOD_SUGAR_AC = "BLOOD_SUGAR_AC"; // 飯前血糖
        public const string KEY_BLOOD_SUGAR_PC = "BLOOD_SUGAR_PC"; // 飯後血糖
        public const string KEY_DIASTOLIC = "DIASTOLIC_RIGHT";  // 收縮壓
        public const string KEY_SYSTOLIC = "SYSTOLIC_RIGHT";    // 舒張壓
        public const string KEY_PULSE = "PULSE_RATE";   // 脈搏
        public const string KEY_HEIGHT = "BODY_HEIGHT"; // 身高
        public const string KEY_WEIGHT = "BODY_WEIGHT"; // 體重
        public const string KEY_BMI = "BMI";    // BMI
        public const string KEY_BMR = "BMR";    // BMR
        public const string KEY_PBF = "PBF";    // 體脂率
        public const string KEY_BODY_FAT = "BODY_FAT";  // 體脂
        public const string KEY_FFM = "FFM";    // FFM
        public const string KEY_TBW = "TBW";    // TBW
        public const string KEY_WAIST_GIRTH = "WAIST_GIRTH"; // 腰圍
        public const string KEY_HIP_GIRTH = "HIP_GIRTH"; // 臀圍
        public const string KEY_NECK_GIRTH = "NECK_GIRTH"; // 脖圍
        public const string KEY_WRIST_GIRTH = "WRIST_GIRTH"; // 腕圍
        public const string KEY_AN = "AN";    // AN
        public const string KEY_FL = "FL";    // F.L.
        public const string KEY_SF = "SF";    // S.F.
        public const string KEY_VF = "VF";    // V.F.

        public const string KEY_CODE = "code";
        public const string KEY_CID = "cid";
        public const string KEY_ID = "id";
        public const string KEY_VALUE = "value";
        public const string KEY_DATA = "data";

        public const string ATTR_NAME = "ATTR_NAME";
        public const string ATTR_VALUE = "ATTR_VALUE";
        public const string ATTR_CODE = "ATTR_CODE";
        public const string ATTR_UNIT = "UNIT";
        public const string ATTR_CREATE_DATE = "CREATE_DATE_STR";
        public const string ATTR_CARE_ID = "CARE_ID";
        public const string ATTR_ATTR_ID = "CARE_ATTR_ID";

        private JArray Data = new JArray();

        public DateTime Recorded {
            get {
                string date = string.Format("{0} {1}:{2}", this[KEY_RECORD_DATE].ToString(), this[KEY_RECORD_HH].ToString(), this[KEY_RECORD_MM].ToString());
                return DateTime.ParseExact(date, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture);
            }
            set {
                this[KEY_RECORD_DATE] =string.Format("{0:yyyyMMdd}", value);
                this[KEY_RECORD_HH]=string.Format("{0:HH}", value);
                this[KEY_RECORD_MM]=string.Format("{0:mm}", value);
            }
        }

        public string RecordedDate {
            get {
                return this[KEY_RECORD_DATE].ToString();
            }
        }
        
        public string RecordedHH {
            get {
                return this[KEY_RECORD_HH].ToString();
            }
        }
        
        public string RecordedMM {
            get {
                return this[KEY_RECORD_MM].ToString();
            }
        }
        
        public float Height {
            get {
                return getData(KEY_HEIGHT);
            }
            set {
                addData(KEY_HEIGHT, value.ToString());
            }
        }

        public float Weight {
            get {
                return getData(KEY_WEIGHT);
            }
            set {
                addData(KEY_WEIGHT, value.ToString());
            }
        }

        public float Temperature {
            get {
                return getData(KEY_TEMPERATURE);
            }
            set {
                addData(KEY_TEMPERATURE, value.ToString());
            }
        }

        public float BloodSugarAC {
            get {
                return getData(KEY_BLOOD_SUGAR_AC);
            }
            set {
                addData(KEY_BLOOD_SUGAR_AC, value.ToString());
            }
        }

        public float BloodSugarPC {
            get {
                return getData(KEY_BLOOD_SUGAR_PC);
            }
            set {
                addData(KEY_BLOOD_SUGAR_PC, value.ToString());
            }
        }
        public float PBF {
            get {
                return getData(KEY_PBF);
            }
            set {
                addData(KEY_PBF, value.ToString());
            }
        }

        public float Diastolic {
            get {
                return getData(KEY_DIASTOLIC);
            }
            set {
                addData(KEY_DIASTOLIC, value.ToString());
            }
        }
        public float Systolic {
            get {
                return getData(KEY_SYSTOLIC);
            }
            set {
                addData(KEY_SYSTOLIC, value.ToString());
            }
        }

        public int Pulse {
            get {
                return (int)getData(KEY_PULSE);
            }
            set {
                addData(KEY_PULSE, value.ToString());
            }
        }

        public float BMI {
            get {
                return getData(KEY_BMI);
            }
            set {
                addData(KEY_BMI, value.ToString());
            }
        }

        public float BMR {
            get {
                return getData(KEY_BMR);
            }
            set {
                addData(KEY_BMR, value.ToString());
            }
        }

        public float BODY_FAT {
            get {
                return getData(KEY_BODY_FAT);
            }
            set {
                addData(KEY_BODY_FAT, value.ToString());
            }
        }

        public float FFM {
            get {
                return getData(KEY_FFM);
            }
            set {
                addData(KEY_FFM, value.ToString());
            }
        }

        public float TBW {
            get {
                return getData(KEY_TBW);
            }
            set {
                addData(KEY_TBW, value.ToString());
            }
        }



        public string Unit {
            get {
                return getString(ATTR_UNIT);
            }
            set {
                addData(ATTR_UNIT, value.ToString());
            }
        }

        public String Name {
            get {
                return getString(ATTR_NAME);
            }
            set {
                addData(ATTR_NAME, value.ToString());
            }
        }

        public String AttrID {
            get {
                return getString(ATTR_ATTR_ID);
            }
            set {
                addData(ATTR_ATTR_ID, value.ToString());
            }
        }

        public String CareID {
            get {
                return getString(ATTR_CARE_ID);
            }
            set {
                addData(ATTR_CARE_ID, value.ToString());
            }
        }

        public String Value {
            get;
            set;
        }

        public xcPhsMeasurementRecord() {
            Recorded = DateTime.Now;
            this.Add(KEY_DATA, Data);
        }

        public xcPhsMeasurementRecord(String dataSource, JObject jRecord) {
            this.Add(KEY_DATA, Data);
            switch (dataSource) {
                case "mihealth":
                    //long registeredAt = jRecord["registeredAt"].ToObject<long>();
                    this.Recorded = xcDateTime.fromDateString(jRecord["registeredAt"].ToString()).ToLocalTime();
                    if (jRecord["properties"] != null) {
                        JObject properties = jRecord["properties"].ToObject<JObject>();
                        foreach(JProperty property in properties.Properties()) {
                            
                            switch (property.Name.ToLower()) {
                                case "height":
                                    this.Height = property.Value.ToObject<float>(); ;
                                    break;
                                case "weight":
                                    this.Weight = property.Value.ToObject<float>(); ;
                                    break;
                                case "bmi":
                                    this.BMI = property.Value.ToObject<float>(); ;
                                    break;
                                case "systolic":
                                    this.Systolic = property.Value.ToObject<float>(); ;
                                    break;
                                case "diastolic":
                                    this.Diastolic = property.Value.ToObject<float>(); ;
                                    break;
                                case "pulse":
                                    this.Pulse = property.Value.ToObject<int>(); ;
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        public xcPhsMeasurementRecord(xcBpRecord bpRecord) {
            Recorded = DateTime.Now;
            this.Add(KEY_DATA, Data);
            setRecord(bpRecord);
        }

        public xcPhsMeasurementRecord(xcFatRecord fatRecord) {
            Recorded = DateTime.Now;
            this.Add(KEY_DATA, Data);
            setRecord(fatRecord);
        }

        public xcPhsMeasurementRecord(xcFatCompositionRecord fatCompositionRecord) {
            Recorded = DateTime.Now;
            this.Add(KEY_DATA, Data);
            setRecord(fatCompositionRecord);
        }

        public xcPhsMeasurementRecord(JObject jRecord) {
            this.Add(KEY_DATA, Data);

            string key = jRecord[ATTR_CODE].ToString();
            string value = jRecord[ATTR_VALUE].ToString();
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
                Value = value;
                addData(key, value);
            }
            if (jRecord[ATTR_UNIT] != null)
                Unit = jRecord[ATTR_UNIT].ToString();
            if (jRecord[ATTR_NAME] != null)
                Name = jRecord[ATTR_NAME].ToString();
            if (jRecord[ATTR_CREATE_DATE] != null)
                Recorded = jRecord[ATTR_CREATE_DATE].ToObject<DateTime>();
            if (jRecord[ATTR_ATTR_ID] != null)
                AttrID = jRecord[ATTR_ATTR_ID].ToString();
            if (jRecord[ATTR_CARE_ID] != null)
                CareID = jRecord[ATTR_CARE_ID].ToString();
        }

        public void setRecord(xcBpRecord bpRecord) {
            this.Systolic = bpRecord.Systolic;
            this.Diastolic = bpRecord.Diastolic;
            this.Pulse = bpRecord.Pulse;
        }

        public void setRecord(xcFatRecord fatRecord) {
            this.Height = fatRecord.Height;
            this.Weight = fatRecord.Weight;
            this.BMI = fatRecord.BMI;
        }

        public void setRecord(xcFatCompositionRecord fatCompositionRecord) {
            if (fatCompositionRecord == null)
                return;

            this.Height = fatCompositionRecord.Height;
            this.Weight = fatCompositionRecord.Weight;
            this.BMI = fatCompositionRecord.BMI;
            this.BMR = fatCompositionRecord.BmrByKCal;
            this.PBF = fatCompositionRecord.BodyFatPercentage;
            this.BODY_FAT = fatCompositionRecord.FatMass;
            this.FFM = fatCompositionRecord.FatFreeMass;
            this.TBW= fatCompositionRecord.BodyWaterMass;

        }

        private void addData(string key, string value) {
            JObject jObject;
            foreach (JToken token in Data) {
                jObject = (JObject)token;
                if (jObject[KEY_CODE].Equals(key)) {
                    jObject[KEY_VALUE] = value;
                    return;
                }
            }

            jObject = new JObject();
            jObject[KEY_CODE] = key;
            jObject[KEY_VALUE] = value;
            Data.Add(jObject);
        }

        private string getString(string key) {
            JObject jObject;
            foreach (JToken token in Data) {
                jObject = (JObject)token;
                if (jObject[KEY_CODE].ToString().Equals(key)) {
                    return jObject[KEY_VALUE].ToString();
                }
            }
            return string.Empty;
        }

        private float getData(string key) {
            string value = getString(key);

            return string.IsNullOrEmpty(value) ? -1 : (float)xcDecimal.Parse(value);
        }

        public override string ToString() {
            return ToString(Newtonsoft.Json.Formatting.None, null);
        }

        public string ToUpdateString() {
            JObject jRootData = new JObject();
            jRootData[KEY_CID] = CareID;
            jRootData[KEY_RECORD_DATE] = RecordedDate;
            jRootData[KEY_RECORD_HH] = RecordedHH;
            jRootData[KEY_RECORD_MM] = RecordedMM;

            JArray jData = new JArray();
            jRootData[KEY_DATA] = jData;
            
            JObject jItem = new JObject();
            jItem[KEY_ID] = AttrID;
            jItem[KEY_VALUE] = Value;
            
            jData.Add(jItem);

            return jRootData.ToString(Newtonsoft.Json.Formatting.None, null);

        }

        public string ToDisableString() {
            JArray jData = new JArray();
            JObject jItem = new JObject();
            jItem[KEY_CID] = CareID;

            jData.Add(jItem);
            return jData.ToString(Newtonsoft.Json.Formatting.None, null);
        }
    }
}
