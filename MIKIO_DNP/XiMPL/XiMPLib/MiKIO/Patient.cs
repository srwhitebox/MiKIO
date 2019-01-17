using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xType;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.XiMPL;
using XiMPLib.xBinding;
using XiMPLib.MiHIS;
using System.ComponentModel;
using XiMPLib.xNetwork;
using XiMPLib.MiPHS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using XiMPLib.MiHealth;

namespace XiMPLib.MiKIO {
    public class Patient : INotifyPropertyChanged {
        private const string KEY_TOKEN = "token";

        public string ID {
            get;
            set;
        }

        public string EasyCardId
        {
            get;
            set;
        }

        public string StudentID
        {
            get; set;
        }

        public string StudentUid
        {
            get;set;
        }

        public byte IdType {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public DateTime BirthDate {
            get;
            set;
        }

        public Double Age
        {
            get
            {
                var age = new DateTime(DateTime.Now.Ticks - BirthDate.Ticks);
                return age.Year + (age.Month < 6 ? 0 : 0.5);
            }
        }

        public string BirthString {
            get {
                return BirthDate.ToString("MMdd");
            }
        }


        public char Gender {
            get;
            set;
        }

        public bool IsFirst {
            get;
            set;
        }

        public string PatientNo {
            get;
            set;
        }

        public xcBpRecord BpRecord {
            get;
            set;
        }

        public xcFatRecord FatRecord {
            get;
            set;
        }

        public string EMail {
            get;
            set;
        }

        public string Password {
            get;
            set;
        }

        public Int32 Grade
        {
            get; set;
        }

        public String ClassId
        {
            get; set;
        }

        public String SeatNo
        {
            get; set;
        }

        private String classId;
        public String ClassID
        {
            get
            {
                return classId;
            }
            set
            {
                this.classId = value;
                updateStudents();
            }
        }

        public List<String> ClassIdList
        {
            get; set;
        }

        public List<String> StudentList
        {
            get; set;
        }

        public List<Patient> StudentRegisterList
        {
            get; set;
        }

        public RegInfoList RegInfoList {
            get;
            set;
        }

        public Boolean IsLoggedIn
        {
            get; set;
        }

        public List<CheckBoxItem> InternalDiseaseList {
            get; set;
        }

        public List<CheckBoxItem> InternalDiseaseTreatmentList
        {
            get; set;
        }

        public List<CheckBoxItem> SurgeryDiseaseList {
            get; set;
        }

        public List<CheckBoxItem> SurgeryDiseaseTreatmentList
        {
            get; set;
        }

        public List<CheckBoxItem> InjuredPlaceList
        {
            get; set;
        }

        public List<CheckBoxItem> InjuredPartList {
            get; set;
        }

        public String InternalDiseases {
            get {
                String value = "";
                foreach(CheckBoxItem item in InternalDiseaseList){
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Text;
                    }
                }

                return value;
            }
        }

        public String InternalDiseaseCodes
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in InternalDiseaseList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Name;
                    }
                }

                return value;
            }
        }


        public String CareID
        {
            get;
            set;
        }

        public String Nurse
        {
            get;set;
        }

        public String DeptId
        {
            get; set;
        }

        public String DeptName
        {
            get; set;
        }

        public JObject JDiesease {
            get {
                JObject jDisease = new JObject();
                jDisease.Add("disease", InternalDiseaseCodes);
                jDisease.Add("manager", Nurse);
                return jDisease;
           }
        }

        public String InjuredPlaces {
            get {
                String value = "";
                foreach (CheckBoxItem item in InjuredPlaceList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Text;
                    }
                }

                return value;
            }
        }

        public String InjuredPlaceCodes
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in InjuredPlaceList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Name;
                    }
                }

                return value;
            }
        }

        public String InternalDiseaseTreatments
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in InternalDiseaseTreatmentList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Text;
                    }
                }

                return value;
            }
        }

        public String InternalDiseaseTreatmentCodes
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in InternalDiseaseTreatmentList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Name;
                    }
                }

                return value;
            }
        }

        public Boolean hasInternalDisease
        {
            get
            {
                return !String.IsNullOrEmpty(this.InternalDiseaseCodes);
            }
        }

        public String SurgeryDiseaseTreatments
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in SurgeryDiseaseTreatmentList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Text;
                    }
                }

                return value;
            }
        }

        public String SurgeryDiseaseTreatmentCodes
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in SurgeryDiseaseTreatmentList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Name;
                    }
                }

                return value;
            }
        }

        public String InjuredPartName
        {
            get
            {
                if (String.IsNullOrEmpty(InjuredPartCode))
                    return "";

                String partNames = "";
                foreach (String code in InjuredPartCode.Split(',')) {
                    if (partNames.Length > 0)
                        partNames += ", ";
                    String partName = code.Trim();
                    foreach (CheckBoxItem item in InjuredPartList) {
                        if (item.Name == code) {
                            partName = item.Text;
                        }
                    }
                    partNames += partName;
                }
                return partNames;
            }
        }

        private String injuredPartCode;
        public String InjuredPartCode
        {
            get;set;
        }

        public String Wounds {
            get {
                String value = "";
                foreach (CheckBoxItem item in SurgeryDiseaseList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Text;
                    }
                }

                return value;
            }
        }

        public String WoundCodes
        {
            get
            {
                String value = "";
                foreach (CheckBoxItem item in SurgeryDiseaseList) {
                    if (item.IsChecked) {
                        if (value.Length > 0)
                            value += ", ";
                        value += item.Name;
                    }
                }

                return value;
            }
        }

        public JObject JSurgery {
            get {
                JObject jDisease = new JObject();
                jDisease.Add("injuredPlace", InjuredPlaceCodes);
                jDisease.Add("injuredPart", InjuredPartCode);
                jDisease.Add("disease", WoundCodes);
                jDisease.Add("manager", Nurse);
                return jDisease;
            }
        }

        public JObject JInternalDiseaseTreatment
        {
            get
            {
                if (String.IsNullOrEmpty(InternalDiseaseTreatmentCodes))
                    return null;

                JObject jTreatment = new JObject();
                jTreatment.Add("treatment", InternalDiseaseTreatmentCodes);
                if (!String.IsNullOrEmpty(Comment))
                    jTreatment.Add("comment", Comment);
                return jTreatment;
            }
        }

        public JObject JSurgeryDiseaseTreatment
        {
            get
            {
                if (String.IsNullOrEmpty(SurgeryDiseaseTreatmentCodes))
                    return null;

                JObject jTreatment = new JObject();
                jTreatment.Add("treatment", SurgeryDiseaseTreatmentCodes);
                if (!String.IsNullOrEmpty(Comment))
                    jTreatment.Add("comment", Comment);
                return jTreatment;
            }
        }

        public Boolean hasWounds
        {
            get
            {
                return !String.IsNullOrEmpty(this.WoundCodes);
            }
        }


        public String Comment
        {
            get; set;
        }


        public Patient() {
            this.IsLoggedIn = false;
            initInternalDiseaseList();
            initInternalDiseaseTreatmentList();
            initSurgeryDiseaseList();
            initSurgeryDiseaseTreatmentList();
            initSurgeryPlaceList();
            initSurgeryPartList();

            this.Nurse = "李小姐";
        }

        public void initInternalDiseaseList() {
            InternalDiseaseList = new List<CheckBoxItem>();
        }

        public void initInternalDiseaseTreatmentList() {
            InternalDiseaseTreatmentList = new List<CheckBoxItem>();
        }

        public void initSurgeryPlaceList() {
            InjuredPlaceList = new List<CheckBoxItem>();
        }

        public void initSurgeryPartList() {
            InjuredPartList = new List<CheckBoxItem>();
        }

        public void initSurgeryDiseaseList() {
            SurgeryDiseaseList = new List<CheckBoxItem>();
        }

        public void initSurgeryDiseaseTreatmentList() {
            SurgeryDiseaseTreatmentList = new List<CheckBoxItem>();
        }

        public CheckBoxItem getCheckBoxItem(List<CheckBoxItem> checkBoxList, string code) {
            foreach (CheckBoxItem item in checkBoxList) {
                if (item.Name == code) {
                    return item;
                }
            }
            return null;
        }

        public CheckBoxItem getCheckBoxItem(List<CheckBoxItem> checkBoxList, string code, String text) {
            CheckBoxItem item = getCheckBoxItem(checkBoxList, code);
            if (item == null) {
                item = new CheckBoxItem(code, text);
                checkBoxList.Add(item);
            }
            return item;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Patient(string id, DateTime birthDate, char gender, byte idType = 2) {
            set(id, birthDate, gender, idType);
        }

        public Patient(int grade, String classId, String seatNo)
        {
            this.Grade = grade;
            this.ClassId = classId;
            this.SeatNo = seatNo;
            this.IsLoggedIn = false;
        }

        public void set(string id, DateTime birthDate, char gender, byte idType = 2) {
            this.ID = id;
            this.IdType = idType;
            this.BirthDate = birthDate;
            this.Gender = Gender;
            this.Password = "";
            this.IsLoggedIn = false;
        }

        public void set(xcNHICardInfo cardInfo) {
            set(cardInfo.HolderIDN, cardInfo.BirthDate, cardInfo.Gender, 2);
            Name = cardInfo.HolderName;
        }

        public void setMihealthStudent(JObject data)
        {
            this.clear();
            
            this.Name = (String)data["name"];
            //this.Gender = ((String)data["gender"])[0];
            JObject schoolRegister = (JObject)data["registerProperties"];
            this.ID = (String)data["studentNo"];
            this.StudentUid = (String)data["studentUid"];
            notifyChanged();
        }

        public void clear() {
            this.ID = null;
            this.IdType = 0xFF;
            this.BirthDate = DateTime.MaxValue;
            this.Gender = 'M';
            this.Name = string.Empty;
            this.Password = string.Empty;
            this.ClassID = null;
            this.ClassId = null;
            this.SeatNo = null;
            this.Grade = 0;
            this.IsLoggedIn = false;
            this.Comment = null;
            this.EasyCardId = null;
            clearInternalDisease();
            clearSurgery();

            notifyChanged();
        }

        public void clearInternalDisease() {
            foreach (CheckBoxItem item in InternalDiseaseList) {
                if (item.IsChecked) {
                    item.IsChecked = false;
                    item.notifyChanged();
                }
            }

            foreach (CheckBoxItem item in InternalDiseaseTreatmentList) {
                if (item.IsChecked) {
                    item.IsChecked = false;
                    item.notifyChanged();
                }
            }

            notifyChanged();
        }

        public void clearSurgery() {
            foreach (CheckBoxItem item in InjuredPlaceList) {
                if (item.IsChecked) {
                    item.IsChecked = false;
                    item.notifyChanged();
                }
                if (item.Name == "other")
                {
                    item.IsChecked = true;
                    item.notifyChanged();
                }
            }

            foreach (CheckBoxItem item in SurgeryDiseaseList) {
                if (item.IsChecked) {
                    item.IsChecked = false;
                    item.notifyChanged();
                }
            }

            foreach (CheckBoxItem item in SurgeryDiseaseTreatmentList) {
                if (item.IsChecked) {
                    item.IsChecked = false;
                    item.notifyChanged();
                }
            }

            notifyChanged();
        }

        public void readPatientInfo() {
            xcBinder.MiHIS.GetPatient(getPatientCallback);
        }

        private void getPatientCallback(string patientNo) {
            PatientNo = patientNo;
        }

        public void loadRegInfoList() {
            if (RegInfoList == null)
                RegInfoList = new RegInfoList();
            RegInfoList.loads();
        }

        public void getBillInfo() {
            xcBinder.MiHIS.GetPatientCheckBill_Detail(PatientNo, getPatientCheckBillCallback);
        }

        private void getPatientCheckBillCallback(string response) {
            BillInfo billInfo = new BillInfo();
            billInfo.dispatchXml(response);
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

        private async void updateStudents()
        {
            if (ClassID == null)
                return;

            string url = xcBinder.AppProperties.MiHealthDomain + "/api/"+ xcBinder.AppProperties.HospitalID + "/students";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add("classId", ClassID);
            client.Add("count", "100");
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            if (mihealthResponse != null && mihealthResponse.IsSucceed)
            {
                JArray array = (JArray)mihealthResponse.Data;
                StudentList = new List<string>();
                StudentRegisterList = new List<Patient>();
                //StudentList.Clear();
                foreach(JToken token in array)
                {
                    String name = token["name"].ToString();
                    JObject schoolRegister = (JObject)token["registerProperties"];
                    String seat = schoolRegister["seat"].ToString();
                    //StudentList.Add(seat + ". " + name);

                    Patient patient = new Patient(int.Parse(token["grade"].ToString()), schoolRegister["classId"].ToString(), schoolRegister["seat"].ToString());
                    patient.Name = name;
                    StudentRegisterList.Add(patient);
                }
                StudentRegisterList = StudentRegisterList.OrderBy(o => o.SeatNo).ToList();
                foreach(Patient patient in StudentRegisterList)
                {
                    StudentList.Add(patient.SeatNo + ". "+ patient.Name);
                }
                notifyChanged();
            }
        }

        public void updateCareID() {
            string url = xcBinder.AppProperties.MiHealthDomain + "/api/care/regNo/new";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            String jResponse = client.requestString();
            xcMiHealthResponse response = new xcMiHealthResponse(jResponse);
            this.CareID = (String)response.Data;
            notifyChanged();

            updateNurses();
        }

        public void updateNurses() {
            string url = xcBinder.AppProperties.MiHealthDomain + "/api/nurses";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_TOKEN, xcBinder.MiHealth.Token);
            String jResponse = client.requestString();
            xcMiHealthResponse mihealthResponse = new xcMiHealthResponse(jResponse);
            if (mihealthResponse != null && mihealthResponse.IsSucceed) {
                JArray array = (JArray)mihealthResponse.Data;
                this.Nurse = array[0]["name"].ToString();
            }
            notifyChanged();
        }


        private async Task<xcMiHealthResponse> requestMiHealthResponse(xcWebClient client)
        {
            string response = await client.requestAsync();

            return response == null ? null : new xcMiHealthResponse(response);
        }

    }
}
