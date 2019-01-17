using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.XiMPL {
    public class xcModelParams : Dictionary<string, string>{
        public string Key {
            get;
            set;
        }

        public string DefaultValue {
            get;
            set;
        }

        public bool IsModelParam {
            get {
                return !string.IsNullOrEmpty(Key);
            }
        }

        public xcModelParams(string paramString) {
            paramString = paramString.Trim();

            if (string.IsNullOrWhiteSpace(paramString) || !paramString.StartsWith("${") || !paramString.EndsWith("}")) {
                DefaultValue = paramString;
                return;
            }
            
            paramString = paramString.Substring(2, paramString.Length - 3).Trim();
            int index = paramString.IndexOf("?");

            if (index < 0) {
                this.Key = paramString;
                return;
            }

            this.Key = paramString.Substring(0, index);

            paramString = paramString.Substring(index + 1);

            string[] pairs = paramString.Split(';');
            foreach (string pair in pairs) {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2) {
                    string tempKey = keyValue[0].Trim();
                    if (ContainsKey(tempKey))
                        Remove(tempKey);
                    Add(tempKey, keyValue[1].Trim());
                } else {
                    DefaultValue = pair.Trim();
                }
            }
        }
    }
}
