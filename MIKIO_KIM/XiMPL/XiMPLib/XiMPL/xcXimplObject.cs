using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xDocument;

namespace XiMPLib.XiMPL {
    public class xcXimplObject : xcJObject {
        public const String MASTER_TAG = "XMPL";

        public String Name {
            get {
                return getString("name");
            }
        }

        public xcJObject Model {
            get;
            set;
        }

        public xcXimplObject(Uri uriPath) : base(uriPath, MASTER_TAG){
        
        }

        public xcXimplObject(Uri uriPath, xcJObject jModel)
            : base(uriPath, MASTER_TAG) {
                Model = jModel;
        }

        public String getString(String key) {
            String value = base.getString(key).Trim();
            if (isModelAttr(value)) {
                return getModelAttr(value);
            } else
                return value;
        }

        protected String getModelAttr(String value) {
            if (isModelAttr(value)) {
                return Model.getString(value);
            } else
                return value;
        }

        protected Boolean isModelAttr(String value) {
            return !String.IsNullOrWhiteSpace(value) && value.StartsWith("${") && value.EndsWith("}");
        }
    }
}
