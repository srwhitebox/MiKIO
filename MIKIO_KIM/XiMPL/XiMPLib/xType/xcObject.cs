using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace XiMPLib.xType {
    public class xcObject {

        /// <summary>
        /// Copy all properties from a source object to destination object
        /// </summary>
        /// <param name="srcObject"></param>
        /// <param name="destObject"></param>
        public static void copyProperties(object destObject, object srcObject) {
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(srcObject)) {
                if (!item.IsReadOnly && item.SerializationVisibility != DesignerSerializationVisibility.Hidden) {
                    item.SetValue(destObject, item.GetValue(srcObject));
                }
            }
        }

    }
}
