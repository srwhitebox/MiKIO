using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace XiMPLib.MiPHS {
    public class xcPhsJsonResponse : JObject {
        
        public string Message {
            get {
                return this["msg"].ToString();
            }
        }

        public bool IsSucceed {
            get{
                return Message != null && Message.Equals("success");
            }
        }

        public string Token {
            get {
                return this["token"].ToString();
            }
        }

        public xcPhsJsonResponse(string response):base(Parse(response)){            
        }
    }
}
