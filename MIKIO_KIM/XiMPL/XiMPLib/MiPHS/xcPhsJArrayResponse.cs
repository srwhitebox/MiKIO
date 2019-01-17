using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace XiMPLib.MiPHS {
    public class xcPhsJArrayResponse : JArray {

        public xcPhsJArrayResponse(string response)
            : base(Parse(response)) {            
        }
    }
}
