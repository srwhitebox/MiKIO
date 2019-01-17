using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiHealth {
    public class Nurse : INotifyPropertyChanged {
        public String Name
        {
            get
            {
                JObject nurse = getNurse(Dept);
                return nurse== null ? null : nurse.GetValue("name").ToString();
            }
        }

        public String NurseUid
        {
            get
            {
                JObject nurse = getNurse(Dept);
                return nurse == null ? null : nurse.GetValue("uid").ToString();
            }
        }

        private String Dept
        {
            get; set;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private JArray NurseArray;

        public Nurse() {

        }

        public void set(JArray jArray) {
            Dept = "internal";
            NurseArray = jArray;
        }

        public void setDept(String dept) {
            Dept = dept;
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        private JObject getNurse(String dept) {
            if (NurseArray == null)
                return null;
            JObject jNurse = null;
            foreach(JObject nurse in NurseArray) {
                JObject properties = (JObject)nurse.GetValue("properties");
                JToken jDept = properties.GetValue("dept");
                if (jDept.ToString().Contains(dept))
                {
                    return nurse;
                }
                else
                    jNurse = nurse;
            }
            return jNurse;
        }
    }
}
