using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.MiKIO;
using XiMPLib.MiPHS;
using XiMPLib.xBinding;
using XiMPLib.xNetwork;
using XiMPLib.xType;

namespace XiMPLib.MiHealth {
    public class MiHealthCareInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Domain
        {
            get
            {
                return xcBinder.AppProperties.MiHealthDomain;
            }
        }

        public AppProperties AppProperties
        {
            get
            {
                return xcBinder.AppProperties;
            }
        }

        public Patient Patient {
            get {
                return xcBinder.Patient;
            }
        }

        public DateTime RegisteredAt {
            get; set;
        }

        public String uid {
            get; set;
        }

        public String DeptId
        {
            get; set;
        }

        public String Department
        {
            get
            {
                return AppProperties.getDepartmentName(DeptId);
            }
        }

        public String RegisteredDate {
            get
            {
                return RegisteredAt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        private String diseaseCodes;
        public String Disease {
            get
            {
                if (string.IsNullOrEmpty(this.diseaseCodes))
                    return "";
                else {
                    string diseaseName = "";
                    foreach (string code in diseaseCodes.Split(',')) {
                        CheckBoxItem item = Patient.getCheckBoxItem(DeptId == "internal" ? Patient.InternalDiseaseList : Patient.SurgeryDiseaseList, code.Trim());
                        if (diseaseName.Length > 0)
                            diseaseName += ", ";
                        if (item != null) {
                            diseaseName += item.Text;
                        }else {
                            diseaseName += code.Trim();
                        }
                    }
                    return diseaseName;
                }
            }
            set
            {
                this.diseaseCodes = value;
            }
        }

        private String injuredPlaceCode;
        public String InjuredPlace
        {
            get
            {
                if (String.IsNullOrEmpty(injuredPlaceCode))
                    return "";
                String places = "";
                foreach(String code in injuredPlaceCode.Split(',')) {
                    if (places.Length > 0)
                        places += ", ";
                    string place = getTextFromCode(Patient.InjuredPlaceList, code.Trim());
                    if (string.IsNullOrEmpty(place)) {
                        places += code;
                    }
                    else {
                        places = place;
                    }
                }
                return places;
            }
            set
            {
                injuredPlaceCode = value;
            }
        }

        private String injuredPartCode;
        public String InjuredPart
        {
            get
            {
                if (String.IsNullOrEmpty(injuredPartCode))
                    return "";
                String parts = "";
                foreach (String code in injuredPartCode.Split(',')) {
                    if (parts.Length > 0)
                        parts += ", ";
                    string part = getTextFromCode(Patient.InjuredPartList, code.Trim());
                    if (string.IsNullOrEmpty(part)) {
                        parts += code;
                    }
                    else {
                        parts = part;
                    }
                }
                return parts;
            }
            set
            {
                injuredPartCode = value;
            }
            
        }

        public String Treatments
        {
            get; set;
        }

        public String Comment
        {
            get; set;
        }

        public String Wounds {
            get; set;
        }

        public String Manager {
            get; set;
        }

        public String CareID {
            get; set;
        }

        public List<CheckBoxItem> InternalDiseaseList
        {
            get
            {
                return xcBinder.Patient.InternalDiseaseList;
            }

        }

        public List<CheckBoxItem> InternalDiseaseTreatmentList
        {
            get
            {
                return xcBinder.Patient.InternalDiseaseTreatmentList;
            }
        }

        public List<CheckBoxItem> SurgeryDiseaseList
        {
            get
            {
                return xcBinder.Patient.SurgeryDiseaseList;
            }
        }

        public List<CheckBoxItem> SurgeryDiseaseTreatmentList
        {
            get
            {
                return xcBinder.Patient.SurgeryDiseaseTreatmentList;
            }
        }

        private void clear() {
            copyFrom(new MiHealthCareInfo());
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public void copyFrom(MiHealthCareInfo careInfo) {
            getTreatment(careInfo.DeptId, careInfo.uid);
            if (careInfo == null)
                this.clear();
            else
                xcObject.copyProperties(this, careInfo);
            notifyChanged();
        }

        private async void getTreatment(String deptId, String uid) {
            string url = Domain + "/api/care/treatments/" + uid;
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            string response = await client.requestAsync();

            xcMiHealthResponse mihealthResponse = response == null ? null : new xcMiHealthResponse(response);

            JArray jArray = (JArray)mihealthResponse.Data;
            foreach (JObject jRecord in jArray) {
                JToken jTreatment = jRecord["treatment"];
                if (jTreatment != null) {
                    JObject jObject = (JObject)jTreatment;
                    JToken jValue = jTreatment["treatment"];
                    if (jValue != null) {
                        StringBuilder sb = new StringBuilder();
                        String[] tokens = jValue.ToString().Split(',');
                        foreach(String token in tokens) {
                            if (sb.Length > 0) {
                                sb.Append(", ");
                            }
                            String treatment = getTextFromCode(deptId.StartsWith("internal") ? InternalDiseaseTreatmentList : SurgeryDiseaseTreatmentList, token.Trim());
                            sb.Append(treatment);
                        }
                        Treatments = sb.ToString();
                    }


                    jValue = jTreatment["comment"];
                    if (jValue != null) {
                        Comment = jValue.ToString();
                    }
                }
            }   
        }

        private String getTextFromCode(List<CheckBoxItem> list, String code) {
            if (list == null || list.Count == 0)
                return code;
            foreach(CheckBoxItem item in list) {
                if (item.Name == code) {
                    return item.Text;
                }
            }
            return code;
        }

        private async Task<xcMiHealthResponse> requestMiHealthResponse(xcWebClient client) {
            string response = await client.requestAsync();

            return response == null ? null : new xcMiHealthResponse(response);
        }
    }
}
