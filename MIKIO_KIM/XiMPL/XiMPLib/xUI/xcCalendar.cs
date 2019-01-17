using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using XiMPLib.xBinding;
using XiMPLib.xDocument;

namespace XiMPLib.xUI {
    public class xcCalendar : Calendar {
        private Uri ParentUri {
            get;
            set;
        }

        private Uri DateChangedAction {
            get;
            set;
        }

        private bool IsCalendarOpen {
            get;
            set;
        }

        private bool BlackoutToday {
            get;
            set;
        }

        private Binding Binding;

        public xcCalendar(xcJObject jProperties, Uri parentUri = null) {
            ParentUri = parentUri;
            this.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime lastDate = DateTime.Today.AddMonths(3);
            lastDate = new DateTime(lastDate.Year, lastDate.Month, DateTime.DaysInMonth(lastDate.Year, lastDate.Month));
            this.SelectionMode = CalendarSelectionMode.SingleDate;
            this.DisplayDateEnd = lastDate;
            this.Focusable = false;
            this.BlackoutToday = false;
            this.IsTodayHighlighted = false;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            apply(jProperties);
            this.IsVisibleChanged += xcCalendar_IsVisibleChanged;
            if (Binding != null)
                SetBinding(Calendar.SelectedDateProperty, Binding);
            this.SelectedDates.Clear();
            this.SelectedDatesChanged += xcCalendar_SelectedDatesChanged;
        }

        void xcCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
            if (DateChangedAction != null && this.SelectedDates.Count > 0 && this.Visibility == System.Windows.Visibility.Visible) {
                xcBinder.doAction(DateChangedAction);
            }
        }

        void xcCalendar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.NewValue) {
                this.SelectedDates.Clear();
                if (this.BlackoutToday) {
                    this.BlackoutDates.Add(new CalendarDateRange(DateTime.Today));
                }
            }
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
                    Point location = property.Point;
                    Canvas.SetLeft(this, location.X);
                    Canvas.SetTop(this, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(this, property.Integer);
                    break;
                case "font":
                    xcFontProperty fontProperty = property.FontProperty;
                    if (fontProperty.isFont()) {
                        this.FontFamily = fontProperty.FontFamily;
                        if (fontProperty.FontWeight != null)
                            this.FontWeight = fontProperty.FontWeight;
                        if (fontProperty.FontSize > 0)
                            this.FontSize = fontProperty.FontSize;
                        if (fontProperty.FontStyle != null)
                            this.FontStyle = fontProperty.FontStyle;
                        if (fontProperty.FontStretch != null)
                            this.FontStretch = fontProperty.FontStretch;
                    }
                    break;
                case "fontfamily":
                    this.FontFamily = property.FontFamily;
                    break;
                case "fontsize":
                    XiMPLib.xType.xcLength length = new XiMPLib.xType.xcLength(property.String, XiMPLib.xType.xcLength.UNIT.POINT, xcUiProperty.getDpiY());
                    this.FontSize = length.Point;
                    break;
                case "fontweight":
                    this.FontWeight = property.FontWeight;
                    break;
                case "fontstyle":
                    this.FontStyle = property.FontStyle;
                    break;
                case "fontstretch":
                    this.FontStretch = property.FontStretch;
                    break;
                case "margin":
                    this.Margin = property.Thickness;
                    break;
                case "borderthickness":
                    this.BorderThickness = property.Thickness;
                    break;
                case "cursor":
                    Cursor = property.Cursor;
                    break;
                case "opacity":
                    this.Opacity = property.getFloat(100);
                    break;
                case "visibility":
                    this.Visibility = property.Visibility;
                    break;
                case "enabled":
                case "isenabled":
                    this.IsEnabled = property.Boolean;
                    break;
                case "background":
                    this.Background = property.Brush;
                    break;
                case "foreground":
                    this.Foreground = property.Brush;
                    break;
                case "borderbrush":
                    BorderBrush = property.Brush;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "firstdayofweek":
                    this.FirstDayOfWeek = property.DayOfWeek;
                    break;
                case "startdate":
                    this.DisplayDateStart = property.Date;
                    break;
                case "enddate":
                    this.DisplayDateEnd = property.Date;
                    break;
                case "datechangedaction":
                    this.DateChangedAction = property.Uri;
                    break;
                case "datasource":
                    this.DataContext = xcBinder.getBinder(property.String);
                    break;
                case "blackoutpastdates":
                    if (property.Boolean)
                        this.BlackoutDates.AddDatesInPast();
                    break;
                case "blackouttoday":
                    this.BlackoutToday = property.Boolean;
                    break;
                case "value":
                case "text":
                case "content":
                    if (property.isBindProperty) {
                        if (Binding == null)
                            Binding = new Binding();
                        Binding.Path = new PropertyPath(property.BindProperty);
                        // this.SetBinding(TextProperty, property.BindProperty);
                    }
                    break;
                case "iscalendaropen":
                    this.IsCalendarOpen = property.Boolean;
                    break;
                default:
                    break;
            }
        }
    }
}
