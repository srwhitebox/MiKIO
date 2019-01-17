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

namespace XiMPLib.MiPHS
{
    /// <summary>
    /// PHS API
    /// </summary>
    public class xcPhs {
        private const string KEY_USERNAME = "username";
        private const string KEY_PASSWORD = "password";
        private const string KEY_TOKEN= "token";
        private const string KEY_DATA = "data";
        private const string KEY_CODE = "code";
        private const string KEY_START_DATE = "startDate";
        private const string KEY_END_DATE = "endDate";
       
        /// <summary>
        /// Domain
        /// </summary>
        public string Domain {
            get {
                AppProperties Properties = xcBinder.AppProperties;
                return this.IsKiosk ? Properties.PhsDomain : Properties.PhsAppDomain;
            }
        }

        /// <summary>
        /// Token which achieved after login
        /// </summary>
        public string Token {
            get;
            set;
        }

        public bool isLoggedIn {
            get {
                return Token != null && !string.IsNullOrEmpty(Token);
            }
        }

        public bool IsKiosk {
            get;
            set;
        }

        public string ID {
            get {
                if (Patient == null)
                    return null;
                return IsKiosk ? Patient.ID : Patient.EMail;
            }
        }

        public string Password {
            get {
                if (Patient == null)
                    return null;
                return IsKiosk ? Patient.BirthString : Patient.Password;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public xcPhs(bool isKiosk=true) {
            this.IsKiosk = isKiosk;
        }

        public XiMPLib.MiKIO.Patient Patient {
            get {
                return xcBinder.Patient;
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
            if (Patient == null || (string.IsNullOrEmpty(Patient.ID) && Patient.BirthDate == null)) {
                if (loginCallback != null)
                    loginCallback(false);
                return;
            }
            string url = string.Format("{0}/au/{1}", Domain, "login");
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_USERNAME, Patient.ID);
            client.Add(KEY_PASSWORD, !string.IsNullOrEmpty(Patient.Password) ? Patient.Password : Patient.BirthString);

            xcPhsJsonResponse phsResponse = await requestPhsResponse(client);
            if (phsResponse != null && phsResponse.IsSucceed) {
                Token = phsResponse.Token;
            }

            if (loginCallback != null & phsResponse !=  null) {
                loginCallback(phsResponse.IsSucceed);
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="logoutCallback"></param>
        /// <param name="timeout"></param>
        public async void logout(Action<bool> logoutCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!isLoggedIn) {
                if (logoutCallback != null)
                    logoutCallback(false);
                return;
            }

            string url = string.Format("{0}/au/{1}", Domain, "logout");
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            xcPhsJsonResponse phsResponse = await requestPhsResponse(client);

            if (logoutCallback != null) {
                logoutCallback(phsResponse.IsSucceed);
            }
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="refreshCallback"></param>
        /// <param name="timeout"></param>
        public async void refreshToken(Action<bool> refreshCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!isLoggedIn) {
                if (refreshCallback!=null)
                    refreshCallback(false);
                return;
            }

            string url = string.Format("{0}/au/{1}", Domain, "refresh_token");
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_TOKEN, this.Token);
            xcPhsJsonResponse phsResponse = await requestPhsResponse(client);

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
        public async void addMeasurementRecord(xcPhsMeasurementRecord record, Action<bool> addRecordCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT) {
            if (!isLoggedIn) {
                if (addRecordCallback != null)
                    addRecordCallback(false);
                return;
            }

            string url = string.Format("{0}/m/pe/{1}", Domain, "create");

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_DATA, record.ToString());
            client.Add(KEY_TOKEN, this.Token);
            xcPhsJsonResponse phsResponse = await requestPhsResponse(client);

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
            if (!isLoggedIn) {
                if (getRecordCallback != null)
                    getRecordCallback(null);
                return;
            }
            
            string url = string.Format("{0}/m/pe/{1}", Domain, "query");

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            client.Add(KEY_CODE, item);
            client.Add(KEY_START_DATE, from.ToString(Global.DATE_FORMAT));
            client.Add(KEY_END_DATE, to.ToString(Global.DATE_FORMAT));
            client.Add(KEY_TOKEN, this.Token);

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
            if (!isLoggedIn) {
                if (updateCallback != null)
                    updateCallback(false);
                return;
            }

            string url = string.Format("{0}/m/pe/{1}", Domain, "update");

            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_POST);
            client.add(record);

            xcPhsJsonResponse phsResponse = await requestPhsResponse(client);

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
        private async Task<xcPhsJsonResponse> requestPhsResponse(xcWebClient client) {
            string response = await client.requestAsync();

            return response==null ? null : new xcPhsJsonResponse(response);
        }

        /// <summary>
        /// Request and get PHS Record List
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<List<xcPhsMeasurementRecord>> requestPhsRecords(xcWebClient client) {
            string response = await client.requestAsync();

            if (response == null)
                return null;
            try {
                JObject jResponse = JObject.Parse(response);
                String msg = jResponse.GetValue("msg").ToString();

                if (msg.Equals("token_error") || msg.Contains("fail"))
                    return null;
            }
            catch
            {
            }
            return response == null ? null : new xcPhsMeasurementRecordResponse(response).List;
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
    }
}
