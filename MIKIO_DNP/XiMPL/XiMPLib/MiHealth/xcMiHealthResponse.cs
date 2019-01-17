using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace XiMPLib.MiPHS {
    public class xcMiHealthResponse : JObject {
        public JToken Data
        {
            get
            {
                return this["data"];
            }
        }

        public string Message {
            get {
                return this["msg"].ToString();
            }
        }

        public int Code
        {
            get
            {
                return (int)this["code"];
            }
        }

        public bool IsSucceed {
            get{
                return Code < 400;;
            }
        }

        public string Token {
            get {
                return ((JObject)Data)["token"].ToString();
            }
        }

        public xcMiHealthResponse(string response):base(Parse(response)){            
        }
    }
}
