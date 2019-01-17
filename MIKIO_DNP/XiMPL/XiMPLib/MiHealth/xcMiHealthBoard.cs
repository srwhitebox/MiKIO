using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.MiPHS;
using XiMPLib.xBinding;
using XiMPLib.xNetwork;

namespace XiMPLib.MiHealth {
    public class xcMiHealthBoard : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public void updateBoard() {
            string url = xcBinder.AppProperties.MiHealthDomain + "/api/notice/importants";
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);
            String jResponse = client.requestString();
            xcMiHealthResponse response = new xcMiHealthResponse(jResponse);
            notifyChanged();
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
