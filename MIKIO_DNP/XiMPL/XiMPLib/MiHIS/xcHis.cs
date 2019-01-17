using MitacHis.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using XiMPLib.MiHIS;
using XiMPLib.MiHIS.Database;
using XiMPLib.MiKIO;
using XiMPLib.Type;
using XiMPLib.xBinding;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.XiMPL;
using XiMPLib.xType;

namespace MitacHis
{
    public class xcHis {
        private AppProperties AppProperties {
            get {
                return xcBinder.AppProperties;
            }
        }
        private string Domain{
            get {
                return AppProperties.HisDomain;
            }
        }
        
        /// <summary>
        /// Hostpital ID
        /// ex) salesdemo
        /// </summary>
        public string HospitalId {
            get {
                return AppProperties.HospitalID;
            }
        }

        /// <summary>
        /// Language
        /// ex) zh-TW
        /// </summary>
        public string Language {
            get {
                return AppProperties.Language;
            }
        }

        /// <summary>
        /// Authentification Key
        /// ex) 123456
        /// </summary>
        public string AuthKey {
            get {
                return AppProperties.HospitalAuthKey;
            }
        }

        public int OpdTimeID {
            get {
                return OpdTimeList.CurOpdTime != null ? OpdTimeList.CurOpdTime.Id : -1;
            }
        }

        public Patient Patient {
            get {
                return xcBinder.Patient;
            }
        }

        private OpdTimeList OpdTimeList {
            get {
                return OpdProgress.OpdTimeList;
            }
        }

        public OpdProgress OpdProgress;
        private Timer OpdPrgoressUpdateTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="authKey"></param>
        /// <param name="language"></param>
        public xcHis() {
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            OpdProgress = new OpdProgress();
            //ReadAllOPDProgress();
            GetOPDProgress(); 
            startUpdateOpdProgress();
        }

        public async void registMeasuredData(object record, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType={6}&birthDay={7}&opdDate={8}&isFirst={9}", Domain, "ReqQueryByDate", Language, HospitalId, AuthKey, xcBinder.Patient.ID, 2, xcBinder.Patient.BirthDate.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"), 'N');
            DataSet dsQueryResult = await getDatasetResponseAsync(url, timeout);
            if (dsQueryResult == null) {
            } else if (dsQueryResult.Tables.Count > 0) {

                DataTable tb = dsQueryResult.Tables[0];
                String caseNo = null;
                // Distpatch result...
                foreach (DataRow row in tb.Rows) {
                    RegInfo regInfo = new RegInfo(row);
                    if (string.IsNullOrEmpty(regInfo.CaseNo) && !regInfo.OpdDate.Equals(DateTime.Today))
                        continue;
                    string period = regInfo.Period;
                    TimeRange range = new TimeRange(period);
                    if (range.Start < DateTime.Now.TimeOfDay) {
                        caseNo = regInfo.CaseNo;
                    }
                        
                }

                if (!String.IsNullOrEmpty(caseNo))
                    AccessPhysiologicalMeasurementValues(DateTime.Now, caseNo, new xcHisPhysiologicaMeasuremenRecord(record));

                ////Dispatch room info
                //DataTable tb = dsQueryResult.Tables[0];
                //// Distpatch result...
                //string caseNo = tb.Rows[0]["caseno"].ToString();
                //if (string.IsNullOrEmpty(caseNo)) {
                //    return;
                //}
                //AccessPhysiologicalMeasurementValues(DateTime.Now, caseNo, new xcHisPhysiologicaMeasuremenRecord(record));

            }


        }

        public string AccessPatientDetailInfo() {
            return string.Empty;
        }

        public async void AccessPhysiologicalMeasurementValues(DateTime opdDate, string caseNo, xcHisPhysiologicaMeasuremenRecord record, Action<string> opdQueryCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType={6}&accessType=W&caseno={7}{8}", Domain, "AccessPhysiologicalMeasurementValues", Language, HospitalId, AuthKey, Patient.ID, Patient.IdType, caseNo, record.UrlParams);
            string queryResult = await downloadResponseAsync(url, timeout);
            if (string.IsNullOrEmpty(queryResult)) {

            } else  {
                // Distpatch result...
                if (opdQueryCallback != null) {
                    opdQueryCallback(queryResult);
                }
            }
        }

        public async void ArrivedCheckIn(DateTime opdDate, string opdTimeId, string roomId, Action<string> arrivalCheckinCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType=2&birthDay={6}&opdDate={7}&opdTimeID={8}&roomID={9}&isForeign=&isHCSuser=Y&isOnlyReadWebAllowed=true", Domain, "ArrivedCheckIn", Language, HospitalId, AuthKey,
                Patient.ID, Patient.BirthDate.ToString("yyyyMMdd"), opdDate.ToString("yyyyMMdd"), opdTimeId, roomId);
            string result = await downloadResponseAsync(url, timeout);

            if (string.IsNullOrEmpty(result)) {

            } else {
                if (arrivalCheckinCallback != null) {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(result);
                    string caseNo = xml.InnerText;
                    arrivalCheckinCallback(caseNo);
                }
            }
        }

        public string DoFirstReg() {
            return string.Empty;
        }

        public string DoReferral() {
            return string.Empty;
        }

        public async void DoReg(DateTime opdDate, string opdTimeId, string deptId, string doctorId, string memo="", Action<string, TimeSpan, string> doRegCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType=2&birthDay={6}&isShowName=true&opdDate={7}&opdTimeID={8}&deptID={9}&doctorID={10}&memo={11}&isOnlyReadWebAllowed=true", Domain, "DoReg", Language, HospitalId, AuthKey,
                Patient.ID, Patient.BirthDate.ToString("yyyyMMdd"), opdDate.ToString("yyyyMMdd"), opdTimeId, deptId, doctorId, memo);
            string result = await downloadResponseAsync(url, timeout);
            if (!string.IsNullOrEmpty(result)) {
                try {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(result);
                    string regNo = null;
                    TimeSpan estTime = new TimeSpan();
                    string[] innerText = xml.InnerText.Split('/');
                    foreach (string keyValue in innerText) {
                        if (!string.IsNullOrEmpty(keyValue)) {
                            string[] pair = keyValue.Split('=');
                            switch (pair[0]) {
                                case "regNO":
                                    regNo = pair[1];
                                    break;
                                case "estiTime":
                                    estTime = TimeSpan.ParseExact(pair[1], "hhmm", CultureInfo.InvariantCulture);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (doRegCallback != null) {
                        doRegCallback(regNo, estTime, null);
                    }
                } catch {
                    doRegCallback(null, new TimeSpan(), result);
                }
            } else {
                doRegCallback(null, new TimeSpan(), "同時段已在同科\n已經健保卡拿出自動登出");
            }
        }

        public async void DoReqCancel(DateTime opdDate, string opdTimeId, string deptId, string doctorId, Action<bool> doRegCancelCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType=2&birthDay={6}&opdDate={7}&opdTimeID={8}&deptID={9}&doctorID={10}", Domain, "DoReqCancel", Language, HospitalId, AuthKey,
                Patient.ID, Patient.BirthDate.ToString("yyyyMMdd"), opdDate.ToString("yyyyMMdd"), opdTimeId, deptId, doctorId);
             string result = await downloadResponseAsync(url, timeout);
             bool isCanceled = false;
             if (!string.IsNullOrEmpty(result)) {
                 if (result.Contains("true"))
                     isCanceled = true;
             }
             if (doRegCancelCallback != null) {
                 doRegCancelCallback(isCanceled);
             }
        }

        public string GetBookingPatients(){
            return string.Empty;
        }

        public async void GetDept(Action<DataTable> getDeptCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&isOnlyReadWebAllowed=true&deptGroupID=", Domain, "GetDept", Language, HospitalId, AuthKey);
            DataSet dsDept = await getDatasetResponseAsync(url, timeout);
            if (dsDept != null && getDeptCallback != null) {
                getDeptCallback(dsDept.Tables[0]);
            }
        }

        public string GetHospital(){
            return string.Empty;
        }

        public string GetHospitalInfo(){
            return string.Empty;
        }

        public string GetOPDCaseNo(){
            return string.Empty;
        }

        public void startUpdateOpdProgress(int interval = 60*1000) {
            OpdPrgoressUpdateTimer = new Timer(interval);
            OpdPrgoressUpdateTimer.Elapsed += OpdPrgoressUpdateTimer_Elapsed;
            OpdPrgoressUpdateTimer.Start();
        }

        void OpdPrgoressUpdateTimer_Elapsed(object sender, ElapsedEventArgs e) {
            refreshOpdProgress();
        }

        public void stopUpdateOpdProgress() {
            OpdPrgoressUpdateTimer.Stop();
        }

        public void refreshOpdProgress() {
            GetOPDProgress();
            //foreach(RoomInfo room in OpdProgress){
            //    GetOPDProgress(room.ID);
            //}
        }

        public async void GetOPDProgress(int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (String.IsNullOrEmpty(Domain))
                return;

            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&RoomID=&opdTimeID={5}", Domain, "GetOPDProgress", Language, HospitalId, AuthKey, OpdTimeID);
            DataSet dsOpdProgress = await getDatasetResponseAsync(url, timeout);

            if (dsOpdProgress == null) {
            } else if (dsOpdProgress.Tables.Count > 0) {
                OpdProgress.updateRoomInfo(dsOpdProgress.Tables[0]);
                //OpdTimeList.configTimeRange(dsOpdProgress.Tables[0]);
            }


//            OpdProgress.notifyAll();

            //refreshOpdProgress();

            //startUpdateOpdProgress();
            //start monitor...
        }


        public async void GetOPDProgress(string roomId, Action<RoomInfo> opdProgressCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (OpdTimeID == -1)
                return;

            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&RoomID={5}&opdTimeID={6}", Domain, "GetOPDProgress", Language, HospitalId, AuthKey, roomId, OpdTimeID);
            DataSet dsOpdProgress = await getDatasetResponseAsync(url, timeout);
            if (dsOpdProgress == null) {
                OpdProgress[roomId].CalledNo = string.Empty;
            }else if (dsOpdProgress.Tables.Count > 0) {
                //Dispatch room info
                RoomInfo curRoom = OpdProgress.updateRoomInfo(dsOpdProgress.Tables[0]);

                if (opdProgressCallback != null) {
                    opdProgressCallback(curRoom);
                }
            }
        }

        public async void GetOPDSchdule(string deptId, string doctorID, DateTime fromDate, DateTime toDate, string opdTimeID, Action<DataTable> opdGetScheduleCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (fromDate > toDate) {
                DateTime tempDate = toDate;
                toDate = fromDate;
                fromDate = tempDate;
            }
            string opdDate = string.Format("{0:yyyyMMdd}-{1:yyyyMMdd}", fromDate, toDate);
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&deptID={5}&doctorID={6}&opdDate={7}&opdTimeID={8}&isOnlyReadWebAllowed=false", Domain, "GetOPDSchdule", Language, HospitalId, AuthKey, deptId, doctorID, opdDate, opdTimeID);
            DataSet dsOpdSchedule = await getDatasetResponseAsync(url, timeout);
            if (dsOpdSchedule == null) {
            } else if (dsOpdSchedule.Tables.Count > 0) {
                //OpdTimeList.configTimeRange(dsOpdSchedule.Tables[0]);
                //Dispatch room info
                if (opdGetScheduleCallback != null) {
                    opdGetScheduleCallback(dsOpdSchedule.Tables[0]);
                }
            }
        }

        public async void GetPatient(Action<string> getPatientCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string birthString = Patient.BirthDate.ToString("yyyyMMdd");
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&birthDay={6}&idType=2", Domain, "GetPatient", Language, HospitalId, AuthKey, Patient.ID, birthString);


            string result = await downloadResponseAsync(url, timeout);
            if (result == null) {
                
            } else {
                string patientId = null;
                string patientName = null;
                bool isFirst = false;
                char gender = 'M';

                XDocument xdoc = XDocument.Parse(result, LoadOptions.None);

                string body = xdoc.FirstNode.Document.Root.FirstNode.ToString();
                string[] tokens = body.Split('/');
                foreach (string item in tokens) {
                    string[] pairs = item.Split('=');
                    if (pairs.Length != 2)
                        continue;

                    switch (pairs[0]) {
                        case "patientNO":
                            patientId = pairs[1];
                            break;
                        case "patientName":
                            patientName = pairs[1];
                            break;
                        case "isFirst":
                            isFirst = pairs[1].Equals('Y');
                            break;
                        case "gender":
                            gender = pairs[1].Equals(string.Empty) ? (char)0 : pairs[1][0];
                            break;
                    }
                }

                //Dispatch room info
                if (getPatientCallback != null) {
                    getPatientCallback(patientId);
                }
            }            
        }

        public string GetPatientAllHospitialRegistered(){
            return string.Empty;
        }

        public string GetPatientAllHospitialRegistered_2(){
            return string.Empty;
        }

        public async void GetPatientCheckBill_Detail(string patientNo, Action<string> getPatientCheckBillCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string opdDate = DateTime.Today.ToString("yyyyMMdd");
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&PatientID={5}&opdDate={6}", Domain, "GetPatientCheckBill_Detail", Language, HospitalId, AuthKey, Patient.PatientNo, opdDate);
            string result = await downloadResponseAsync(url, timeout);
            if (result == null) {

            } else {
                if (getPatientCheckBillCallback != null) {
                    getPatientCheckBillCallback(result);
                }
            }
        }

        public string GetPatientDiagnosis(){
            return string.Empty;
        }

        public string GetPatientprescriptionCode(){
            return string.Empty;
        }

        public string GetReceiptList(){
            return string.Empty;
        }

        public string GetReduPatients(){
            return string.Empty;
        }

        public string GetRegDate(){
            return string.Empty;
        }

        public string GetRegDoctor() {
            return string.Empty;
        }

        public string GetVersion() {
            return string.Empty;
        }

        public async void PatientCheckBill(string patientNo, string caseNo, int receiptNo, int receivableAmt = 0, int partialPay = 0,int medicineWgt = 0,  int selfAmt = 0, int healthInsDiff = 0, string memo = "", Action<string> patientCheckBillCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string opdDate = DateTime.Today.ToString("yyyyMMdd");
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&PatientID={5}&CaseNo={6}&ReceiptNo={7}&ReceivableAmt={8}&PartialPay={9}&MedicineWgt={10}&SelfAmt={11}&HealthInsDiff={12}&memo={13}", Domain, "PatientCheckBill", Language, HospitalId, AuthKey, 
                patientNo, caseNo, receiptNo, receivableAmt, partialPay, medicineWgt, selfAmt, healthInsDiff, memo);
            string result = await downloadResponseAsync(url, timeout);
            if (result == null) {

            } else {
                if (patientCheckBillCallback != null) {
                    int index = result.IndexOf("Y=");
                    int lastIndex = result.IndexOf("</");
                    patientCheckBillCallback(result);
                }
            }            
        }

        public async void ReqQuery(Action<DataTable> opdQueryCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType=2&birthDay={6}&opdDate={7}&isFirst=", Domain, "ReqQuery", Language, HospitalId, AuthKey, Patient.ID, Patient.BirthDate.ToString("yyyyMMdd"), DateTime.Today.ToString("yyyyMMdd"));
            DataSet dsQueryResult = await getDatasetResponseAsync(url, timeout);
            if (dsQueryResult == null) {

            } else if (dsQueryResult.Tables.Count > 0) {
                //Dispatch room info
                DataTable tb = dsQueryResult.Tables[0];
                // Distpatch result...
                if (opdQueryCallback != null) {
                    opdQueryCallback(tb);
                }
            }
        }

        public async void ReqQueryByDate(string id, DateTime birthDate, DateTime opdDate, byte idType = 2, bool isFirst = false, Action<DataTable> opdQueryCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            string url = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&idNumber={5}&idType={6}&birthDay={7}&opdDate={8}&isFirst={9}", Domain, "ReqQueryByDate", Language, HospitalId, AuthKey, id, idType, birthDate.ToString("yyyyMMdd"), opdDate.ToString("yyyyMMdd"), isFirst ? 'Y' : 'N');
            DataSet dsQueryResult = await getDatasetResponseAsync(url, timeout);
            if (dsQueryResult == null) {
                
            } else if (dsQueryResult.Tables.Count > 0) {
                //Dispatch room info
                DataTable tb = dsQueryResult.Tables[0];
                // Distpatch result...
                if (opdQueryCallback != null) {
                    opdQueryCallback(tb);
                }
            }
        }

        private static async Task<string> downloadResponseAsync(string uri, int timeOut = Global.DEFAULT_REQUEST_TIMEOUT) {
            bool cancelledOrError = false;
            string result = null;

            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new WebClient()) {
                client.DownloadDataCompleted += (sender, e) => {
                    if (e.Error != null || e.Cancelled) {
                        cancelledOrError = true;
                    } else {
                        string resultString = Encoding.UTF8.GetString(e.Result);

                        result = resultString;
                    }
                };
                client.DownloadDataAsync(new Uri(uri));
                var n = DateTime.Now;
                while (result == null && !cancelledOrError && DateTime.Now.Subtract(n).TotalMilliseconds < timeOut) {
                    await Task.Delay(100); // wait for respsonse
                }
            }
            return result;
        }

        private static async Task<DataSet> getDatasetResponseAsync(string uri, int timeOut = Global.DEFAULT_REQUEST_TIMEOUT) {
            DataSet dsResult = null;
            bool cancelledOrError = false;
            
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var client = new WebClient()) {
                client.OpenReadCompleted += (sender, e) => {
                    if (e.Error != null || e.Cancelled) {
                        cancelledOrError = true;
                    } else {
                        try {
                            Stream stream = e.Result;
                            dsResult = new DataSet();
                            dsResult.ReadXml(stream);
                            stream.Close();
                        } catch {

                        }
                    }
                };
                
                client.UseDefaultCredentials = true;
                client.OpenReadAsync(new Uri(uri));
                var n = DateTime.Now;
                while (dsResult == null && !cancelledOrError && DateTime.Now.Subtract(n).TotalMilliseconds < timeOut) {
                    await Task.Delay(100); // wait for respsonse
                }
            }
            return dsResult;
        }
    }
}
