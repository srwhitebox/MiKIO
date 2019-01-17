using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XiMPLib.xDocument;

namespace XiMPLib.XiMPL {
    public class xcProject : xcXimplObject {
        public String Version {
            get {
                return getString("version");
            }
        }

        public String Distributor {
            get {
                return getString("distributor");
            }
        }

        public DateTime CreatedDate {
            get {
                return DateTime.Parse(getString("created_date"));
            }
        }

        public Uri StartUp {
            get {
                return new Uri(getString("startup"));
            }
        }

        public xcProject(Uri uriPath)
            : base(uriPath) {
        }
    }
}
