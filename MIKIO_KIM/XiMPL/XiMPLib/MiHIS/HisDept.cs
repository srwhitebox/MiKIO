using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiHIS {
    public class HisDept {
        public string ID {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public string GroupID {
            get;
            set;
        }

        public string GroupName {
            get;
            set;
        }

        public bool IsWebAllowed {
            get;
            set;
        }

        public HisDept(string id, string name) {
            this.ID = id;
            this.Name = name;
        }

        public HisDept(DataRow row) {
            string id = Convert.ChangeType(row["deptID"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(id)) {
                this.ID = id;
            }
            
            string name = Convert.ChangeType(row["deptName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.Name  = name;
            }
             id = Convert.ChangeType(row["deptGroupID"], typeof(string)) as string;
             if (!string.IsNullOrEmpty(id)) {
                this.GroupID = id;
            }
            name = Convert.ChangeType(row["deptGroupName"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(name)) {
                this.GroupName = name;
            }

            string isAllowed = Convert.ChangeType(row["webAllowed"], typeof(string)) as string;
            if (!string.IsNullOrEmpty(isAllowed) && isAllowed.Equals("Y", StringComparison.CurrentCultureIgnoreCase)) {
                this.IsWebAllowed = true;
            } else {
                this.IsWebAllowed = false;
            }
        }
    }
}
