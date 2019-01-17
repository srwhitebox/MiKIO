using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

using Newtonsoft.Json.Linq;
using XiMPLib.MiPHS;
using XiMPLib.XiMPL;
using XiMPLib.xNetwork;
using XiMPLib.xBinding;
using XiMPLib.xDevice.xBpMeter;
using Newtonsoft.Json;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.MiKIO;
using XiMPLib.xType;
using XiMPLib.MiHealth;
using System.ComponentModel;
using XiMPLib.xUI;
using System.Windows.Data;

namespace XiMPLib.MiPHS
{
    /// <summary>
    /// PHS API
    /// </summary>
    public class xcMiHealth : INotifyPropertyChanged {
        private const string KEY_USERNAME = "username";
        private const string KEY_GRADE = "grade";
        private const string KEY_CAMPUS_ID = "campusId";
        private const string KEY_USER_UID = "userUid";
        private const string KEY_CLASS_ID = "classId";
        private const string KEY_UID = "uid";
        private const string KEY_CARE_UID = "careUid";
        private const string KEY_CLASSROOM = "classRoom";
        private const string KEY_SEAT = "seat";
        private const string KEY_BIRTHDATE = "birthDate";
        private const string KEY_PASSWORD = "password";
        private const string KEY_TOKEN= "token";
        private const string KEY_DATA = "data";
        private const string KEY_PROPERTIES = "properties";
        private const string KEY_TREATMENT = "treatment";
        private const string KEY_CODE = "code";
        private const string KEY_NURSE_UID = "nurseUid";
        private const string KEY_DEPARTMENT = "deptId";
        private const string KEY_REG_NO = "regNo";
        private const string KEY_START_DATE = "startDate";
        private const string KEY_END_DATE = "endDate";
        private const string KEY_REGISTERED_AT = "registeredAt";

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Domain
        /// </summary>
        public string Domain {
            get {
                return xcBinder.AppProperties.MiHealthDomain;
            }
        }

        public string SchoolId
        {
            get
            {
                return xcBinder.AppProperties.HospitalID;
            }

        }

        /// <summary>
        /// Token which achieved after login
        /// </summary>
        public string Token {
            get;
            set;
        }

        public bool IsLoggedIn {
            get {
                return Token != null && !string.IsNullOrEmpty(Token);
            }
        }

        public string ID {
            get {
                if (Patient == null)
                    return null;
                return Patient.ID;
            }
        }

        public string Password {
            get {
                if (Patient == null)
                    return null;
                return Patient.BirthString;
            }
        }

        public XiMPLib.MiKIO.Patient Patient
        {
            get
            {
                return xcBinder.Patient;
            }
        }

        public Nurse Nurse
        {
            get; set;
        }

        public List<BmiStandard> BmiStandards
        {
            get;set;
        }

        public Dictionary<String, CheckBoxItem> InternalDiseaseMap
        {
            get; set;
        }

        public Dictionary<String, CheckBoxItem> SurgeryDiseaseMap
        {
            get; set;
        }

        public Dictionary<String, CheckBoxItem> InjuredPlaceMap
        {
            get; set;
        }

        public List<MiHealthCareInfo> CareRecords
        {
            get; set;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public xcMiHealth() {
            CareRecords = new List<MiHealthCareInfo>();
            Nurse = new Nurse();
            if (!String.IsNullOrEmpty(Domain))
            {
                loadNurses();
                loadClassIds();
                loadBmiStandards();
                loadInternalDiseases();
                loadSurgeryDiseases();
                loadInjuredPlaces();
            }
        }

        public async void loadNurses() {
            
            string url = Domain + "/api/campus/nurses";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add("id", SchoolId);

            xcMiHealthResponse mihealthResponse = null;
            try {
                mihealthResponse = await requestMiHealthResponse(client);
                if (mihealthResponse != null && mihealthResponse.IsSucceed) {
                    JArray array = (JArray)mihealthResponse.Data;
                    Nurse.set(array);
                    Nurse.notifyChanged();
                }
            }
            catch(Exception e) {

            }
        }

        public async void loadClassIds()
        {
            string url = Domain + "/api/" + SchoolId + "/classIds";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            if (mihealthResponse != null && mihealthResponse.IsSucceed)
            {
                JArray array = (JArray)mihealthResponse.Data;
                Patient.ClassIdList = JsonConvert.DeserializeObject<List<String>>(mihealthResponse.Data.ToString());
                Patient.notifyChanged();
            }
        }

        public async void loadBmiStandards()
        {
            string url = Domain + "/api/bmi/list";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            if (mihealthResponse != null && mihealthResponse.IsSucceed)
            {
                BmiStandards = new List<BmiStandard>();
                JArray array = (JArray)mihealthResponse.Data;
                foreach (var item in array)
                {
                    BmiStandards.Add(new BmiStandard((JObject)item));
                }
            }
        }

        public int getBmiLevel(xcFatRecord record)
        {
            if (BmiStandards == null || BmiStandards.Count == 0)
                return 0;

            BmiStandard bmiStandard = null;
            foreach(var standard in BmiStandards)
            {
                if (standard.Age == Patient.Age) {
                    bmiStandard = standard;
                    break;
                }
            }
            if (bmiStandard == null)
                bmiStandard = BmiStandards[BmiStandards.Count-1];
            return bmiStandard.getLevel(Patient.Gender+"", record.BMI);
        }


        public async void loadInternalDiseases() {
            string url = Domain + "/api/property/internalDisease";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            InternalDiseaseMap = new Dictionary<string, CheckBoxItem>();
            if (mihealthResponse != null && mihealthResponse.IsSucceed) {
                JArray array = (JArray)mihealthResponse.Data;
                foreach(var item in array) {
                    JObject jItem = (JObject)item;
                    String code = (String)jItem["code"];
                    JObject jMessage = (JObject)jItem["properties"];
                    String message = (String)jMessage["zh-TW"];
                    InternalDiseaseMap.Add(code, new CheckBoxItem(code, message));
                }
                
            }
        }

        public async void loadSurgeryDiseases() {
            string url = Domain + "/api/property/surgeryDisease";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            SurgeryDiseaseMap = new Dictionary<string, CheckBoxItem>();
            if (mihealthResponse != null && mihealthResponse.IsSucceed) {
                JArray array = (JArray)mihealthResponse.Data;
                foreach (var item in array) {
                    JObject jItem = (JObject)item;
                    String code = (String)jItem["code"];
                    JObject jMessage = (JObject)jItem["properties"];
                    String message = (String)jMessage["zh-TW"];
                    SurgeryDiseaseMap.Add(code, new CheckBoxItem(code, message));
                }

            }
        }

        public async void loadInjuredPlaces() {
            string url = Domain + "/api/property/injuredPlace";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            InjuredPlaceMap = new Dictionary<string, CheckBoxItem>();
            if (mihealthResponse != null && mihealthResponse.IsSucceed) {
                JArray array = (JArray)mihealthResponse.Data;
                foreach (var item in array) {
                    JObject jItem = (JObject)item;
                    String code = (String)jItem["code"];
                    JObject jMessage = (JObject)jItem["properties"];
                    String message = (String)jMessage["zh-TW"];
                    InjuredPlaceMap.Add(code, new CheckBoxItem(code, message));
                }

            }
        }

        public async void updateCareRecords() {
            string url = Domain + "/api/care/list";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_CAMPUS_ID, SchoolId);
            client.Add(KEY_TOKEN, Token);
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            if (mihealthResponse != null && mihealthResponse.IsSucceed) {
                JArray array = (JArray)mihealthResponse.Data;
                foreach (var item in array) {
                    MiHealthCareInfo info = new MiHealthCareInfo();
                    JObject jItem = (JObject)item;
                    info.uid = (String)jItem["dataUid"];
                    info.DeptId = (String)jItem["deptId"];
                    info.CareID = (String)jItem["regNo"];
                    info.Manager = (String)jItem["nurseName"];
                    JObject jProperties = (JObject)jItem["properties"];
                    info.Disease = (String)jProperties["disease"];
                    if (info.DeptId.Equals("surgery")) {
                        if (String.IsNullOrEmpty(info.Disease))
                            info.Disease = (String)jProperties["wounds"];
                        info.InjuredPart = (String)jProperties["injuredPart"];
                        info.InjuredPlace = (String)jProperties["injuredPlace"];
                    }
                    //info.Manager = (String)jProperties["manager"];
                    try {
                        info.RegisteredAt = xcDateTime.fromDateString((string)jItem["registeredAt"]).ToLocalTime();
                    }
                    catch {

                    }
                    CareRecords.Add(info);
                }
            }

            foreach (object control in BoundedList) {
                if (control.GetType().Equals(typeof(xcDataGrid))) {
                    refreshDataGrid((xcDataGrid)control);
                }
            }
        }


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        /// <param name="loginCallback"></param>
        /// <param name="timeout"></param>
        public async void login(Action<bool> loginCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/api/{1}", Domain, SchoolId);
            if (!String.IsNullOrEmpty(Patient.ID)) { // NHI card login
                url = String.Join("/", url, "nhiCard", Patient.ID, Patient.BirthString);
            }
            else if (!String.IsNullOrEmpty(Patient.EasyCardId)) {
                url = String.Join("/", url, "easyCard", Patient.EasyCardId);
            } else if (!string.IsNullOrEmpty(Patient.Name)) {
                var indexDot = Patient.Name.IndexOf('.');
                if (indexDot < 1) {
                    if (loginCallback != null)
                        loginCallback(false);
                    return;
                }

                Patient.SeatNo = Patient.Name.Substring(0, indexDot);
                url = String.Join("/", url, "login", Patient.ClassID, Patient.SeatNo, Patient.Password);
            } else { 
                if (loginCallback != null)
                    loginCallback(false);
                return;
            }

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            
            xcMiHealthResponse mihealthResponse = await requestMiHealthResponse(client);
            if (mihealthResponse != null && mihealthResponse.IsSucceed)
            {
                Token = mihealthResponse.Token;

                // Get User Info
                url = string.Format("{0}/api/{1}/student", Domain, SchoolId);
                client = new xcWebClient(url, xcWebClient.METHOD_GET);
                client.Add(KEY_TOKEN, Token);
                mihealthResponse = await requestMiHealthResponse(client);
                if (mihealthResponse != null && mihealthResponse.IsSucceed)
                {
                    Patient.clear();
                    JObject data = (JObject)mihealthResponse.Data;
                    Patient.Name = (String)data["name"];
                    Patient.Gender = ((String)data["gender"])[0];
                    Patient.BirthDate = DateTime.Parse((String)data["birthDate"]);
                    JObject schoolRegister = (JObject)data["registerProperties"];
                    Patient.ID = (String)data["studentNo"]; //schoolRegister.GetValue("guyId").ToString();
                }
            }else {
                if (!String.IsNullOrEmpty(Patient.ID)) { // NHI card login
                    
                }
                else if (!String.IsNullOrEmpty(Patient.EasyCardId)) {
                    System.Windows.MessageBox.Show(Patient.EasyCardId + "\nThis card is not registered", "Login error");
                }
            }

            if (loginCallback != null) {
                Patient.IsLoggedIn = mihealthResponse != null && mihealthResponse.IsSucceed;
                Patient.notifyChanged();

                loginCallback(Patient.IsLoggedIn);
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="logoutCallback"></param>
        /// <param name="timeout"></param>
        public async void logout(Action<bool> logoutCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!IsLoggedIn) {
                if (logoutCallback != null)
                    logoutCallback(false);
                return;
            }

            string url = string.Format("{0}/api/{1}/logout/{2}", Domain, SchoolId, Token);
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);
            Token = null;
            Patient.clear();
            Patient.notifyChanged();
            if (logoutCallback != null) {
                logoutCallback(phsResponse.IsSucceed);
            }
        }

        List<xcDataGrid> BoundedList = new List<xcDataGrid>();
        internal void addBoundedList(xcDataGrid dataGrid) {
            BoundedList.Add(dataGrid);
        }

        private delegate void RefreshDatagridDelegate(xcDataGrid dataGrid);
        private void refreshDataGrid(xcDataGrid dataGrid) {
            if (dataGrid.Dispatcher.CheckAccess()) {
                dataGrid.Items.Refresh();
                if (dataGrid.IsVisible) {
                    var cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                    cv.Filter = dataGrid.Filter != null ? dataGrid.Filter : null;
                }
            }
            else {
                dataGrid.Dispatcher.BeginInvoke(new RefreshDatagridDelegate(refreshDataGrid), dataGrid);
            }
        }

        /// <summary>
                 /// Refresh Token
                 /// </summary>
                 /// <param name="refreshCallback"></param>
                 /// <param name="timeout"></param>
        public async void refreshToken(Action<bool> refreshCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!IsLoggedIn) {
                if (refreshCallback!=null)
                    refreshCallback(false);
                return;
            }

            string url = string.Format("{0}/au/{1}", Domain, "refresh_token");
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_TOKEN, this.Token);
            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);

            if (phsResponse != null && phsResponse.IsSucceed) {
                Token = phsResponse.Token;
            } else
                return;

            if (refreshCallback != null) {
                refreshCallback(phsResponse.IsSucceed);
            }
        }

        /// <summary>
        /// Add PHS record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="addRecordCallback"></param>
        /// <param name="timeout"></param>
        public async void addMeasurementRecord(xcFatRecord record, Action<bool> addRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!xcBinder.IsBatchMode && !IsLoggedIn) {
                if (addRecordCallback != null)
                    addRecordCallback(false);
                return;
            }
             
            string url = string.Format("{0}/api/measurement/save", Domain);

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);
            client.Add(KEY_CAMPUS_ID, SchoolId);
            if (!string.IsNullOrEmpty(Patient.StudentUid))
                client.Add(KEY_USER_UID, Patient.StudentUid);
            client.Add(KEY_PROPERTIES, record.ToString());
            client.Add(KEY_TOKEN, this.Token);
            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);
            
            if (addRecordCallback != null && phsResponse != null)
                addRecordCallback(phsResponse.IsSucceed);            
        }

        public async void addMeasurementRecord(xcBpRecord record, Action<bool> addRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!IsLoggedIn) {
                if (addRecordCallback != null)
                    addRecordCallback(false);
                return;
            }

            string url = string.Format("{0}/api/measurement/save", Domain);

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);
            client.Add(KEY_CAMPUS_ID, SchoolId);
            if (!string.IsNullOrEmpty(Patient.StudentUid))
                client.Add(KEY_USER_UID, Patient.StudentUid);
            client.Add(KEY_PROPERTIES, record.ToString());
            client.Add(KEY_TOKEN, this.Token);
            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);

            if (addRecordCallback != null && phsResponse != null)
                addRecordCallback(phsResponse.IsSucceed);
        }

        public async void addMeasurementRecord(String leftEye, String rightEye, String leftEyeCorrective = null, String rightEyeCorrective = null, Action<bool> addRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!xcBinder.IsBatchMode && !IsLoggedIn) {
                if (addRecordCallback != null)
                    addRecordCallback(false);
                return;
            }

            string url = string.Format("{0}/api/measurement/save", Domain);

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);

            JObject jEyeGrade = new JObject();
            if (!String.IsNullOrEmpty(leftEye))
                jEyeGrade.Add("leftEye", leftEye);
            if (!String.IsNullOrEmpty(rightEye))
                jEyeGrade.Add("rightEye", rightEye);
            if (!String.IsNullOrEmpty(leftEyeCorrective))
                jEyeGrade.Add("leftEyeCorrective", leftEyeCorrective);
            if (!String.IsNullOrEmpty(rightEyeCorrective))
                jEyeGrade.Add("rightEyeCorrective", rightEyeCorrective);
            client.Add(KEY_CAMPUS_ID, SchoolId);
            if (!string.IsNullOrEmpty(Patient.StudentUid))
                client.Add(KEY_USER_UID, Patient.StudentUid);
            client.Add(KEY_PROPERTIES, jEyeGrade.ToString());
            client.Add(KEY_TOKEN, this.Token);
            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);

            if (addRecordCallback != null && phsResponse != null)
                addRecordCallback(phsResponse.IsSucceed);
        }

        /// <summary>
        /// Read PHS record by item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="getRecordCallback"></param>
        /// <param name="timeout"></param>
        public async void getMeasurementRecord(string item, DateTime from, DateTime to, Action<List<xcPhsMeasurementRecord>> getRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!IsLoggedIn) {
                if (getRecordCallback != null)
                    getRecordCallback(null);
                return;
            }
            
            string url = string.Format("{0}/api/measurement/list", Domain, SchoolId);

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_CAMPUS_ID, SchoolId);
            client.Add(KEY_TOKEN, Token);
            String miHealthItem = "weight";
            switch (item) {
                case "BODY_HEIGHT":
                    miHealthItem = "height";
                    break;
                case "BODY_WEIGHT":
                    miHealthItem = "weight";
                    break;
                case "PULSE":
                    miHealthItem = "pulse";
                    break;
                case "SYSTOLIC_RIGHT":
                    miHealthItem = "systolic";
                    break;
                case "DIASTOLIC_RIGHT":
                    miHealthItem = "diastolic";
                    break;
            }
            client.Add("item", miHealthItem);
            client.Add("from", xcDateTime.toDateString(from.ToUniversalTime()));
            client.Add("to", xcDateTime.toDateString(to.AddDays(1).ToUniversalTime()));
            client.Add("dateFormat", "yyyyMMdd");
            List<xcPhsMeasurementRecord> phsRecords = await requestPhsRecords(client);
            if (getRecordCallback != null)
                getRecordCallback(phsRecords);
        }

        /// <summary>
        /// Update PHS record for each item
        /// </summary>
        /// <param name="record"></param>
        /// <param name="updateCallback"></param>
        /// <param name="timeout"></param>
        public async void updateMeasurementRecord(xcPhsMeasurementRecord record, Action<bool> updateCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!IsLoggedIn) {
                if (updateCallback != null)
                    updateCallback(false);
                return;
            }

            string url = string.Format("{0}/m/pe/{1}", Domain, "update");

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);
            client.add(record);

            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);

            if (phsResponse != null && phsResponse.IsSucceed) {
                Token = phsResponse.Token;
            } else
                return;

            if (updateCallback != null)
                updateCallback(phsResponse.IsSucceed);            
        }

        /// <summary>
        /// Request and get PHS Response
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<xcMiHealthResponse> requestMiHealthResponse(xcWebClient client) {
            string response = await client.requestAsync();

            return response==null ? null : new xcMiHealthResponse(response);
        }

        public async void addCareRecord(String department, Action<bool> addRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!IsLoggedIn) {
                if (addRecordCallback != null)
                    addRecordCallback(false);
                return;
            }

            string url = string.Format("{0}/api/care/add", Domain);

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);

            JObject jData = new JObject();
            jData.Add(KEY_TOKEN, this.Token);
            jData.Add(KEY_CAMPUS_ID, SchoolId);
            jData.Add(KEY_DEPARTMENT, department);
            jData.Add(KEY_REG_NO, Patient.CareID);
            Nurse.setDept(department);
            if (Nurse.NurseUid != null)
                jData.Add(KEY_NURSE_UID, Nurse.NurseUid);
            jData.Add(KEY_PROPERTIES, department.Equals("surgery") ? Patient.JSurgery : Patient.JDiesease);

            client.Data = jData.ToString();

            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);
            if (phsResponse != null) {
                string careUid = ((JObject)phsResponse.Data)["uid"].ToString();
                addTreatmentRecord(careUid, department, addRecordCallback, timeout);
            }else {

            }


            Patient.Comment = null;

            if (department.Equals("surgery"))
                Patient.clearSurgery();
            else
                Patient.clearInternalDisease();
            
            if (addRecordCallback != null && phsResponse != null)
                addRecordCallback(phsResponse.IsSucceed);
        }

        public async void addTreatmentRecord(String careUid, String department, Action<bool> addRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            JObject jTreatment = department.Equals("surgery") ? Patient.JSurgeryDiseaseTreatment : Patient.JInternalDiseaseTreatment;
            if (jTreatment == null)
                return;

            string url = string.Format("{0}/api/care/treatment/save", Domain);

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);

            JObject jData = new JObject();
            jData.Add(KEY_TOKEN, this.Token);
            jData.Add(KEY_CARE_UID, careUid);
            jData.Add(KEY_NURSE_UID, Nurse.NurseUid);
            jData.Add(KEY_TREATMENT, jTreatment);

            client.Data = jData.ToString();

            xcMiHealthResponse phsResponse = await requestMiHealthResponse(client);
            if (addRecordCallback != null && phsResponse != null) {
                addRecordCallback(phsResponse.IsSucceed);
            }
        }

 
        /// <summary>
        /// Request and get PHS Record List
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<List<xcPhsMeasurementRecord>> requestPhsRecords(xcWebClient client) {
            string response = await client.requestAsync();

            return new xcPhsMeasurementRecordResponse("mihealth", response).List;
        }

        private async Task<string> requestPhsActionRecords(xcWebClient client) {
            string response = await client.requestAsync();

            return response;
        }

        public async void getAnouncements(Action getAnouncementsCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/ext/anmt/at/EL_ACTION", Domain);
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);

            xcPhsJArrayResponse phsResponse = await requestPhsJArrayResponse(client);
            if (phsResponse != null && phsResponse.HasValues) {
                xcBinder.PhsAnnouncementList.set(phsResponse);
            }

            if (getAnouncementsCallback != null) {
                getAnouncementsCallback();
            }
        }

        private async Task<xcPhsJArrayResponse> requestPhsJArrayResponse(xcWebClient client) {
            string response = await client.requestAsync();
            if (string.IsNullOrEmpty(response))
                return null;
            try {
                return new xcPhsJArrayResponse(response);
            } catch {
                return null;
            }
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

    }
}
