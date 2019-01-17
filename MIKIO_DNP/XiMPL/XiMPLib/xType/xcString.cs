using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcString{

        /// <summary>
        /// Convert to integer
        /// If boolean value is true, it'll return 1 else 0.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int toInt(String source, int defaultValue = 0){
            try {
                return (int)Convert.ToDecimal(source);
            } catch {
                return 0;
            }
        }

        /// <summary>
        /// Remove all white spaces
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string removeWhiteSpace(String source) {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            return source.ToCharArray()
                     .Where(c => !Char.IsWhiteSpace(c))
                     .Select(c => c.ToString())
                     .Aggregate((a, b) => a + b);
        }

        /// <summary>
        /// Convert to boolean value
        /// If this is number and it is not zero(0), it'll return true;
        /// If this is "true", "yes", "y", "ok", "up", "on", "start", it'll return true.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean toBoolean(string source, bool defaultValue = false) {
            if (string.IsNullOrWhiteSpace(source))
                return defaultValue;

            switch (source.ToLower()) {
                case "true":
                case "yes":
                case "y":
                case "ok":
                case "up":
                case "on":
                case "start":
                    return true;
                default:
                    try {
                        return int.Parse(source) != 0;
                    } catch {
                        return false;
                    }
            }
        }

        public static float toFloat(string source, float defaultValue = 0) {
            if (string.IsNullOrWhiteSpace(source))
                return defaultValue;
            try {
                return (float)xcDecimal.Parse(source);
            } catch {
                return defaultValue;
            }
        }

        public static string toString(byte[] bytesArray, int index, int count, Encoding encoding) {
            return encoding.GetString(bytesArray, index, count);
        }

        public static float toPercent(string source, float defaultValue = 0) {
            if (string.IsNullOrWhiteSpace(source))
                return defaultValue;
            try {
                source = source.Trim();
                if (source.EndsWith("%")) {
                    return (float)xcDecimal.Parse(source.Substring(0, source.Length-1)) / 100;
                } else {
                    return (float)xcDecimal.Parse(source);
                }
            } catch {
                return defaultValue;
            }
        }

        public static string toMaskedString(string source, string mask) {
            if (string.IsNullOrEmpty(mask))
                return source;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i< source.Length && i < mask.Length; i++) {
                sb.Append(mask[i] == '0' ? source[i] : mask[i]);
            }
            if (source.Length > mask.Length) {
                sb.Append(source.Substring(mask.Length));
            }
            return sb.ToString();
        }
    }
}
