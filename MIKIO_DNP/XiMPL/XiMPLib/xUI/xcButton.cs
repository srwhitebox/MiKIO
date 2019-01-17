using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XiMPLib.xDocument;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Web;
using System.Collections.Specialized;
using XiMPLib.xBinding;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;

namespace XiMPLib.xUI {
    public class xcButton : Button {
        Panel Panel {
            get;
            set;
        }

        private Uri ClickEventUri {
            get;
            set;
        }

        private Uri ActionUri {
            get;
            set;
        }

        private Uri ParentUri {
            get;
            set;
        }

        public xcButton(xcJObject jProperties, Uri parentUri=null) {
            ParentUri = parentUri;

            apply(jProperties);
            setStyle();
            this.Click += xcButton_Click;
        }


        private void xcButton_Click(object sender, RoutedEventArgs e) {
            if (ActionUri != null)
                xcBinder.doAction(this.ActionUri);
            if (ClickEventUri != null)
                xcBinder.doAction(ClickEventUri);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            Console.WriteLine("mouse Up");
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            Console.WriteLine("mouse Down");
            base.OnMouseDown(e);
            setStyle();
        }

        private void setStyle() {
            Style style = new System.Windows.Style(typeof(Button));
            Trigger triggerIsPressed = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            //triggerIsPressed. = new Binding() { Path = new PropertyPath("Content"), RelativeSource = RelativeSource.Self };
            triggerIsPressed.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));
            //triggerIsPressed.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.Red), "border"));
            style.Triggers.Add(triggerIsPressed);
            this.Style = style;

            //this.Style//.Triggers.Add(triggerIsPressed);
            //this.Triggers.Add(triggerIsPressed);
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
                case "horizontalalignment":
                    this.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    this.VerticalAlignment = property.VerticalAlignment;
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
                case "focusable":
                    this.Focusable = property.Boolean;
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
                case "flowdirection":
                    this.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "layout":
                    xcLayout layout = property.Layout;
                    Panel = layout.Panel;
                    this.AddChild((System.Windows.UIElement)Panel);
                    break;
                case "value":
                case "text":
                case "content":
                    break;
                case "isdefault":
                    this.IsDefault = property.Boolean;
                    break;
                case "iscancel":
                    this.IsCancel = property.Boolean;
                    break;
                case "style":
                    break;
                case "click":
                    ClickEventUri = property.Uri;
                    //NameValueCollection queryParameters = HttpUtility.ParseQueryString(ClickEventUri.Query);   
                    break;
                case "action":
                    this.ActionUri = property.Uri;
                    break;
                default:
                    // Add controls
                    break;
            }
        }
    }
}
