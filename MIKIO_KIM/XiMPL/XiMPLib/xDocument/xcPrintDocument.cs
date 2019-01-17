using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using XiMPLib.XiMPL;
using XiMPLib.xType;
using System.Runtime.InteropServices;
using XiMPLib.xDevice.xPrinter;

namespace XiMPLib.xDocument {
    /// <summary>
    /// Print Document class
    /// </summary>
    public class xcPrintDocument : PrintDocument {
        /// <summary>
        /// Get/set Model Attributes dictionary
        /// </summary>
        private xcModelDictionary ModelDictionary{
            get;
            set;
        }

        /// <summary>
        /// Get/set Print Document as Ximple Object
        /// </summary>
        private xcXimplObject PrintDocument {
            get;
            set;
        }

        private xcJObject Page {
            get {
                return new xcJObject((JObject)PrintDocument.getValue("page"));
            }
        }

        private Uri ParentUri {
            get;
            set;
        }

        private string PrinterName {
            get {
                string name = PrintDocument.getString("printer_name");
                if (string.IsNullOrEmpty(name))
                    return xcPrinter.DefaultPrinter;
                else
                    return name;
            }
        }

        private int Copies {
            get {
                string copies = PrintDocument.getString("copies");
                int copy = string.IsNullOrEmpty(copies) ? 1 : int.Parse(copies);
                return copy < 1 ? 1 : copy;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jPrintDocument"></param>
        public xcPrintDocument(xcXimplObject jPrintDocument) {
            PrintDocument = jPrintDocument;
            configure();
        }

        public xcPrintDocument(Uri uriPath) {
            PrintDocument = new xcXimplObject(uriPath);
            ParentUri = new Uri(uriPath, ".");
            configure();
        }
        /// <summary>
        /// Add Model Attributes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void addModelAttribute(String key, object value) {
            if (String.IsNullOrWhiteSpace(key) || value == null)
                return;
            
            if (ModelDictionary == null)
                ModelDictionary = new xcModelDictionary();
            ModelDictionary.Add(key, value);
        }

        System.Globalization.CultureInfo CultureInfo{
            get {
                string langCode = XiMPLib.xBinding.xcBinder.AppProperties.Language;
                return System.Globalization.CultureInfo.CreateSpecificCulture(string.IsNullOrEmpty(langCode)?"zh-TW" : langCode);
            }
        }
        
        /// <summary>
        /// Print document
        /// </summary>
        public void print() {
            PrintPage += new PrintPageEventHandler(printPageEventHandler);
            for (int i = 0; i < Copies; i++) {
                Print();
            }
        }

        /// <summary>
        /// Print page event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        private void printPageEventHandler(object sender, PrintPageEventArgs ev) {
            Graphics gr = ev.Graphics;
            foreach(var pair in Page){
                JToken jToken = pair.Value;
                switch (jToken.Type) {
                    case JTokenType.Array:
                        foreach(JToken token in jToken){
                            drwaObject(gr, pair.Key, new xcJObject((JObject)token));
                        }
                        break;
                    case JTokenType.Object:
                        drwaObject(gr, pair.Key, new xcJObject((JObject)pair.Value));
                        break;
                }
            }
        }

        /// <summary>
        /// Draw objects on the graphics
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="key"></param>
        /// <param name="jProperties"></param>
        private void drwaObject(Graphics gr, String key, xcJObject jProperties) {
            switch (xcString.removeWhiteSpace(key).ToLower()) {
                case "text":
                    var value = ModelDictionary.getValue(jProperties.getString("value"));
                    if (value == null)
                        break;
                    if (value is string || value is String)
                    {
                        if (String.IsNullOrEmpty((string)value))
                            value = " ";
                    }
                    string textFormat = jProperties.getString("format");

                    bool convertToShortTaiwaneseWeekday = false;
                    if (value.GetType().Equals(typeof(DateTime)) && !string.IsNullOrEmpty(textFormat) && textFormat.IndexOf("www")>=0 && CultureInfo.Name.Equals("zh-TW")) {
                        textFormat = textFormat.Replace("www", "ddd");
                        convertToShortTaiwaneseWeekday = true;
                    }

                    string textValue;
                    if (value.GetType().Equals(typeof(DateTime))) {
                        DateTime date = (DateTime)value;
                        if (textFormat.IndexOf("YYY") >= 0) {
                            textFormat = textFormat.Replace("YYY", "yyy");
                            date = date.AddYears(-1911);
                        }
                        textValue = date.ToString(textFormat, CultureInfo);
                        if (convertToShortTaiwaneseWeekday) {
                            textValue = textValue.Replace("週", "");
                        }
                    } else if (value.GetType().Equals(typeof(FloatRange))) {
                        var range = (FloatRange) value;
                        textValue = string.IsNullOrEmpty(textFormat) ? value.ToString() : string.Format(textFormat, range.Start, range.End, CultureInfo);
                    } else if (value.GetType().Equals(typeof(IntRange))) {
                        var range = (IntRange)value;
                        textValue = string.IsNullOrEmpty(textFormat) ? value.ToString() : string.Format(textFormat, range.Start, range.End, CultureInfo);
                    } else if (value.GetType().Equals(typeof(int)) || value.GetType().Equals(typeof(float)) || value.GetType().Equals(typeof(short)) || value.GetType().Equals(typeof(double)) || value.GetType().Equals(typeof(string))) {
                        textValue = string.IsNullOrEmpty(textFormat) ? value.ToString() : string.Format(textFormat, value);
                    } else {
                        textValue = string.IsNullOrEmpty(textFormat) ? value.ToString() : string.Format("{0:" + textFormat + "}", value, CultureInfo);
                    }

                    string textMask = jProperties.getString("mask");
                    if (!string.IsNullOrEmpty(textMask)) {
                        textValue = xcString.toMaskedString(textValue, textMask);
                    }

                    Font font = jProperties.getFont("font");
                    RectangleF rectf = new xcRectangle(jProperties.getString("area")).PageRectangleF;
                    
                    string brushValue = jProperties.getString("brush");

                    Brush brush = string.IsNullOrEmpty(brushValue) ? new SolidBrush(Color.Black) : toBrush(brushValue);
                    StringFormat format = new StringFormat();
                    string alignment = jProperties.getString("alignment");
                    if (!string.IsNullOrWhiteSpace(alignment)) {
                        format.Alignment = xcAlignment.toAlignment(alignment);
                    }
                    alignment = jProperties.getString("linealignment");
                    if (!string.IsNullOrWhiteSpace(alignment)) {
                        format.LineAlignment = xcAlignment.toAlignment(alignment);
                    }
                    string trimming = jProperties.getString("trimming");
                    if (!string.IsNullOrWhiteSpace(alignment)) {
                        format.Trimming = xcTrimming.toTrimming(alignment);
                    }

                    gr.DrawString(textValue, font, brush, rectf, format);
                    break;
                case "image":
                    String path = jProperties.getString("path");
                    Uri absUri = ParentUri == null ? new Uri(path) : new System.Uri(ParentUri, path);

                    Image image = new xcImage(absUri.AbsolutePath).Image;
                    float ratio = (float)image.Width / (float)image.Height;
                    
                    Rectangle rect = new xcRectangle(jProperties.getString("area")).PageRectangle;
                    //rect.Height = (int)(rect.Width / ratio);
                    gr.DrawImage(image, rect);
                    break;
                case "line":
                    Pen pen = xcPen.toPagePen(jProperties.getString("pen"));
                    String point = jProperties.getString("start");
                    Point ptStart = xcDrawingPoint.toPagePoint(point);
                    point = jProperties.getString("end");
                    Point ptEnd = xcDrawingPoint.toPagePoint(point);
                    gr.DrawLine(pen, ptStart, ptEnd);
                    break;
                case "rectangle":
                    pen = xcPen.toPagePen(jProperties.getString("pen"));
                    rect = new xcRectangle(jProperties.getString("area")).PageRectangle;
                    gr.DrawRectangle(pen, rect);
                    break;
            }
        }


        private Brush toBrush(string brush) {
            string[] tokens = brush.Split(':');
            if (tokens.Length < 2) {
                return new SolidBrush(Color.Black);
            } else {
                switch (tokens[0].Trim().ToLower()) {
                    case "color":
                        try {
                            string colorValue = tokens[1].Trim();
                            Color color;
                            if (colorValue[0] == '#')
                                color = Color.FromArgb(int.Parse(colorValue.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier));
                            else
                                color = Color.FromName(colorValue);
                            return new SolidBrush(color);
                        } catch {
                            return new SolidBrush(Color.Black);
                        }
                    default:
                        return new SolidBrush(Color.Black);
                }
            }
        }

        /// <summary>
        /// Configure the printer settings, paper settings
        /// </summary>
        private void configure() {
            PrinterSettings.PrinterName = this.PrinterName;
            if (string.IsNullOrEmpty(PrinterSettings.PrinterName))
                return;

            // This controller will not show the print progress
            this.PrintController = new System.Drawing.Printing.StandardPrintController();
            
            DefaultPageSettings.Landscape = PrintDocument.getBoolean("landscape", false);
            string paperSize = PrintDocument.getString("paper_size");
            if (!string.IsNullOrWhiteSpace(paperSize))
                DefaultPageSettings.PaperSize = new xcPaperSize(paperSize);
            string paperMargins = PrintDocument.getString("paper_margin");
            if (!string.IsNullOrWhiteSpace(paperMargins))
                DefaultPageSettings.Margins = new xcPaperMargins(paperMargins);
        }

    }
}
