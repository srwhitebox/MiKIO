using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XiMPLib.MiPHS;
using XiMPLib.xBinding;
using XiMPLib.xChart;
using XiMPLib.xDocument;
using XiMPLib.xUI;

namespace WXiMPLib.xChart {
    public class xcPhsChart : Border {

        public enum PERIOD_TYPE {
            DAY,
            WEEK,
            MONTH,
            QUATER,
            YEAR,
            RANGE,
        }

        public enum CHART_TARGET {
            HEIGHT,
            WEIGHT,
            BMI,
            BLOODPRESSURE,
            SYSTOLIC,
            DIASTOLIC,
            PULSE,
            TEMPERATURE,
            BLOOD_SUGAR_AC,
            BLOOD_SUGAR_PC,
            PBF,
        }

        public String DataSource {
            get; set;
        }

        public Uri ParentUri {
            get;
            set;
        }


        private PERIOD_TYPE periodType;
        public PERIOD_TYPE PeriodType {
            get {
                return periodType;
            }
            set {
                periodType = value;
                setDateRange(periodType);
            }
        }

        private CHART_TARGET chartTarget;
        public CHART_TARGET Target {
            get {
                return chartTarget;
            }
            set {
                chartTarget = value;
                if (this.Titles == null || this.YHeaders == null)
                    return;
                switch (chartTarget) {
                    case CHART_TARGET.WEIGHT:
                        Title = this.Titles== null ? "Weight" : this.Titles[0]; // "體重";
                        YHeader = this.Titles == null ?  "Kg" : this.YHeaders[0]; // "公斤/Kg";
                        break;
                    case CHART_TARGET.HEIGHT:
                        Title = this.Titles == null || this.Titles.Length < 2 ? "Height" : this.Titles[1]; // "身高";
                        YHeader = this.Titles == null || this.Titles.Length < 2 ? "cm" : this.YHeaders[1]; // "公分/cm";
                        break;
                    case CHART_TARGET.BMI:
                        Title = this.Titles == null || this.Titles.Length < 3 ? "BMI" : this.Titles[2]; // "身體質量指數(BMI)";
                        YHeader = this.Titles == null || this.Titles.Length < 3 ? "BMI" : this.YHeaders[2]; // "BMI";
                        break;
                    case CHART_TARGET.BLOODPRESSURE:
                        Title = this.Titles == null || this.Titles.Length < 4 ? "Blood Pressure" : this.Titles[3]; // "血壓";
                        YHeader = this.Titles == null || this.Titles.Length < 4 ? "mmHg" : this.YHeaders[3]; // "mmHg";
                        break;
                    case CHART_TARGET.SYSTOLIC:
                        Title = this.Titles == null || this.Titles.Length < 5 ? "Systolic" : this.Titles[4]; // "收縮壓";
                        YHeader = this.Titles == null || this.Titles.Length < 5 ? "mmHg" : this.YHeaders[4]; // "mmHg";
                        break;
                    case CHART_TARGET.DIASTOLIC:
                        Title = this.Titles == null || this.Titles.Length < 6 ? "Diastolic" : this.Titles[5]; // "舒張壓";
                        YHeader = this.Titles == null || this.Titles.Length < 6 ? "mmHg" : this.YHeaders[5]; // "mmHg";
                        break;
                    case CHART_TARGET.PULSE:
                        Title = this.Titles == null || this.Titles.Length < 7 ? "Pulse" : this.Titles[6]; // "脈搏";
                        YHeader = this.Titles == null || this.Titles.Length < 7 ? "beats/min" : this.YHeaders[6]; // "次/分鐘";
                        break;
                }
            }
        }

        public string FieldCode {
            get {
                switch (Target) {
                    case CHART_TARGET.HEIGHT:
                        return xcPhsMeasurementRecord.KEY_HEIGHT;
                    case CHART_TARGET.WEIGHT:
                        return xcPhsMeasurementRecord.KEY_WEIGHT;
                    case CHART_TARGET.BMI:
                        return xcPhsMeasurementRecord.KEY_BMI;
                    case CHART_TARGET.SYSTOLIC:
                        return xcPhsMeasurementRecord.KEY_SYSTOLIC;
                    case CHART_TARGET.DIASTOLIC:
                        return xcPhsMeasurementRecord.KEY_DIASTOLIC;
                    case CHART_TARGET.PULSE:
                        return xcPhsMeasurementRecord.KEY_PULSE;
                    case CHART_TARGET.TEMPERATURE:
                        return xcPhsMeasurementRecord.KEY_TEMPERATURE;
                    case CHART_TARGET.BLOOD_SUGAR_AC:
                        return xcPhsMeasurementRecord.KEY_BLOOD_SUGAR_AC;
                    case CHART_TARGET.BLOOD_SUGAR_PC:
                        return xcPhsMeasurementRecord.KEY_BLOOD_SUGAR_PC;
                    case CHART_TARGET.PBF:
                        return xcPhsMeasurementRecord.KEY_PBF;
                    default:
                        return string.Empty;
                }

            }
        }
        public string LangCode {
            get {
                return xcBinder.AppProperties.Language;
            }
        }

        private CultureInfo CultureInfo {
            get {
                return CultureInfo.CreateSpecificCulture(LangCode);
            }
        }

        public DateTime FromDate {
            get;
            set;
        }

        public DateTime ToDate {
            get;
            set;
        }

        private int Days {
            get {
                return (int)(ToDate - FromDate).TotalDays;
            }
        }

        public double MinValue {
            get;
            set;
        }

        public double MaxValue {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string[] Titles
        {
            get; set;
        }

        public string XHeader {
            get;
            set;
        }

        public string YHeader {
            get;
            set;
        }

        public string[] YHeaders
        {
            get; set;
        }

        public Thickness LabelArea{
            get;set;
        }

        public FontFamily TitleFontFamily {
            get;
            set;
        }

        public double TitleFontSize {
            get;
            set;
        }

        public FontWeight TitleFontWeight {
            get;
            set;
        }

        public FontStretch TitleFontStretch {
            get;
            set;
        }

        public FontStyle TitleFontStyle {
            get;
            set;
        }

        public FontFamily LabelFontFamily {
            get;
            set;
        }

        public double LabelFontSize {
            get;
            set;
        }

        public FontWeight LabelFontWeight {
            get;
            set;
        }

        public FontStretch LabelFontStretch {
            get;
            set;
        }

        public FontStyle LabelFontStyle {
            get;
            set;
        }

        public FontFamily HeaderFontFamily {
            get;
            set;
        }

        public double HeaderFontSize {
            get;
            set;
        }

        public FontWeight HeaderFontWeight {
            get;
            set;
        }

        public FontStretch HeaderFontStretch {
            get;
            set;
        }

        public FontStyle HeaderFontStyle {
            get;
            set;
        }

        public Brush YSeparator {
            get;
            set;
        }

        public Brush XSeparator {
            get;
            set;
        }

        public Brush HolidaySeparator {
            get;
            set;
        }

        public Brush TodaySeparator {
            get;
            set;
        }

        public Brush NormalValue {
            get;
            set;
        }
        public Brush AbnormalValue{
            get;
            set;
        }

        public Brush ValueLine {
            get;
            set;
        }

        /// <summary>
        /// Chart container
        /// </summary>
        private Canvas Canvas { 
            get; 
            set; 
        }

        protected double XGap {
            get;
            set;
        }
        protected double YGap {
            get;
            set;
        }


        protected Polyline PolyLine;
        protected Polyline PolyLineSystolic;
        protected Polyline PolyLineDiastolic;
        protected xcPhsValueList PhsValueList;
        protected xcPhsValueList PhsValueListSystolic;
        protected xcPhsValueList PhsValueListDiastolic;
        protected bool IsSystolicLoaded = false;
        protected bool IsDiastolicLoaded = false;
        public xcPhsChart(PERIOD_TYPE type, string language = "zh-TW") {
            PeriodType = type;
            PhsValueList = new xcPhsValueList();
            PhsValueList.OnLoadListCompleted = OnLoadListCompleted;

            PhsValueListSystolic = new xcPhsValueList();
            PhsValueListSystolic.OnLoadListCompleted = OnLoadBpListCompleted;
            PhsValueListDiastolic = new xcPhsValueList();
            PhsValueListDiastolic.OnLoadListCompleted = OnLoadBpListCompleted;

            iniitializeComponents();
        }

        public xcPhsChart(string language = "zh-TW") {
            PhsValueList = new xcPhsValueList();

            iniitializeComponents();
        }

        public xcPhsChart(xcJObject jProperties, Uri parentUri) {
            iniitializeComponents();

            ParentUri = parentUri;
            apply(jProperties);
            if (String.IsNullOrEmpty(DataSource))
                DataSource = "phs";

            PhsValueList = new xcPhsValueList(DataSource);
            PhsValueList.OnLoadListCompleted = OnLoadListCompleted;
            PhsValueListSystolic = new xcPhsValueList(DataSource);
            PhsValueListSystolic.OnLoadListCompleted = OnLoadBpListCompleted;
            PhsValueListDiastolic = new xcPhsValueList(DataSource);
            PhsValueListDiastolic.OnLoadListCompleted = OnLoadBpListCompleted;

        }

        private void apply(xcJObject jproperties) {
            foreach (var property in jproperties.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    foreach (JToken childProperty in property.Value) {
                        apply(new xcJProperty(new JProperty(property.Name, childProperty), ParentUri));
                    }
                } else {
                    // process properites
                    apply(new xcJProperty(property, ParentUri));
                }
            }
            
        }

        public void apply(xcJProperty property) {
            switch (property.Key) {
                case "name":
                    Name = property.String;
                    break;
                case "width":
                    this.Width = property.Float;
                    break;
                case "height":
                    this.Height = property.Float;
                    break;
                case "size":
                    XiMPLib.xType.xcSize size = property.Size;
                    if (size.hasSize()) {
                        this.Width = size.Width;
                        this.Height = size.Height;
                    }
                    break;
                case "left":
                case "x":
                    Canvas.SetLeft(this, property.Float);
                    break;
                case "top":
                case "y":
                    Canvas.SetTop(this, property.Float);
                    break;
                case "location":
                    System.Windows.Point location = property.Point;
                    Canvas.SetLeft(this, location.X);
                    Canvas.SetTop(this, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(this, property.Integer);
                    break;
                case "horizontalalignment":
                    this.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    this.VerticalAlignment = property.VerticalAlignment;
                    break;
                case "margin":
                    this.Margin = property.Thickness;
                    break;
                case "padding":
                    this.Padding = property.Thickness;
                    break;
                case "cursor":
                    Cursor = property.Cursor;
                    break;
                case "opacity":
                    this.Opacity = property.getFloat(100);
                    break;
                case "background":
                    this.Background = property.Brush;
                    break;
                case "borderthickness":
                    this.BorderThickness = property.Thickness;
                    break;
                case "borderbrush":
                    BorderBrush = property.Brush;
                    break;
                case "cornerradius":
                    this.CornerRadius = property.CornerRadius;
                    break;
                case "visibility":
                    this.Visibility = property.Visibility;
                    break;
                case "enabled":
                case "isenabled":
                    this.IsEnabled = property.Boolean;
                    break;
                case "flowdirection":
                    this.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "effect":
                    this.Effect = property.Effect;
                    break;
                case "period":
                    this.PeriodType = (PERIOD_TYPE)Enum.Parse(typeof(PERIOD_TYPE), property.String, true);
                    break;
                case "target":
                    this.Target = (CHART_TARGET)Enum.Parse(typeof(CHART_TARGET), property.String, true);
                    break;
                case "fromdate":
                    this.FromDate = DateTime.Parse(property.String);
                    break;
                case "todate":
                    this.ToDate = DateTime.Parse(property.String);
                    break;
                case "minvalue":
                    this.MinValue = property.Float;
                    break;
                case "maxvalue":
                    this.MaxValue = property.Float;
                    break;
                case "title":
                    this.Titles = property.StringArray;
                    this.Title = this.Titles[0];
                    break;
                case "xheader":
                    this.XHeader = property.String;
                    break;
                case "yheader":
                    this.YHeaders = property.StringArray;
                    this.YHeader = this.YHeaders[0];
                    break;
                case "labelarea":
                    this.LabelArea = property.Thickness;
                    break;
                case "titlefont":
                    xcFontProperty titleFontProperty = property.FontProperty;
                    if (titleFontProperty.isFont()) {
                        this.TitleFontFamily = titleFontProperty.FontFamily;
                        if (titleFontProperty.FontWeight != null)
                            this.TitleFontWeight = titleFontProperty.FontWeight;
                        if (titleFontProperty.FontSize > 0)
                            this.TitleFontSize = titleFontProperty.FontSize;
                        if (titleFontProperty.FontStyle != null)
                            this.TitleFontStyle = titleFontProperty.FontStyle;
                        if (titleFontProperty.FontStretch != null)
                            this.TitleFontStretch = titleFontProperty.FontStretch;
                    }
                    break;
                case "labelfont":
                    xcFontProperty labelFontProperty = property.FontProperty;
                    if (labelFontProperty.isFont()) {
                        this.LabelFontFamily = labelFontProperty.FontFamily;
                        if (labelFontProperty.FontWeight != null)
                            this.LabelFontWeight = labelFontProperty.FontWeight;
                        if (labelFontProperty.FontSize > 0)
                            this.LabelFontSize = labelFontProperty.FontSize;
                        if (labelFontProperty.FontStyle != null)
                            this.LabelFontStyle = labelFontProperty.FontStyle;
                        if (labelFontProperty.FontStretch != null)
                            this.LabelFontStretch = labelFontProperty.FontStretch;
                    }
                    break;
                case "headerfont":
                    xcFontProperty headerFontProperty = property.FontProperty;
                    if (headerFontProperty.isFont()) {
                        this.HeaderFontFamily = headerFontProperty.FontFamily;
                        if (headerFontProperty.FontWeight != null)
                            this.HeaderFontWeight = headerFontProperty.FontWeight;
                        if (headerFontProperty.FontSize > 0)
                            this.HeaderFontSize = headerFontProperty.FontSize;
                        if (headerFontProperty.FontStyle != null)
                            this.HeaderFontStyle = headerFontProperty.FontStyle;
                        if (headerFontProperty.FontStretch != null)
                            this.HeaderFontStretch = headerFontProperty.FontStretch;
                    }
                    break;
                case "yseparator":
                    this.YSeparator = property.Brush;
                    break;
                case "xseparator":
                    this.XSeparator = property.Brush;
                    break;
                case "holidayseparator":
                    this.HolidaySeparator = property.Brush;
                    break;
                case "todayseparator":
                    this.TodaySeparator = property.Brush;
                    break;
                case "valuecircle":
                    this.NormalValue = property.Brush;
                    break;
                case "abnormalvaluecircle":
                    this.AbnormalValue = property.Brush;
                    break;
                case "valueline":
                    this.ValueLine = property.Brush;
                    break;
                case "datasource":
                    this.DataSource = property.String;
                    break;
                default:
                    break;
            }
        }


        protected void iniitializeComponents() {
            this.Background = new LinearGradientBrush(Color.FromRgb(0xf1, 0xf8, 0xf3), Color.FromRgb(0xcc, 0xea, 0xf4), new Point(400, 50), new Point(400, 100));
            this.CornerRadius = new CornerRadius(20.0);

            TitleFontFamily = new FontFamily("SimHei");
            TitleFontSize = 20;
            TitleFontWeight = FontWeights.Bold;

            HeaderFontFamily = new FontFamily("SimHei");
            HeaderFontSize = 14;
            HeaderFontWeight = FontWeights.Bold;

            LabelFontFamily = new FontFamily("SimHei");
            LabelFontSize = 12;

            SnapsToDevicePixels = true;
            this.LabelArea = new Thickness(60, 50, 60, 50);
            Canvas = new Canvas();
            Canvas.Margin = new Thickness(10);
            this.Child = this.Canvas;
            this.Canvas.IsVisibleChanged +=Canvas_IsVisibleChanged;

            TodaySeparator = Brushes.SkyBlue;
            HolidaySeparator = Brushes.White;
            XSeparator = YSeparator = Brushes.White;

            NormalValue = Brushes.Green;
            ValueLine = Brushes.LightSkyBlue;
        }

        private void Canvas_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) {
                updateData();
            }
        }

        public void updateData() {
            Canvas.Children.Clear();
            drawTitle();
            this.drawXAxis();

            if (Target == CHART_TARGET.BLOODPRESSURE) {
                IsSystolicLoaded = false;
                IsDiastolicLoaded = false;

                PolyLineSystolic = new Polyline();
                PolyLineSystolic.StrokeThickness = 3.0;
                PolyLineSystolic.Stroke = ValueLine;

                PolyLineDiastolic = new Polyline();
                PolyLineDiastolic.StrokeThickness = 3.0;
                PolyLineDiastolic.Stroke = ValueLine;

                PhsValueListSystolic.loadList(xcPhsMeasurementRecord.KEY_SYSTOLIC, FromDate.AddDays(1), ToDate);
                PhsValueListDiastolic.loadList(xcPhsMeasurementRecord.KEY_DIASTOLIC, FromDate.AddDays(1), ToDate);
            } else {
                PolyLine = new Polyline();
                PolyLine.StrokeThickness = 3.0;
                PolyLine.Stroke = ValueLine;
                PhsValueList.loadList(FieldCode, FromDate.AddDays(1), ToDate);
            }
        }

        private void OnLoadListCompleted(xcPhsValueList list) {
            if (list != null && list.Count > 0) {
                MinValue = list.MinValue;
                MaxValue = list.MaxValue;
            } else {
                switch (Target) {
                    case CHART_TARGET.HEIGHT:
                        MinValue = 100;
                        MaxValue = 250;
                        break;
                    case CHART_TARGET.WEIGHT:
                        MinValue = 30;
                        MaxValue = 180;
                        break;
                    case CHART_TARGET.BLOODPRESSURE:
                    case CHART_TARGET.SYSTOLIC:
                    case CHART_TARGET.DIASTOLIC:
                        MinValue = 80;
                        MaxValue = 120;
                        break;
                    case CHART_TARGET.BMI:
                        MinValue = 20;
                        MaxValue = 40;
                        break;
                    default:
                        MinValue = 20;
                        MaxValue = 100;
                        break;
                }
            }

            drawYAxis();
            try {
                Canvas.Children.Add(PolyLine);
                if (list != null)
                {
                    foreach (DateTime dateTime in list.Keys)
                    {
                        if (dateTime > ToDate || dateTime < FromDate.AddDays(1))
                            continue;
                        drawValue((DateTime)dateTime, ((xcPhsValue)list[dateTime]).Value);
                    }
                }
            }
            catch
            {

            }
        }

        
        private void OnLoadBpListCompleted(xcPhsValueList list) {
            if (list != null && list.Equals(PhsValueListDiastolic))
                IsDiastolicLoaded = true;
            if (list != null && list.Equals(PhsValueListSystolic))
                IsSystolicLoaded = true;

            if (IsDiastolicLoaded && IsSystolicLoaded) {
                if (PhsValueListSystolic != null && PhsValueListSystolic.Count > 0){
                    MaxValue = PhsValueListSystolic.MaxValue;
                    MinValue = PhsValueListSystolic.MinValue;
                } else {
                    MinValue = 80;
                    MaxValue = 120;
                }

                if (PhsValueListDiastolic != null && PhsValueListDiastolic.Count > 0){
                    if (MaxValue < PhsValueListDiastolic.MaxValue)
                        MaxValue = PhsValueListDiastolic.MaxValue;
                    if (MinValue > PhsValueListDiastolic.MinValue)
                        MinValue = PhsValueListDiastolic.MinValue;
                }

                drawYAxis();
                try {
                    Canvas.Children.Add(PolyLineSystolic);
                    Canvas.Children.Add(PolyLineDiastolic);
                    if (PhsValueListSystolic != null) {
                        foreach (var dateTime in PhsValueListSystolic.Keys) {
                            drawValue(PhsValueListSystolic, (DateTime)dateTime, ((xcPhsValue)PhsValueListSystolic[dateTime]).Value);
                        }
                    }

                    if (PhsValueListDiastolic != null) {
                        foreach (var dateTime in PhsValueListDiastolic.Keys) {
                            drawValue(PhsValueListDiastolic, (DateTime)dateTime, ((xcPhsValue)PhsValueListDiastolic[dateTime]).Value);
                        }
                    }
                }
                catch {

                }
            }

        }

        protected void drawValue(DateTime date, double value) {
            int days = (int)(date - FromDate).TotalDays -1;

            Ellipse circle = new Ellipse();
            circle.Width = 14;
            circle.Height = 14;
            circle.Stroke = NormalValue;
            circle.Fill = Brushes.White;
            circle.StrokeThickness = 3;
            double minGap = XGap / (24 * 60);
            double xLocation = LabelArea.Left + (double)days * XGap - circle.Width / 2 + minGap; // *new TimeSpan(date.Hour, date.Millisecond, date.Second).TotalMinutes;
            double yLocation = LabelArea.Top - 10 + (MaxValue - value) * YGap + circle.Height/2-4;

            PolyLine.Points.Add(new Point(xLocation + 8.0, yLocation + 8.0));

            Canvas.SetLeft(circle, xLocation);
            Canvas.SetTop(circle, yLocation);
            Canvas.Children.Add(circle);
        }

        protected void drawValue(xcPhsValueList list, DateTime date, double value) {
            int days = (int)(date - FromDate).TotalDays - 1;

            Ellipse circle = new Ellipse();
            circle.Width = 14;
            circle.Height = 14;
            circle.Stroke = NormalValue;
            circle.Fill = Brushes.White;
            circle.StrokeThickness = 3;
            double minGap = XGap / (24 * 60);
            double xLocation = LabelArea.Left + (double)days * XGap - circle.Width / 2 + minGap; // *new TimeSpan(date.Hour, date.Millisecond, date.Second).TotalMinutes;
            //double yLocation = LabelArea.Top + (MaxValue + 19 - value) * YGap + circle.Height / 2 - 4;
            double yLocation = LabelArea.Top - 10 + (MaxValue - value) * YGap + circle.Height / 2 - 4;
            Polyline line = null;

            if (list.Equals(PhsValueListSystolic))
                line = PolyLineSystolic;
            else if (list.Equals(PhsValueListDiastolic))
                line = PolyLineDiastolic;
            if (line != null)
                line.Points.Add(new Point(xLocation + 8.0, yLocation + 8.0));
            
            Canvas.SetLeft(circle, xLocation);
            Canvas.SetTop(circle, yLocation);
            Canvas.Children.Add(circle);
        }

        protected void drawTitle() {
            Label title = new Label();
            title.Content = Title;
            title.FontSize = this.TitleFontSize;
            title.FontFamily = TitleFontFamily;
            title.FontWeight = TitleFontWeight;
            title.FontStretch = TitleFontStretch;
            title.FontStyle = TitleFontStyle;
            title.Width = this.Width;
            title.Height = LabelArea.Top;
            title.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            title.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            this.Canvas.Children.Add(title);
        }

        protected void drawXAxis() {
            Polyline line;
            Label label;
            DateTime date = FromDate.Date;
            int days = (int)Days;

            double startX = LabelArea.Left;
            XGap = this.Width - LabelArea.Left - LabelArea.Right;
            if (days == 1)
                XGap /= 23;
            else
                XGap /= days - 1;

            //Draw X-Axis
            for (int i = 0; i < days; i++) {
                date += new TimeSpan(1, 0, 0, 0, 0);

                // Draw X-Axis line
                line = new Polyline();
                if (i == 0) {
                    line.Points.Add(new Point(startX, LabelArea.Top));
                    line.Points.Add(new Point(startX, Height - LabelArea.Bottom+5));
                    line.Stroke = Brushes.Black;
                    line.StrokeThickness = 3.0;
                    Canvas.Children.Add(line);
                } else {
                    //line.StrokeDashArray = DoubleCollection.Parse("2, 2");
                    line.Points.Add(new Point(startX, LabelArea.Top));
                    line.Points.Add(new Point(startX, Height - LabelArea.Bottom));
                    if (days <= 10) {
                        if (date.Equals(DateTime.Today)) {
                            line.Stroke = TodaySeparator;
                            line.StrokeThickness = 2.0;
                        } else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) {
                            line.Stroke = HolidaySeparator;
                            line.StrokeThickness = 1.0;
                        } else {
                            line.Stroke = XSeparator;
                            line.StrokeThickness = 1.0;
                        }
                        Canvas.Children.Add(line);
                    } else if (days > 10 && days <= 50) {
                        if (date.Equals(DateTime.Today)) {
                            line.Stroke = TodaySeparator;
                            line.StrokeThickness = 2.0;
                        } else if (date.DayOfWeek == DayOfWeek.Monday) {
                            line.Stroke = XSeparator;
                            line.StrokeThickness = 2.0;
                        } else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) {
                            line.Stroke = HolidaySeparator;
                            line.StrokeThickness = 1.0;
                        } else {
                            line.Stroke = XSeparator;
                            line.StrokeThickness = 1.0;
                        }
                        Canvas.Children.Add(line);
                    } else if (days > 50 && days <= 100) {
                        if (date.Equals(DateTime.Today)) {
                            line.Stroke = TodaySeparator;
                            line.StrokeThickness = 2.0;
                            Canvas.Children.Add(line);
                        } else if (date.DayOfWeek == DayOfWeek.Monday) {
                            line.Stroke = XSeparator;
                            line.StrokeThickness = 1.0;
                            Canvas.Children.Add(line);
                        }
                        if (date.Day < 7) {
                            line.StrokeThickness = 2.0;
                        }
                    } else {
                        if (date.Equals(DateTime.Today)) {
                            line.Stroke = TodaySeparator;
                            line.StrokeThickness = 2.0;
                            Canvas.Children.Add(line);
                        }else if (date.Day == 1) {
                            line.Stroke = XSeparator;
                            line.StrokeThickness = 1.0;
                            
                            Canvas.Children.Add(line);
                        }
                    }
                }
                // Draw X-Axis label
                label = new Label();
                label.FontSize = LabelFontSize;
                label.FontFamily = LabelFontFamily;
                label.Width = 60;
                label.RenderTransform = new RotateTransform(20);
                label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                Canvas.SetLeft(label, startX - 10);
                Canvas.SetTop(label, Height - LabelArea.Bottom);
                if (days < 10) {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) {
                        label.Foreground = Brushes.Red;
                    }
                    if (XGap > 50) {
                        label.Content = date.ToString("MM.dd\nddd", CultureInfo);
                    } else {
                        label.Content = date.ToString("M.d", CultureInfo);
                    }
                    if (date.Equals(DateTime.Today)) {
                        label.FontWeight = FontWeights.Bold;
                    }
                    Canvas.Children.Add(label);
                } else if (days > 10 && days <= 50) {
                    label.Content = date.ToString("M.d", CultureInfo);
                    if (date.DayOfWeek == DayOfWeek.Monday) {
                        Canvas.Children.Add(label);
                    }
                } else if (days > 50 && days <= 100) {
                    label.Content = date.ToString("yy.M.d", CultureInfo);
                    if (date.DayOfWeek == DayOfWeek.Monday) {
                        Canvas.Children.Add(label);
                    }
                } else {
                    label.Content = date.ToString("yy.M.d", CultureInfo);
                    if (date.Day == 1) {
                        Canvas.Children.Add(label);
                    }
                }
                // Calculate next line
                startX += XGap;
            }
        }

        private void drawYAxis() {
            if (MaxValue == 0)
                return;

            Polyline line;
            Label header;

            // Draw abnormal background
            double overValue = -1;
            double underValue = -1;

            switch (Target) {
                case CHART_TARGET.BLOODPRESSURE:
                case CHART_TARGET.SYSTOLIC:
                case CHART_TARGET.DIASTOLIC:
                    overValue = 120;
                    underValue = 80;
                    break;
                case CHART_TARGET.BMI:
                    overValue = 35;
                    underValue = 18.5;
                    break;
            }
            
            int max = (int)(overValue > MaxValue ? overValue : MaxValue) + 15; // (MaxValue + ( ? overValue - MaxValue : 0) + 20);
            int min = (int)(underValue < MinValue ? underValue : MinValue) - 15; // - ( ? 0 : underValue - MinValue) - 20);
            MaxValue = max;
            MinValue = min;
            int rowCount = max - min;
            YGap = (this.Height - LabelArea.Top - LabelArea.Bottom) / rowCount;
            double startY = LabelArea.Top;

            if (overValue > 0) {
                if (max - overValue < 0)
                    return;
                Rectangle rectangle = new Rectangle();
                rectangle.Stroke = Brushes.Transparent;
                rectangle.StrokeThickness = 0;
                rectangle.Fill = new SolidColorBrush(Color.FromRgb(0xe7, 0xb4, 0xb5));
                rectangle.Width = this.Width - LabelArea.Left - LabelArea.Right;
                rectangle.Height = (max - overValue) * YGap;
                Canvas.SetZIndex(rectangle, -1);
                Canvas.SetLeft(rectangle, LabelArea.Left);
                Canvas.SetTop(rectangle, LabelArea.Top);
                this.Canvas.Children.Add(rectangle);
            }

            if (underValue > 0) {
                Rectangle rectangle = new Rectangle();
                rectangle.Stroke = Brushes.Transparent;
                rectangle.StrokeThickness = 0;
                rectangle.Fill = new SolidColorBrush(Color.FromRgb(0xe7, 0xb4, 0xb5));
                rectangle.Width = this.Width - LabelArea.Left - LabelArea.Right;
                rectangle.Height = (underValue - min) * YGap;
                Canvas.SetZIndex(rectangle, -1);
                Canvas.SetLeft(rectangle, LabelArea.Left);
                Canvas.SetTop(rectangle, this.Height - rectangle.Height - LabelArea.Bottom);
                this.Canvas.Children.Add(rectangle);
            }

            for (int i = 0; i < rowCount + 1; i++) {
                if ((max - i) % 10 == 0) {
                    line = new Polyline();
                    line.Stroke = YSeparator;
                    //line.StrokeDashArray = DoubleCollection.Parse("2, 2");
                    line.Points.Add(new Point(LabelArea.Left + 3, startY));
                    line.Points.Add(new Point(Width - LabelArea.Right, startY));
                    line.StrokeThickness = (max - i) % 50 == 0 ? 2 : 1.0;

                    Canvas.Children.Add(line);

                    // Draw Y-Axis label
                    header = new Label();
                    header.FontSize = LabelFontSize;
                    header.FontFamily = LabelFontFamily; int value = max - i;
                    header.Width = LabelArea.Left - 5;
                    header.Height = 30;
                    header.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                    header.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

                    if (value <= MaxValue + 15) {
                        header.Content = max - i;
                    }

                    Canvas.SetLeft(header, 0);
                    Canvas.SetTop(header, startY - header.Height / 2);
                    if (i >= 10)
                        Canvas.Children.Add(header);
                }
                startY += YGap;
            }
            // Header
            header = new Label();
            header.Width = LabelArea.Left;
            header.Height = 30;
            header.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
            header.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            header.Content = YHeader;
            header.FontFamily = HeaderFontFamily;
            header.FontSize = HeaderFontSize;
            header.FontStretch = HeaderFontStretch;
            header.FontWeight = HeaderFontWeight;
            header.FontStyle = HeaderFontStyle;
            Canvas.SetLeft(header, 0);
            Canvas.SetTop(header, LabelArea.Top);
            Canvas.Children.Add(header);


            // Draw Y Origin
            line = new Polyline();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3.0;
            line.Points.Add(new Point(LabelArea.Left - 5, this.Height - LabelArea.Bottom));
            line.Points.Add(new Point(this.Width - LabelArea.Right + 5, this.Height - LabelArea.Bottom));

            Canvas.Children.Add(line);

        }

        private Size MeasureString(string candidate) {
            var formattedText = new FormattedText(candidate, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(LabelFontFamily, LabelFontStyle, LabelFontWeight, LabelFontStretch), LabelFontSize, Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void setDateRange(PERIOD_TYPE periodType) {
            this.ToDate = DateTime.Today;
            switch (periodType) {
                case PERIOD_TYPE.DAY:
                    this.FromDate = this.ToDate;
                    break;
                case PERIOD_TYPE.WEEK:
                    this.FromDate = this.ToDate.AddDays(-7);
                    break;
                case PERIOD_TYPE.MONTH:
                    this.FromDate = ToDate.AddMonths(-1);
                    break;
                case PERIOD_TYPE.QUATER:
                    this.FromDate = ToDate.AddMonths(-3);
                    break;
                case PERIOD_TYPE.YEAR:
                    this.FromDate = ToDate.AddYears(-1);
                    break;
            }
        }

        public void setTarget(string target) {
            this.Target = (CHART_TARGET)Enum.Parse(typeof(CHART_TARGET), target, true);
            this.updateData();
        }

        public void setPeriod(string period) {
            this.PeriodType = (PERIOD_TYPE)Enum.Parse(typeof(PERIOD_TYPE), period, true);
            this.updateData();
        }

    }
}
