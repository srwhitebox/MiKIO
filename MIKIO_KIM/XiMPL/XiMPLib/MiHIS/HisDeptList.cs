using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xBinding;

namespace XiMPLib.MiHIS {
    public class HisDeptList : List<HisDept>{
        public HisDeptList() {

        }

        public void loads() {
            xcBinder.MiHIS.GetDept(getDeptCallback);
        }

        private void getDeptCallback(DataTable table) {
            foreach (var row in table.AsEnumerable()) {
                Add(new HisDept(row));
            }
        }
    }
}
