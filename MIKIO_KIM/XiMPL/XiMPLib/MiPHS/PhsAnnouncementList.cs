using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiPHS {
    public class PhsAnnouncementList : List<PhsAnnouncementRecord>{
        public void set(xcPhsJArrayResponse array) {
            this.Clear();
            foreach (var token in array) {
                PhsAnnouncementRecord record = new PhsAnnouncementRecord((JObject)token);
                Add(record);
            }
        }
    }
}
