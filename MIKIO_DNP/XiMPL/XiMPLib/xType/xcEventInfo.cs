using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcEventInfo {
        public string Name {
            get;
            set;
        }

        public string Target {
            get;
            set;
        }

        public string Action {
            get;
            set;
        }

        public string BatchAction
        {
            get;set;
        }

        public xcEventInfo(string name, string target, string action, string batchAction) {
            Name = name;
            Target = target;
            Action = action;
            BatchAction = batchAction;
        }
    }
}
