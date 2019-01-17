using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcDecimal {
        public static Decimal Parse(String value, IFormatProvider formatProvider = null) {
            if (formatProvider == null)
                formatProvider = CultureInfo.CreateSpecificCulture("en-US");
            return Decimal.Parse(value, formatProvider);
        }
    }
}
