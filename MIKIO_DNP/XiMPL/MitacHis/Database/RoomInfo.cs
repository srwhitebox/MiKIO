using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MitacHis.Database {
    public class RoomInfo : IEquatable<RoomInfo>, IComparable<RoomInfo> {
        public string roomID {
            get;
            set;
        }

        public string roomName {
            get;
            set;
        }

        public string roomTitle {
            get {
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

        public string roomLocation {
            get;
            set;
        }

        public string doctorName {
            get;
            set;
        }

        public string doctorTitle {
            get;
            set;
        }

        public string opdTimeID {
            get;
            set;
        }

        public string optTimeName {
            get {
                switch (opdTimeID) {
                    case "1":
                        return "上牛診";
                    case "2":
                        return "下友診";
                    case "5":
                        return "夜間診";
                    default:
                        return "";
                }
            }
        }

        public string subDoctorID {
            get;
            set;
        }

        public string subDoctorName {
            get;
            set;
        }

        public string calledNo {
            get;
            set;
        }

        public string waitingCount {
            get;
            set;
        }

        public string regCount {
            get;
            set;
        }

        public DateTime updated {
            get;
            set;
        }

        public RoomInfo() {
            updated = DateTime.Now;
        }

        public RoomInfo(string roomId) {
            this.roomID = roomId;
            updated = DateTime.Now;
        }

        public bool Equals(RoomInfo obj){
            foreach (var propertyInfo in typeof(RoomInfo).GetProperties()) {
                var org = propertyInfo.GetValue(this) as string;
                var target = propertyInfo.GetValue(obj) as string;
                if (!propertyInfo.Name.Equals("updated"))
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
            return updated.CompareTo(other.updated);
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


        public bool updateFrom(RoomInfo info) {
            bool hasUpdaed = false;
            foreach (var propertyInfo in typeof(RoomInfo).GetProperties()) {
                //PropertyInfo propertyInfo = GetType().GetProperty(prop.Name);
                if (propertyInfo.Name.Equals("updated"))
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
                if (!org.Equals(target)) {
                    propertyInfo.SetValue(this, target, null);
                    updated = DateTime.Now;
                    hasUpdaed = true;
                }
            }
            return hasUpdaed;
        }

    }
}
