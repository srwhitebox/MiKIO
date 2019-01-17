using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MitacHis
{
    public class STheseHis
    {
        /// <summary>
        /// Hostpital ID
        /// ex) salesdemo
        /// </summary>
        public string HospitalId {
            get;
            set;
        }

        /// <summary>
        /// Language
        /// ex) zh-TW
        /// </summary>
        public string Language {
            get;
            set;
        }

        /// <summary>
        /// Authentification Key
        /// ex) 123456
        /// </summary>
        public string AuthKey {
            get;
            set;
        }

        public STheseHis() {
            HospitalId = "salesdemo";
            AuthKey = "123456";
            Language = "zh-TW";
        }

        public STheseHis(string hospitalId, string language) {
            this.HospitalId = hospitalId;
            this.Language = language;
        }

        public STheseHis(string HospitalID) {
            this.Language = "zh-TW";
        }

        public string AccessPatientDetailInfo() {
            return string.Empty;
        }

        public string AccessPhysiologicalMeasurementValues() {
            return string.Empty;
        }

        public string ArrivedCheckIn() {
            return string.Empty;
        }

        public string DoFirstReg() {
            return string.Empty;
        }

        public string DoReferral() {
            return string.Empty;
        }

        public string DoReg() {
            return string.Empty;
        }

        public string DoReqCancel() {
            return string.Empty;
        }

        public string GetBookingPatients(){
            return string.Empty;
        }

        public string GetDept(){
            return string.Empty;
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

        public string GetOPDProgress(string roomId, string opdTime) {
            string url = GetUrl("GetOPDProgress", roomId, opdTime);
            return string.Empty;
        }

        public string GetOPDSchdule(){
            return string.Empty;
        }

        public string GetPatient(){
            return string.Empty;
        }

        public string GetPatientAllHospitialRegistered(){
            return string.Empty;
        }

        public string GetPatientAllHospitialRegistered_2(){
            return string.Empty;
        }

        public string GetPatientCheckBill_Detail(){
            return string.Empty;
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

        public string PatientCheckBill() {
            return string.Empty;
        }

        public string ReqQuery() {
            return string.Empty;
        }

        public string ReqQueryByDate() {
            return string.Empty;
        }

        private string GetUrl(string api, string roomId, string opdTime) {
            return string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&RoomID={5}&opdTimeID={6}", "https://thcloud.mihis.com.tw/THESEHisSRV/STheseHis.asmx", api, Language, HospitalId, AuthKey, roomId, opdTime);
        }
    }
}
