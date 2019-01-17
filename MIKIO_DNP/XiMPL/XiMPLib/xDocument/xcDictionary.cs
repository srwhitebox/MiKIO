using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.XiMPL;
using XiMPLib.xIO;

namespace XiMPLib.xDocument {
    public class xcDictionary : Dictionary<string, string>{
        public xcDictionary() {

        }

        public void addXmlSource(Uri uriPath) {
            var xmplObject = new xcXimplObject(uriPath);

            foreach (var property in xmplObject.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    foreach (JToken childProperty in property.Value) {
                        Add(childProperty);
                    }
                } else {
                    foreach (JToken token in property.Children()) {
                        Add(token);
                    }
                }
            }
        }

        private void Add(JToken token) {
            var keyToken = token["@key"];
            if (keyToken != null)
            {
                var key = token["@key"].ToString();
                var value = token["@value"].ToString();
                Add(key, value);
            }
        }
    }
}
