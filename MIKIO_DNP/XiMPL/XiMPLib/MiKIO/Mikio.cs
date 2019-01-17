using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.MiPHS;
using XiMPLib.xBinding;
using XiMPLib.XiMPL;
using XiMPLib.xNetwork;

namespace XiMPLib.MiKIO
{
    public class Mikio : INotifyPropertyChanged
    {
        public string Domain
        {
            get
            {
                return xcBinder.AppProperties.MiHealthDomain;
            }
        }

        public String CampusId
        {
            get
            {
                return xcBinder.AppProperties.HospitalID;
            }
        }

        public String AuthKey
        {
            get
            {
                return xcBinder.AppProperties.HospitalAuthKey;
            }
        }

        public String MikioId
        {
            get
            {
                return xcBinder.AppProperties.KioskID;
            }
        }

        public JArray Students
        {
            get;set;
            //get
            //{
            //    if (MikioProperties != null)
            //        return JArray.Parse(MikioProperties.GetValue("students").ToString());
            //    return null;
            //}
        }

        public Boolean HasStudents
        {
            get
            {
                return Students != null && Students.Count > 0;
            }
        }

        public JObject MikioProperties
        {
            get
            {
                if (jMikio == null)
                    return null;
                JToken jToken = this.jMikio.GetValue("properties");
                return (JObject)jToken;
            }
        }

        private JObject jMikio;

        public event PropertyChangedEventHandler PropertyChanged;

        public Mikio()
        {
            refresh();
        }

        public Mikio(JObject jMikio)
        {
            this.jMikio = jMikio;            
        }

        public async void refresh(Action getAnouncementsCallback = null, int timeout = Global.DEFAULT_REQUEST_TIMEOUT)
        {
            string url = string.Format("{0}/api/mikio/students/{1}/{2}", Domain, CampusId, MikioId);
            xcWebClient client = new xcWebClient(url, xcWebClient.METHOD_GET);

            if (Students == null)
                Students = new JArray();
            else
                Students.Clear();
            xcMiHealthResponse mihealthResponse = null;
            try
            {
                mihealthResponse = await requestMiHealthResponse(client);
                if (mihealthResponse != null && mihealthResponse.IsSucceed)
                {
                    Students.Clear();
                    JArray students = (JArray)mihealthResponse.Data;
                    for (int i=0; i< students.Count; i++){
                        if (studentAvailable((JObject)students[i])) {                          
                            Students.Add(students[i]);
                        }
                    }
                }
                notifyChanged();
            }
            catch (Exception e)
            {

            }
        }

        private Boolean studentAvailable(JObject student)
        {
            if ((Boolean)student["isAbsent"] == true)
                return false;

            for(int i = 0; i< Students.Count; i++){
                if (Students[i]["studentUid"] == student["studentUid"])
                    return false;
            }
            return true;
        }

        public void notifyChanged()
        {
            OnPropertyChanged("HasStudents");
            OnPropertyChanged("Students");
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task<xcMiHealthResponse> requestMiHealthResponse(xcWebClient client)
        {
            string response = await client.requestAsync();

            return response == null ? null : new xcMiHealthResponse(response);
        }

    }
}
