using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcActionInfo {
        public string Name {
            get;
            set;
        }

        public string Action {
            get;
            set;
        }

        public xcActionInfo(string name, string action) {
            Name = name;
            Action = action;
        }
    }
}
