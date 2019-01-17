using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.XiMPL;

using Newtonsoft.Json.Linq;

namespace XiMPLib.xNetwork {
    /// <summary>
    /// Web Client Class
    /// It can add parameters using add..
    /// Method will be decide the parameter handling
    /// </summary>
    public class xcWebClient : NameValueCollection{
        public static string METHOD_GET = "get";
        public static string METHOD_POST = "post";

        /// <summary>
        /// Uri
        /// </summary>
        public Uri Uri {
            get;
            set;
        }

        /// <summary>
        /// URI to get
        /// It'll add the parameters to the Uri
        /// </summary>
        public Uri GetUri {
            get {
                UriBuilder builder = new UriBuilder(this.Uri);
                StringBuilder sb = new StringBuilder(builder.Query);
                foreach (string key in this.Keys) {
                    if (sb.Length > 0)
                        sb.Append("&");
                    string paramValue = string.Format("{0}={1}", key, this[key]);
                    sb.Append(paramValue);
                }
                builder.Query = sb.ToString();
                return builder.Uri;
            }
        }

        /// <summary>
        /// Request Method
        /// </summary>
        public string RequestMethod {
            get;
            set;
        }

        /// <summary>
        /// Request Timeout
        /// </summary>
        public int RequestTimeout{
            get;
            set;
        }

        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding Encoding {
            get;
            set;
        }

        public String Data
        {
            get; set;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestMethod"></param>
        /// <param name="timeout"></param>
        /// <param name="encodingName"></param>
        public xcWebClient(string uri, string requestMethod="", int timeout=Global.DEFAULT_REQUEST_TIMEOUT, string encodingName="utf-8") {
            try {
                this.Encoding = Encoding.GetEncoding(encodingName);
                this.Uri = new Uri(uri);
                this.RequestMethod = requestMethod;
                this.RequestTimeout = timeout;
            }
            catch {

            }
        }

        /// <summary>
        /// Add parameters from the JObject. But it'll add the parameters in the root
        /// </summary>
        /// <param name="jValues"></param>
        public void add(JObject jValues) {
            foreach (var token in jValues) {
                Add(token.Key, token.Value.ToString());
            }
        }

        /// <summary>
        /// Transform the respnse to Data Rows collection
        /// </summary>
        /// <returns></returns>
        public async Task<DataRowCollection> requestDataRowsAsync() {
            string response = await requestAsync();

            DataSet dataSet = new DataSet();
            dataSet.ReadXml(response);

            if (dataSet.Tables.Count == 0) {
                return null;
            }

            DataTable table = dataSet.Tables[0];
            if (table.Rows.Count == 0)
                return null;

            return table.Rows;
        }

        public String requestString() {
            using (var client = new WebClient()) {
                client.Encoding = this.Encoding;
                return client.DownloadString(GetUri);
            }
        }

        /// <summary>
        /// Requet
        /// </summary>
        /// <returns></returns>
        public async Task<string> requestAsync() {
            string result = null;
            bool cancelledOrError = false;
            using (var client = new WebClient()) {
                client.Encoding = this.Encoding;

                client.DownloadStringCompleted += (sender, e) => {
                    if (e.Error != null || e.Cancelled) {
                        cancelledOrError = true;
                    } else {
                        result = e.Result;
                    }
                };

                client.UploadStringCompleted += (sender, e) => {
                    if (e.Error != null || e.Cancelled) {
                        cancelledOrError = true;
                    } else {
                        result = e.Result;
                    }
                };

                client.UploadValuesCompleted += (sender, e) => {
                    if (e.Error != null || e.Cancelled) {
                        cancelledOrError = true;
                    } else {
                        result = Encoding.UTF8.GetString(e.Result);
                    }
                };

                if (this.Uri == null)
                    return result;

                switch (RequestMethod) {
                    case "get":
                        client.DownloadStringAsync(GetUri);
                        break;
                    case "post":
                        if (String.IsNullOrEmpty(this.Data))
                            client.UploadValuesAsync(Uri, RequestMethod, this);
                        else
                            client.UploadStringAsync(Uri, RequestMethod, this.Data);
                        break;
                    default:
                        client.UploadValuesAsync(Uri, this);
                        break;
                }

                var startTime = DateTime.Now;
                while (result == null && !cancelledOrError && DateTime.Now.Subtract(startTime).TotalMilliseconds < RequestTimeout) {
                    await Task.Delay(100); // wait for respsonse
                }
            }
            return result;
        }
    }
}
