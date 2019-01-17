using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using XiMPLib.xIO;
using XiMPLib.xType;
using System.Drawing;
using System.Drawing.Imaging;

namespace XiMPLib.xDocument {
    public class xcJObject : JObject {
        public Uri PathUri {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jsonText"></param>
        /// <param name="rootProperty"></param>
        public xcJObject(string jsonText, string rootProperty=null){
            JObject jObject = Parse(jsonText);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uriPath"></param>
        /// <param name="rootProperty"></param>
        public xcJObject(Uri uriPath, string rootProperty=null) : base(load(uriPath, rootProperty)) {
            this.PathUri = uriPath;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jObject"></param>
        public xcJObject(JObject jObject, string rootProperty = null) : base(getJObject(jObject, rootProperty)){
        }

        private static JObject load(Uri uriPath, string rootProperty = null) {
            if (uriPath == null)
                return null;

            xcFile file = new xcFile(uriPath);
            JObject jObject = new xcXml(file).toJObject();
            jObject = getJObject(jObject, rootProperty);
            return jObject;
        }

        private static JObject getJObject(JObject jObject, string property) {
            JToken value = getValue(jObject, property);
            return value !=null && value.Type == JTokenType.Object ? (JObject)value : new JObject();
        }

        /// <summary>
        /// Return string value with given property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string getString(string property, string defaultValue = "", StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            JToken value = getValue(property, stringComparison);
            return value == null ? defaultValue : value.ToString();
        }

        /// <summary>
        /// Return boolean value with given key
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Boolean getBoolean(string property, Boolean defaultValue = false, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            JToken value = getValue(property, stringComparison);
            if (value != null) {
                switch (value.Type) {
                    case JTokenType.Boolean:
                        return (Boolean)value;
                    case JTokenType.Integer:
                    case JTokenType.Float:
                        return (float)value != 0f;
                    default:
                        try {
                            return xcString.toBoolean(value.ToString());
                        } catch {
                        }
                        break;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Return integer value with given key
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int getInt(string property, int defaultValue = 0, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            JToken value = getValue(property, stringComparison);
            if (value != null)
            {
                switch (value.Type) {
                    case JTokenType.Integer:
                        return (int)value;
                    case JTokenType.Float:
                        return (int)Math.Round((float)value);
                    case JTokenType.Boolean:
                        return (Boolean)value ? 1 : 0;
                    default:
                        return xcString.toInt(value.ToString());
                }
            }
            return defaultValue;
        }

        public float getFloat(string property, float defaultValue = 0, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            JToken value = getValue(property, stringComparison);
            if (value != null) {
                switch (value.Type) {
                    case JTokenType.Integer:
                    case JTokenType.Float:
                        return (float)Math.Round((float)value);
                    default:
                        return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Return URI value with given key
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public Uri getUri(string property, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            string value = getString(property, "", stringComparison);
            return string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
        }

        /// <summary>
        /// Return Font value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public Font getFont(string property, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            string value = getString(property, String.Empty, stringComparison);

            try {
                return !string.IsNullOrWhiteSpace(value) ? new xcDrawingFont(value).Font : SystemFonts.DefaultFont;
            } catch {
                return SystemFonts.DefaultFont;
            }
        }

        public Color getColor(string property, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            string value = getString(property, "Black", stringComparison);
            try {
                return (Color)new ColorConverter().ConvertFromString(value);
            } catch {
                return Color.Black;
            }
        }

        public System.Drawing.Image getImage(string property, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            string value = getString(property, "", stringComparison);
            try {
                return !string.IsNullOrWhiteSpace(value) ? System.Drawing.Image.FromFile(value) : null;
            } catch {
                return null;
            }
        }

        /// <summary>
        /// return JToken value with given key
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public JToken getValue(string property, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            return getValue(this, property, stringComparison);
        }

        public static JToken getValue(JObject jObject, string property, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase) {
            if (property == null)
                return jObject;
            JToken value = jObject.GetValue(property, stringComparison);
            if (value == null)
                value = jObject.GetValue('@' + property, stringComparison);
            return value;
        }
    }
}
