using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

namespace XiMPLib.xDocument {
    public class xcProperties : Dictionary<string, string> {
        public string PropertiesPath {
            get;
            set;
        }

        public xcProperties(string propertiesPath)
        {
            PropertiesPath = propertiesPath;
            read();
        }

        public int getInt(string key, int defaultValue = -1)
        {
            try
            {
                return int.Parse(this[key]);
            }
            catch{
                return defaultValue;
            }
        }

        public bool getBoolean(string key, bool defaultValue = false)
        {
            try
            {
                return bool.Parse(this[key]);
            }catch{
                return defaultValue;
            }
        }

        public string[] getStringArray(string key, bool allowEmptyValue = false) {
            try {
                string value = this[key];
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                JArray jProperty = JArray.Parse(this[key]);
                List<string> itemsArray = new List<string>();
                string[] items = new string[jProperty.Count];
                int i = 0;
                foreach (JToken token in jProperty) {
                    value = token.ToString();
                    if (allowEmptyValue)
                        itemsArray.Add(value);
                        //items[i++] = value;
                    else if (!string.IsNullOrWhiteSpace(value)){
                        itemsArray.Add(value);
                        //items[i++] = value;
                    }
                }
                return itemsArray.Count == 0 ? null : itemsArray.ToArray();
            } catch {
                return null;
            }
        }

        public void setProperty(string key, string[] items) {
            if (items == null || items.Length == 0)
                return;
            JArray value = new JArray();
            foreach (string item in items)
                value.Add(item);
            this[key] =value.ToString(Newtonsoft.Json.Formatting.None);
        }

        public void setProperty(string key, bool value)
        {
            this[key] = value.ToString();
        }

        public void read()
        {
            try {
                string fileData = "";
                using (StreamReader sr = new StreamReader(this.PropertiesPath))
                {
                    fileData = sr.ReadToEnd().Replace("\r", "");
                }
                string[] kvp;
                string[] records = fileData.Split("\n".ToCharArray());
                foreach (string record in records)
                {
                    kvp = record.Split("=".ToCharArray());
                    if (kvp == null || kvp.Length < 2)
                        continue;
                    this.Add(kvp[0], kvp[1]);
                }
            }
            catch (DirectoryNotFoundException e)
            { 
            }
            catch(FileNotFoundException e)
            {
            }
        }

        public void save()
        {
            saveAs(PropertiesPath);
        }

        public void saveAs(string path) {
            if (!Directory.GetParent(path).Exists) {
                Directory.GetParent(path).Create();
            }
            using (StreamWriter sw = new StreamWriter(File.Create(path), Encoding.UTF8)) {
                foreach (string key in Keys) {
                    sw.Write(key + "=" + this[key] + "\r\n");
                }
            }
        }

    }
}
