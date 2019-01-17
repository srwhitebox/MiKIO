using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.XiMPL {
    public class xcModelDictionary : Dictionary<string, object>{
        public object getValue(string modelKey) {
            xcModelParams modelParams = new xcModelParams(modelKey);
            if (!modelParams.IsModelParam) {
                return modelParams.DefaultValue;
            }

            var modelValue = this[modelParams.Key];
            if (modelValue == null)
                return modelParams.DefaultValue;
            string paramKey = modelValue.ToString();
            if (modelParams.ContainsKey(paramKey)) {
                var paramValue = modelParams[modelValue.ToString()];
                if (paramValue == null)
                    paramValue = modelParams.DefaultValue;

                return paramValue == null ? modelValue : paramValue;
            } else
                return string.IsNullOrEmpty(modelParams.DefaultValue) ? modelValue : modelParams.DefaultValue;
        }
    }
}
