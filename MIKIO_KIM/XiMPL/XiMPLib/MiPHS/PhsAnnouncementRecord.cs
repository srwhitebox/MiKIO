using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xBinding;
using XiMPLib.xNetwork;
using XiMPLib.xType;

namespace XiMPLib.MiPHS {
    public class PhsAnnouncementRecord : INotifyPropertyChanged {
        public string ID {
            get;
            set;
        }

        public string CreaterID {
            get;
            set;
        }

        public DateTime CreateDate {
            get;
            set;
        }

        public string CreateDateString {
            get {
                return CreateDate.ToShortDateString();
            }
        }

        public DateTime ExpireDate {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string Type {
            get;
            set;
        }

        public string Content {
            get;
            set;
        }

        public int Total {
            get;
            set;
        }

        public string ContentUrl {
            get {
                return string.Format("{0}/ext/anmt/{1}", xcBinder.AppProperties.PhsDomain, ID);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PhsAnnouncementRecord() {

        }

        public PhsAnnouncementRecord(JObject jInfoRecord) {
            ID = jInfoRecord["id"].ToString();
            CreaterID = jInfoRecord["creator"].ToString();
            CreateDate = xcDateTime.fromTicks((double)jInfoRecord["createDate"].ToObject(typeof(double)));
            if (jInfoRecord["id"] == null) {
                ExpireDate = xcDateTime.fromTicks((double)jInfoRecord["expireDate"].ToObject(typeof(double)));
            }
            Title = jInfoRecord["title"].ToString();
            Total = (int)jInfoRecord["total"].ToObject(typeof(int));
            Type = jInfoRecord["type"].ToString();

            getContent();
            // Content = jInfoRecord["content"].ToString();
        }

        public void copyFrom(PhsAnnouncementRecord record) {
            if (record == null)
                this.clear();
            else
                xcObject.copyProperties(this, record);
            notifyChanged();
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }

            //OnPropertyChanged("ID");
            //OnPropertyChanged("CreaterID");
            //OnPropertyChanged("CreateDate");
            //OnPropertyChanged("CreateDateString");
            //OnPropertyChanged("ExpireDate");
            //OnPropertyChanged("Title");
            //OnPropertyChanged("Type");
            //OnPropertyChanged("Content");
            //OnPropertyChanged("Total");
        }

        public void clear() {
            ID = "";
            CreaterID = "";
            CreateDate = new DateTime();
            ExpireDate = new DateTime();
            Title = "";
            Type = "";
            Content = "";
            notifyChanged();
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        private async void getContent(){
            xcWebClient client = new xcWebClient(ContentUrl, xcWebClient.METHOD_GET);
            xcPhsJsonResponse phsResponse = await requestPhsResponse(client);
            this.Content = phsResponse["content"].ToString();
        }

        private async Task<xcPhsJsonResponse> requestPhsResponse(xcWebClient client) {
            string response = await client.requestAsync();

            return new xcPhsJsonResponse(response);
        }
    }
}
