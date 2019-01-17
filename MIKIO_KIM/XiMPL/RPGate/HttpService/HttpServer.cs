using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using Kayak;
using Kayak.Http;

namespace RPGate.HttpService {
    public class HttpServer {

        private IServer Server;
        private IScheduler Scheduler;
        private RequestDelegate mRequestDelegate;
        private Thread mStatusThread;
        public Boolean IsStarted {
            get;
            set;
        }

        public int Port {
            get;
            set;
        }

        public HttpServer(int port=8282) {
            Port = port;
            IsStarted = false;
            Scheduler = KayakScheduler.Factory.Create(new SchedulerDelegate());
            mRequestDelegate = new RequestDelegate();
            Server = KayakServer.Factory.CreateHttp(mRequestDelegate, Scheduler);
        }

        public HttpServer(RequestDelegate requestDelegate, SchedulerDelegate schedulerDelegate, int port=8282){
            
            Port = port;
            IsStarted = false;
            mRequestDelegate = requestDelegate;
            Scheduler = KayakScheduler.Factory.Create(schedulerDelegate);
            Server = KayakServer.Factory.CreateHttp(requestDelegate, Scheduler);
        }

        public void start() {
            if (IsStarted && mStatusThread != null)
                return;
            mStatusThread = new Thread(startService);
            mStatusThread.Start();
        }

        private void startService() {
            using (Server.Listen(new System.Net.IPEndPoint(IPAddress.Any, Port))) {
                try {
                    Scheduler.Start();
                    IsStarted = true;
                } catch (NullReferenceException e){

                }
            }
        }

        public void stop() {
            Scheduler.Stop();
            mStatusThread.Abort();
            mStatusThread.Join();
            mStatusThread = null;

            IsStarted = false;
        }

        public void setOnHostRequestDelegate(RequestDelegate.onHostRequestDelegate hostRequestDelegate) {
            mRequestDelegate.OnHostRequestDelegate = hostRequestDelegate;
        }

        public class RequestDelegate : IHttpRequestDelegate {
            public onHostRequestDelegate OnHostRequestDelegate {
                get;
                set;
            }

            public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response) {
                if (OnHostRequestDelegate != null)
                    OnHostRequestDelegate(request, requestBody, response);
            }

            public delegate void onHostRequestDelegate(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response);
        }
    }
}
