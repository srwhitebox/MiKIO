using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xBinding;
using XiMPLib.xType;

namespace XiMPLib.MiHIS {
    public class OpdSchedule : INotifyPropertyChanged {
        public string DeptID {
            get;
            set;
        }

        public string DeptName {
            get;
            set;
        }

        public DateTime OpdDate {
            get;
            set;
        }

        public string OpdDateString {
            get {
                return OpdDate.ToString("yyyy.MM.dd");
            }
        }

        public string OpdTimeID {
            get;
            set;
        }

        public string OpdTimeName {
            get {
                return xcBinder.AppProperties.OpdTimeList[OpdTimeID].Name;
            }
        }
        public string RoomID {
            get;
            set;
        }

        private string roomName;
        public string RoomName {
            get {
                string name = roomName;
                if (!string.IsNullOrEmpty(name))
                    return name;
                var roomInfo = xcBinder.AppProperties.RoomList[RoomID];
                if (roomInfo!=null)
                    name = roomInfo.Title;
                return string.IsNullOrEmpty(name) ? RoomID : name;
            }
            set {
                roomName = value;
            }
        }


        public string RoomLocation {
            get;
            set;
        }

        public string DoctorID {
            get;
            set;
        }

        public string DoctorName {
            get;
            set;
        }

        public string SubDoctorID {
            get;
            set;
        }

        public string SubDoctorName {
            get;
            set;
        }

        public bool hasSubDoctor {
            get {
                return !string.IsNullOrEmpty(SubDoctorID);
            }
        }

        public int NumberToFull {
            get;
            set;
        }

        public bool CanRegist {
            get;
            set;
        }

        public string Memo {
            get;
            set;
        }

        public bool IsWebAllowed {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OpdSchedule() {

        }

        public OpdSchedule(DataRow row) {
            string id = Convert.ChangeType(row["deptID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.DeptID = id;
            }
            string name = Convert.ChangeType(row["deptName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.DeptName = name;
            }

            string date = Convert.ChangeType(row["opdDate"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(date)) {
                this.OpdDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            }

            id = Convert.ChangeType(row["opdTimeID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.OpdTimeID = id;
            }

            id = Convert.ChangeType(row["roomID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.RoomID = id;
            }

            name = Convert.ChangeType(row["roomName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.RoomName = name;
            }

            string location = Convert.ChangeType(row["roomLocation"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(location)) {
                this.RoomLocation = location;
            }

            id = Convert.ChangeType(row["doctorID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.DoctorID = id;
            }

            name = Convert.ChangeType(row["doctorName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.DoctorName = name;
            }

            id = Convert.ChangeType(row["subDoctorID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.SubDoctorID = id;
            }

            name = Convert.ChangeType(row["subDoctorName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.SubDoctorName = name;
            }

            string n = Convert.ChangeType(row["nToFull"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(n)) {
                this.NumberToFull = int.Parse(n);
            }

            string canReg = Convert.ChangeType(row["canReg"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(canReg) && canReg.Equals("Y", StringComparison.CurrentCultureIgnoreCase)) {
                this.CanRegist = true;
            } else {
                this.CanRegist = false;
            }

            if (this.OpdDate < DateTime.Today)
                this.CanRegist = false;

            if (this.OpdDate == DateTime.Today) { // If it is today, the time range has to be after this moment.
                var opdTime = xcBinder.AppProperties.OpdTimeList[this.OpdTimeID];
                this.CanRegist = opdTime.canCheckin();
            }

            string memo = Convert.ChangeType(row["memo"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(memo)) {
                this.Memo = memo;
            }

            string isAllowed = Convert.ChangeType(row["webAllowed"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(isAllowed) && isAllowed.Equals("Y", StringComparison.CurrentCultureIgnoreCase)) {
                this.IsWebAllowed = true;
            } else {
                this.IsWebAllowed = false;
            }
        }

        public void copyFrom(OpdSchedule schedule) {
            if (schedule == null)
                this.clear();
            else
                xcObject.copyProperties(this, schedule);
            notifyChanged();
        }

        public void clear() {
            DeptID = null;
            DeptName = null;
            OpdDate = new DateTime();
            OpdTimeID = null;
            RoomID = null;
            RoomName = null;
            RoomLocation = null;
            DoctorID = null;
            DoctorName = null;
            SubDoctorID = null;
            SubDoctorName = null;
            NumberToFull = -1;
            CanRegist = false;
            Memo = null;
            IsWebAllowed = false;

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
