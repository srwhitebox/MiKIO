using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xBinding;
using XiMPLib.xUI;

namespace XiMPLib.MiHIS {
    public class OpdScheduleList : List<OpdSchedule>{
        public List<object> BoundedList = new List<object>();
        
        private DateTime opdDate = DateTime.Today;

        public string DeptID {
            get;
            set;
        }

        public DateTime OpdDate {
            get {
                return opdDate;
            }
            set {
                opdDate = value;
                loads(DeptID, "", opdDate, opdDate, "");
            }
        }

        public DateTime StartDate {
            get;
            set;
        }
        public DateTime EndDate {
            get;
            set;
        }

        public void loads(string deptID) {
            DeptID = deptID;
            OpdDate = DateTime.Today;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
        }

        public void loads(string deptID, DateTime date) {
            OpdDate = date;
            StartDate = date;
            EndDate = date;
        }

        public void loads(string deptId, string doctorId, DateTime fromDate, DateTime toDate, string opdTimeId) {
            this.Clear();
            xcBinder.MiHIS.GetOPDSchdule(deptId, doctorId, fromDate, toDate, opdTimeId, getOpdScheduleCallback);
        }

        private void getOpdScheduleCallback(DataTable table) {
            this.Clear();
            foreach (var row in table.AsEnumerable()) {
                var opdSchedule = new OpdSchedule(row);
                if (opdSchedule.OpdDate.Equals(this.OpdDate) && opdSchedule.CanRegist && int.Parse(opdSchedule.RoomID) <= 10)
                    Add(opdSchedule);
            }

            foreach (object control in BoundedList) {
                if (control.GetType().Equals(typeof(xcDataGrid))) {
                    refreshDataGrid((xcDataGrid)control);
                }
            }
        }

        public void addBoundedList(object control) {
            BoundedList.Add(control);
        }

        private delegate void RefreshDatagridDelegate(xcDataGrid dataGrid);

        private void refreshDataGrid(xcDataGrid dataGrid) {
            if (dataGrid.Dispatcher.CheckAccess()) {
                dataGrid.Items.Refresh();
            } else {
                dataGrid.Dispatcher.BeginInvoke(new RefreshDatagridDelegate(refreshDataGrid), dataGrid);
            }
        }
    }
}
