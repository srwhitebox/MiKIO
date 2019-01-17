using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Kayak;
using Kayak.Http;
namespace RPGate.HttpService {
    public class RequestDelegate : IHttpRequestDelegate {

        public delegate void onHostRequestDelegate(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response);

        private onHostRequestDelegate hostRequestDelegate;

        public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response) {
            hostRequestDelegate(request, requestBody, response);
            var headers = new HttpResponseHead() {
                Status = "200 OK",
                Headers = new Dictionary<string, string>() {
                    { "Content-Type", "text/plain" },
                }
            };
            response.OnResponse(headers, requestBody);
        }

        public void setOnHostRequestDelegate(onHostRequestDelegate hostRequestDelegate) {
            this.hostRequestDelegate = hostRequestDelegate;
        }
    }
}
