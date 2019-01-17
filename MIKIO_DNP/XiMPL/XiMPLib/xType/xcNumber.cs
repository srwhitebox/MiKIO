using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace XiMPLib.xType {
    public class xcNumber {
        public String UnitText {
            get;
            set;
        }

        private Decimal Decimal {
            get;
            set;
        }
        public int Int {
            get {
                return (int)Decimal;
            }
        }

        public float Float {
            get {
                return (float)Decimal;
            }
        }

        public double Double {
            get {
                return (double)Decimal;
            }
        }

        public xcNumber(String value) {
            if (string.IsNullOrWhiteSpace(value))
                return;
            value = xcString.removeWhiteSpace(value);
            if (value.EndsWith("\"")){
                UnitText = "\"";
            } else if (value.EndsWith("\'")) {
                UnitText = "\'";
            } else {
                Regex re = new Regex(@"([a-zA-Z]+)");
                Match result = re.Match(value);
                UnitText = result.Length>0 ? result.Groups[1].Value.ToLower() : string.Empty;
                if (UnitText != string.Empty)
                    value = value.Substring(0, result.Groups[1].Index);
            }

            Decimal = xcDecimal.Parse(value);
        }
    }
}
