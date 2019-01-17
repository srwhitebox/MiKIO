using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.Type
{
    class xcEnumUtil
    {
        public static Object get(System.Type enumType, String enumValue)
        {
            return Enum.Parse(enumType, enumValue, true);
        }
    }
}
