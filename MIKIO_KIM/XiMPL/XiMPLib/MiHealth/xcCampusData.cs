using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xNetwork;

namespace XiMPLib.MiCampus
{
    public class xcCampusData : JObject{

        public JObject GroupInfo
        {
            get
            {
                const string GROUP = "group";
                JToken groupInfo = this[GROUP];
                if (groupInfo == null)
                {
                    this[GROUP] = new JObject();
                }
                return (JObject)this[GROUP];
            }
        }
        public JObject ReceiverInfo
        {
            get
            {
                const string TO = "to";
                JToken groupInfo = this[TO];
                if (groupInfo == null)
                {
                    this[TO] = new JObject();
                }
                return (JObject)this[TO];
            }
        }

        public xcCampusData()
        {
            init();
        }

        public void setFatRecord(float height, float weight, float bmi)
        {
            string message = string.Format("身高 {0}, 體重 {1}, BMI {2}", height, weight, bmi);
            this["message"] = message;
        }

        public void setBloodPressureRecord(int systolic, int diastolic, int pulse)
        {
            string message = string.Format("收縮壓 {0}, 舒張壓 {1}, 脈搏 {2}", systolic, diastolic, pulse);
            this["message"] = message;
        }

        public string send()
        {
            const string url = "https://1campus.net/notification/api/post/token/e7f487427d2c7af0559a1c5100027f14";
            using (var webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Set("Content-Type", "application/json");
                string result = webClient.UploadString(url, this.ToString());
                return result;
            }
        }

        private void init()
        {
            this["sender"] = "MiCampus神通智慧校園";
            this["type"] = "normal";

            GroupInfo["dsns"] = "p.micampus.org.tw";
            GroupInfo["dsnsname"] = "神通小學";
            GroupInfo["id"] = "";
            GroupInfo["name"] = "";

            ReceiverInfo["uuid"] = "3ea4c1f2-b301-45e7-aa91-8459b00d9ea7";
            ReceiverInfo["name"] = "王小神";
        }
    }
}
