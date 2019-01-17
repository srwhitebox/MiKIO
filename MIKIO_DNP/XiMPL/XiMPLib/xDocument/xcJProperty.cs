using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using XiMPLib.xUI;
using XiMPLib.xUI.Effects;
using System.Windows.Media.Effects;
using XiMPLib.xType;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace XiMPLib.xDocument {
    public class xcJProperty : JProperty {
        public string Key {
            get {
                string key = this.Name.StartsWith("@") ? this.Name.Substring(1) : this.Name;
                return key.ToLower();
            }
        }

        public string String {
            get {
                string value = (string)this.Value;
                return value == null ? value : value.Trim().Replace("\u0013\u0010", "\n");
            }
        }

        public string[] StringArray
        {
            get
            {
                string value = (string)this.Value;
                return value.Split('|');
            }
        }

        public bool isBindProperty {
            get {
                string text = String;
                return text.StartsWith("${") && text.EndsWith("}");
            }
        }
        public string BindProperty {
            get {
                return String.Replace("${", "").Replace("}", "");
            }
        }

        public string LowerString {
            get {
                return this.String == null ? this.String : this.String.ToLower();
            }
        }

        public string UpperString {
            get {
                return this.String == null ? this.String : this.String.ToUpper();
            }
        }

        public int Integer {
            get {
                return getInt();
            }
        }

        public float Float {
            get {
                return getFloat();
            }
        }

        public float Percent {
            get {
                return getPercent();
            }
        }

        public bool Boolean {
            get {
                return getBoolean();
            }
        }

        public XiMPLib.xType.xcSize Size{
            get {
                return new XiMPLib.xType.xcSize(String);
            }
        }

        public Point Point {
            get {
                return xcUiProperty.toPoint(String);
            }
        }

        public HorizontalAlignment HorizontalAlignment {
            get {
                return xcUiProperty.toHorizontalAlignment(String);
            }
        }

        public VerticalAlignment VerticalAlignment {
            get {
                return xcUiProperty.toVerticalAlignment(String);
            }
        }

        public Thickness Thickness {
            get {
                return new XiMPLib.xType.xcThickness(String).Thickness;
            }
        }

        public CornerRadius CornerRadius {
            get {
                return new XiMPLib.xType.xcCornerRadius(String).CornerRadius;
            }
        }

        public Cursor Cursor {
            get {
                return xcUiProperty.toCursor(String);
            }
        }

        public Visibility Visibility {
            get{
                return xcUiProperty.toVisibility(String);
            }
        }

        public FlowDirection FlowDirection {
            get {
                return xcUiProperty.toFlowDirection(String);
            }
        }

        public Orientation Orientation {
            get {
                return xcUiProperty.toOrientation(String);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri(ParentUri, String));
            }
        }

        public Brush Brush {
            get {
                return xcUiProperty.toBrush(String, ParentUri);
            }
        }

        public xcBrushes Brushes {
            get {
                return xcUiProperty.toBrushes(String, ParentUri);
            }
        }

        public ResizeMode ResizeMode {
            get {
                return xcUiProperty.toResizeMode(String);
            }
        }

        public WindowStyle WindowStyle {
            get {
                return xcUiProperty.toWindowStyle(String);
            }
        }

        public WindowState WindowState {
            get {
                return xcUiProperty.toWindowState(String, System.Windows.WindowState.Normal);
            }
        }

        public WindowStartupLocation WindowStartupLocation {
            get {
                return xcUiProperty.toStartupLocation(String, WindowStartupLocation.Manual);
            }
        }

        public xcFontProperty FontProperty {
            get {
                return new xcFontProperty(String);
            }
        }

        public FontFamily FontFamily {
            get {
                return new System.Windows.Media.FontFamily(String);
            }
        }

        public FontWeight FontWeight {
            get {
                return xcFontProperty.getFontWeight(String);
            }
        }

        public FontStyle FontStyle {
            get {
                return xcFontProperty.getFontStyle(String);
            }
        }

        public FontStretch FontStretch {
            get {
                return xcFontProperty.getFontStretch(String);
            }
        }

        public CharacterCasing CharacterCasing
        {
            get {
                return (CharacterCasing)Enum.Parse(typeof(CharacterCasing), String, true);
            }
        }

        public TextAlignment TextAlignment {
            get {
                return xcUiProperty.toTextAlignment(String);
            }
        }

        public Transform Transform {
            get {
                return xcUiProperty.toTransform(String);
            }
        }

        public Stretch MediaStretch{
            get {
                try {
                    return (Stretch)Enum.Parse(typeof(Stretch), String, true);
                } catch {
                    return Stretch.UniformToFill;
                }
            }
        }

        public Color Color {
            get {
                return xcUiProperty.toColor(String);
            }
        }

        public Effect Effect {
            get {
                return new xcEffect(xcJObject).Effect;
            }
        }

        public xcJObject xcJObject {
            get {
                return new xcJObject((JObject)Value);
            }
        }

        public xcTextColumn TextColumn {
            get {
                return new xcTextColumn(xcJObject, this.ParentUri);
            }
        }

        public xcButtonColumn ButtonColumn {
            get {
                return new xcButtonColumn(xcJObject, this.ParentUri);
            }
        }

        public DataGridSelectionMode GridSelectionMode {
            get {
                return String.ToLower().Equals("single") ? DataGridSelectionMode.Single : DataGridSelectionMode.Extended;
            }
        }

        public SelectionMode ControlSelectionMode {
            get {
                return String.ToLower().Equals("single") ? SelectionMode.Single : SelectionMode.Extended;
            }
        }

        public xcLayout Layout {
            get {
                return new xcLayout(xcJObject, this.ParentUri);
            }
        }

        public xcControl Control {
            get {
                return new xcControl(xcJObject, this.ParentUri);
            }
        }

        public TextWrapping TextWrapping {
            get {
               try {
                    return (TextWrapping)Enum.Parse(typeof(System.Windows.TextWrapping), String, true);
                } catch {
                    return TextWrapping.NoWrap;
                }
            }
        }

        public PenLineCap PenLineCap {
            get {
                try {
                    return (PenLineCap)Enum.Parse(PenLineCap.GetType(), String, true);
                } catch {
                    return PenLineCap.Flat;
                }
            }
        }

        public DoubleCollection DoubleCollection {
            get {
                string value = String.Replace("(", " ").Replace(")", " ").Replace("-", " ");
                return (DoubleCollection) new DoubleCollectionConverter().ConvertFromString(value);
            }
        }

        public PointCollection PointCollection {
            get {
                return (PointCollection)new PointCollectionConverter().ConvertFromString(String);
            }
        }

        public Uri Uri {
            get {
                return getUri(String);
            }
        }

        public Uri ParentUri {
            get;
            set;
        }

        public TimeSpan Time{
            get {
                if (string.IsNullOrEmpty(String))
                    return new TimeSpan(0, 0, 0, 0, 0);
                if (String.IndexOf(":") >= 0)
                    return TimeSpan.Parse(String);
                else
                    return new TimeSpan(0, 0, 0, 0, int.Parse(String));
            }
        }

        public DateTime Date {
            get {
                return getDate(this.String);
            }
        }

        public DayOfWeek DayOfWeek{
            get {
                try {
                    return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), this.String, true);
                } catch {
                    return DayOfWeek.Sunday;
                }
            }
        }

        public DatePickerFormat DateFormat {
            get {
                try {
                    return (DatePickerFormat)Enum.Parse(typeof(DatePickerFormat), this.String, true);
                } catch {
                    return DatePickerFormat.Short;
                }
            }
        }

        public xcJProperty(JProperty property, Uri parentUri=null)
            : base(property) {
                this.ParentUri = parentUri;
        }

        public int getInt(int defaultValue=0) {
            return XiMPLib.xType.xcString.toInt(this.String, defaultValue);
        }

        public float getFloat(float defaultValue = 0) {
            return XiMPLib.xType.xcString.toFloat(this.String, defaultValue);
        }

        public float getPercent(float defaultValue = 0) {
            return XiMPLib.xType.xcString.toPercent(this.String, defaultValue);
        }

        public bool getBoolean(bool defaultValue = false) {
            return XiMPLib.xType.xcString.toBoolean(this.String, defaultValue);
        }

        public static Uri getUri(string uri, UriKind defaultUriKind = UriKind.RelativeOrAbsolute) {
            return new Uri(uri, defaultUriKind);
        }

        public static DateTime getDate(string dateString) {
            if (string.IsNullOrEmpty(dateString)) {
                return DateTime.Today;
            }
            if (dateString.Equals("today", StringComparison.CurrentCultureIgnoreCase)) {
                return DateTime.Today;
            } else {
                return DateTime.Parse(dateString);
            }
        }

        public ScrollBarVisibility ScrollBarVisiblity {
            get {
                try {
                    return (ScrollBarVisibility)Enum.Parse(typeof(ScrollBarVisibility), String, true);
                } catch {
                    return ScrollBarVisibility.Auto;
                }
            }
        }

        public ListSortDirection SortDirection
        {
            get
            {
                switch (this.String) {
                    case "ascending":
                    case "asc":
                    case "a":
                        return ListSortDirection.Ascending;
                    default:
                        return ListSortDirection.Descending;
                }
            }
        }
    }
}
