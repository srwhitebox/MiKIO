using MitacHis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.MiKIO;
using XiMPLib.xBinding;
using XiMPLib.xType;

namespace XiMPLib.MiHIS {
    public delegate void OnRegInfoChanged(object sender, RegInfo.REG_INFO_STATE state);

    public class RegInfo : INotifyPropertyChanged {
        public enum REG_INFO_STATE {
            REGISTERED,
            CANCELED,
            CHECKED_IN,
        }

        public xcHis MiHis {
            get {
                return xcBinder.MiHIS;
            }
        }


        public Patient Patient {
            get {
                return xcBinder.Patient;
            }
        }

        public string CaseNo {
            get;
            set;
        }

        public string PatientName {
            get;
            set;
        }

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
                if (xcBinder.AppProperties.OpdTimeList[OpdTimeID] == null)
                    return null;
                return xcBinder.AppProperties.OpdTimeList[OpdTimeID].Name;
            }
        }

        public string Period {
            get;
            set;
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
                name = xcBinder.AppProperties.RoomList[RoomID].Title;
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

        public int RegNumber {
            get;
            set;
        }

        public int CalledNumber {
            get;
            set;
        }

        public string Memo {
            get;
            set;
        }

        public int Status {
            get;
            set;
        }

        public DateTime RegDate {
            get {
                return DateTime.Today;
            }
        }

        public OnRegInfoChanged OnRegInfoChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public RegInfo() {

        }

        public RegInfo(DataRow row) {
            string caseNo = Convert.ChangeType(row["CaseNo"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(caseNo)) {
                this.CaseNo = caseNo;
            }

            string name = Convert.ChangeType(row["PatientName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.PatientName = name;
            }

            string id = Convert.ChangeType(row["deptID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.DeptID = id;
            }

            name = Convert.ChangeType(row["deptName"], typeof(string)) as string;
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


            string period = Convert.ChangeType(row["period"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(period)) {
                this.Period = period;
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

            string n = Convert.ChangeType(row["regNumber"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(n)) {
                this.RegNumber = int.Parse(n);
            }

            n = Convert.ChangeType(row["calledNumber"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(n)) {
                this.CalledNumber = int.Parse(n);
            }

            string memo = Convert.ChangeType(row["memo"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(memo)) {
                this.Memo = memo;
            }

            n = Convert.ChangeType(row["status"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(n)) {
                this.Status = int.Parse(n);
            } else
                this.Status = -1;
        }

        public void checkIn() { 
            MiHis.ArrivedCheckIn(OpdDate, OpdTimeID, RoomID, arrivalCheckinCallback);
        }

        private void arrivalCheckinCallback(string caseNo) {
            xcBinder.Progress.setMessage(xcBinder.getStringRes("checkin_succeed", "掛號已完成,請量血壓後至診間報到.\n請您收取健保卡與表單,謝謝您."));
            xcBinder.RegInfoList.loads();            
        }

        public void cancel() {
            MiHis.DoReqCancel(OpdDate, OpdTimeID, DeptID, DoctorID, doRegCancelCallback);
            if (this.OnRegInfoChanged != null) {
                this.OnRegInfoChanged(this, REG_INFO_STATE.CANCELED);
            }
        }

        private void doRegCancelCallback(bool canceled) {
            xcBinder.RegInfoList.loads();
            xcBinder.Progress.setMessage(xcBinder.getStringRes("register_canceled", "取消已完成"));
            if (this.OnRegInfoChanged != null) {
                OnRegInfoChanged(this, REG_INFO_STATE.CANCELED);
            }
        }

        public void clear() {
            copyFrom(new RegInfo());
        }

        public void copyFrom(RegInfo regInfo) {
            if (regInfo == null) {
                this.clear();
            } else
                xcObject.copyProperties(this, regInfo);
            notifyChanged();
        }

        public void copyFrom(OpdSchedule schedule, int regNo = 0) {
            if (schedule == null)
                return;
            this.DeptID = schedule.DeptID;
            this.DeptName = schedule.DeptName;
            this.DoctorID = schedule.DoctorID;
            this.DoctorName = schedule.DoctorName;
            this.OpdDate = schedule.OpdDate;
            this.OpdTimeID = schedule.OpdTimeID;
            this.RoomID = schedule.RoomID;
            this.SubDoctorID = schedule.SubDoctorID;
            this.SubDoctorName = schedule.SubDoctorName;
            this.RegNumber = regNo;
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
        //<CaseNo>AC150525301001</CaseNo>
        //<PatientName>金德漢</PatientName>
        //<deptID>02</deptID>
        //<deptName>一般內科</deptName>
        //<opdDate>20150525</opdDate>
        //<opdTimeID>3</opdTimeID>
        //<period>1330-1720</period>
        //<roomID>01</roomID>
        //<roomName/>
        //<roomLocation/>
        //<doctorID>doc08</doctorID>
        //<doctorName>趙醫生</doctorName>
        //<subDoctorID/>
        //<subDoctorName/>
        //<regNumber>001</regNumber>
        //<calledNumber>0</calledNumber>
        //<memo/>
        //<status>1</status>
    }

}
