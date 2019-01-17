using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using XiMPLib.MiKIO;
using System.Data;
using XiMPLib.MiHIS;

namespace MitacHis.Database {
    public class RoomInfo : IEquatable<RoomInfo>, IComparable<RoomInfo>, INotifyPropertyChanged {
        public string ID {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string Location {
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

        public int OpdTimeID {
            get;
            set;
        }

        public OpdTimeList OpdTimeList {
            get;
            set;
        }

        public OpdTime OpdTime {
            get {
                return OpdTimeList == null ? null : OpdTimeList[OpdTimeID];
            }
        }

        public string OpdTimeName {
            get {
                return OpdTime == null ? null : OpdTime.Name;
            }
        }

        public string SubDoctorID {
            get;
            set;
        }

        public string SubDoctorName {
            get;
            set;
        }

        public string CalledNo {
            get;
            set;
        }

        public string WaitingCounter {
            get;
            set;
        }

        public string RegistrationCounter {
            get;
            set;
        }

        public DateTime Updated {
            get;
            set;
        }

        public bool IsAvailable {
            get {
                return string.IsNullOrEmpty(CalledNo);
            }
        }

        public string StateText {
            get;
            set;
        }

        public string DefaultDoctorTitle {
            get {
                return "醫師";
            }
        }

        public RoomInfo(string id, string name="", string title="", OpdTimeList opdTimeList = null) {
            this.ID = id;
            this.Name = name;
            this.Title = string.IsNullOrEmpty(title) ? getDefaultTitme(ID) : title;
            this.OpdTimeList = opdTimeList;
            Updated = DateTime.Now;
        }

        public bool Equals(RoomInfo obj){
            foreach (var propertyInfo in typeof(RoomInfo).GetProperties()) {
                var org = propertyInfo.GetValue(this) as string;
                var target = propertyInfo.GetValue(obj) as string;
                
                if (!propertyInfo.Name.Equals("Updated"))
                    continue;
                if (org == null && target == null) {
                    continue;
                }
                if (org != null && target == null){
                    return false;
                }
                if (!org.Equals(obj))
                    return false;
            }
            return true;
        }

        public int CompareTo(RoomInfo other) {
            int compared = Updated.CompareTo(other.Updated);
            if (compared != 0) {
                compared *= -1;
            }
            return compared;
        }

        public int sortBy(RoomInfo other, string fieldName) {
            PropertyInfo propertyInfo = GetType().GetProperty(fieldName);
            if (propertyInfo == null)
                return CompareTo(other);
            var value = (string)propertyInfo.GetValue(this);
            var target = (string)propertyInfo.GetValue(other);
            if (value == null && target == null)
                return 0;
            if (value == null && target != null)
                return 1;

            return value.CompareTo(target);
        }


        public bool update(RoomInfo info) {
            bool hasUpdaed = false;
            foreach (var propertyInfo in typeof(RoomInfo).GetProperties()) {
                //PropertyInfo propertyInfo = GetType().GetProperty(prop.Name);
                if (propertyInfo.Name.Equals("Updated"))
                    continue;
                
                var org = propertyInfo.GetValue(this) as string;
                var target = propertyInfo.GetValue(info) as string;
                if (org == null && target == null)
                    continue;
                if (org == null){
                    propertyInfo.SetValue(this, target, null);
                    hasUpdaed = true;
                    continue;
                }
                
                if (!org.Equals(target) && propertyInfo.CanWrite) {
                    propertyInfo.SetValue(this, target, null);
                    Updated = DateTime.Now;
                    hasUpdaed = true;
                }
            }

            notifyChanged();
            return hasUpdaed;
        }

        public bool update(DataRow row) {
            bool hasUpdaed = false;

            foreach (DataColumn col in row.Table.Columns) {
                var value = row[col.ColumnName];
                if (value.Equals(DBNull.Value))
                    value = null;

                switch (col.ColumnName) {
                    case "roomID":
                        if (ID == null && value == null)
                            break;
                        if (ID == null || !ID.Equals(value)) {
                            ID = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "roomName":
                        if (Name == null && value == null)
                            break;
                        if (Name == null || !Name.Equals(value)) {
                            Name = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "roomLocation":
                        if (Location == null && value == null)
                            break;
                        if (Location == null || !Location.Equals(value)) {
                            Location = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "doctorName":
                        if (DoctorName == null && value == null)
                            break;
                        if (DoctorName == null || !DoctorName.Equals(value)) {
                            DoctorName = value as string;
                            this.DoctorTitle = DefaultDoctorTitle;
                            hasUpdaed = true;
                        }
                        break;
                    case "opdTimeID":
                        if (value == null)
                            break;
                        if (!OpdTimeID.Equals(int.Parse(value as string))) {
                            OpdTimeID = int.Parse(value as string);
                            hasUpdaed = true;
                        }
                        break;
                    case "subDoctorID":
                        if (SubDoctorID == null && value == null)
                            break;
                        if (SubDoctorID == null || !SubDoctorID.Equals(value)) {
                            SubDoctorID = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "subDoctorName":
                        if (SubDoctorName == null && value == null)
                            break;
                        if (SubDoctorName == null || !SubDoctorName.Equals(value)) {
                            SubDoctorName = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "calledNo":
                        if (CalledNo == null && value == null)
                            break;
                        if (CalledNo == null || !CalledNo.Equals(value)) {
                            CalledNo = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "waitCount":
                        if (WaitingCounter == null && value == null)
                            break;
                        if (WaitingCounter == null || !WaitingCounter.Equals(value)) {
                            WaitingCounter = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    case "regCount":
                        if (RegistrationCounter == null && value == null)
                            break;
                        if (RegistrationCounter == null || !RegistrationCounter.Equals(value)) {
                            RegistrationCounter = value as string;
                            hasUpdaed = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (hasUpdaed) {
                Updated = DateTime.Now;
                notifyChanged();
            }
            return hasUpdaed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void notifyChanged() {
            OnPropertyChanged("ID");
            OnPropertyChanged("Name");
            OnPropertyChanged("Title");
            OnPropertyChanged("CalledNo");
            OnPropertyChanged("DoctorName");
            OnPropertyChanged("DoctorTitle");
            OnPropertyChanged("OpdTimeName");
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        private string getDefaultTitme(string roomID) {
            switch (roomID) {
                case "01":
                    return "一診";
                case "02":
                    return "二診";
                case "03":
                    return "三診";
                case "04":
                    return "四診";
                case "05":
                    return "五診";
                case "06":
                    return "六診";
                case "07":
                    return "七診";
                case "08":
                    return "八診";
                case "09":
                    return "九診";
                case "10":
                    return "一十診";
                case "11":
                    return "一十一診";
                case "12":
                    return "一十二診";
                case "13":
                    return "一十三診";
                case "14":
                    return "一十四診";
                case "15":
                    return "一十五診";
                default:
                    return roomID;
            }
        }
    }
}
