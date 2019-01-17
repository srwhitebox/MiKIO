using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xBinding;

namespace XiMPLib.MiHIS {
    public class CalledInfo : INotifyPropertyChanged {
        public string RoomNo {
            get;
            set;
        }

        public string Title {
            get {
                string id = string.Format("{0:00}", int.Parse(RoomNo));
                return xcBinder.AppProperties.RoomList[id].Title;
            }
        }

        public string CalledNo {
            get;
            set;
        }

        public string DoctorName {
            get;
            set;
        }

        public string DoctorTitle {
            get;
            set;
        }

        public string OpdTimeName {
            get;
            set;
        }

        public DateTime OpdTime {
            get;
            set;
        }

        public string StateText {
            get;
            set;
        }

        public CalledInfo(string roomNo) {
            RoomNo = roomNo;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
