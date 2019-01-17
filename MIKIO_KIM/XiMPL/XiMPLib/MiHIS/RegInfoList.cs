using MitacHis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using XiMPLib.xBinding;
using XiMPLib.xUI;

namespace XiMPLib.MiHIS {
    public class RegInfoList : List<RegInfo>{
        public List<object> BoundedList = new List<object>();
        public void loads() {
            Clear();
            if (xcBinder.MiHIS != null)
                xcBinder.MiHIS.ReqQuery(regQueryCallback);
        }

        public void addBoundedList(object control) {
            BoundedList.Add(control);
        }

        private void regQueryCallback(DataTable table) {
            foreach (var row in table.AsEnumerable()) {
                var regInfo = new RegInfo(row);
               if (regInfo.Status <= 1)
                    Add(regInfo);
            }

            foreach(object control in BoundedList){
                if (control.GetType().Equals(typeof(xcDataGrid))) {
                    refreshDataGrid((xcDataGrid)control);
                }
            }
        }

        private delegate void RefreshDatagridDelegate(xcDataGrid dataGrid);

        private void refreshDataGrid(xcDataGrid dataGrid) {
            if (dataGrid.Dispatcher.CheckAccess()) {
                dataGrid.Items.Refresh();
                if (dataGrid.IsVisible) {
                    var cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
                    cv.Filter = dataGrid.Filter != null ? dataGrid.Filter : null;
                }
            } else {
                dataGrid.Dispatcher.BeginInvoke(new RefreshDatagridDelegate(refreshDataGrid), dataGrid);
            }
        }
    }
}
