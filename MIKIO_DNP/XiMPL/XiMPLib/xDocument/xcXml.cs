using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XiMPLib.xIO;
using System.Windows;

namespace XiMPLib.xDocument
{
    class xcXml : XmlDocument
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xmlText"></param>
        public xcXml(String xmlText)
        {
            LoadXml(xmlText);
        }

        public xcXml(xcFile file)
        {
            String xmlText = file.Text;
            if (!string.IsNullOrWhiteSpace(xmlText))
            {
                try {
                    LoadXml(xmlText);
                }
                catch(Exception ex) {
                    MessageBox.Show(file.Path + "\n"+ex.Message, "XPL format error");
                    Application.Current.Shutdown();
                }
            }
            else {
                
            }
        }

        /// <summary>
        /// Conver to JSON Object
        /// </summary>
        /// <returns></returns>
        public JObject toJObject()
        {
            String jsonText = Newtonsoft.Json.JsonConvert.SerializeXmlNode(this);
            return JObject.Parse(jsonText);
        }

        public void get() {
            
        }
    }
}
