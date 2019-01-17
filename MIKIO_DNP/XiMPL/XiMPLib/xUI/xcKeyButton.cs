using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using XiMPLib.xDocument;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.Web;
using XiMPLib.xBinding;
using System.Drawing;
using XiMPLib.xType;
using XiMPLib.xDevice;
using XiMPLib.xSystem;

namespace XiMPLib.xUI {
    public class xcKeyButton : Border {
        public Uri ParentUri {
            get;
            set;
        }
        
        private xcBrushes Backgrounds {
            get;
            set;
        }

        public String Value {
            get;
            set;
        }

        public String ShiftValue {
            get;
            set;
        }

        public string DisplayValue{
            get {
                string value = IsShifted ? displayShiftValue : displayValue;
                return value == null || string.IsNullOrEmpty(value) ? (IsShifted ? ShiftValue : Value) : value;
            }
        }

        public bool IsShifted {
            get;
            set;
        }

        private string displayValue;
        private string displayShiftValue;

        public xcKeyButton(xcJObject jProperties, Uri parentUri) {
            ParentUri = parentUri;
            IsShifted = false;
            apply(jProperties);
            xcTextView valueControl = getControl("value");
            if (valueControl != null)
                valueControl.Text = DisplayValue;
            xcTextView shiftValueControl = getControl("shiftvalue");
            if (shiftValueControl != null)
                shiftValueControl.Text = DisplayValue;
        }

        private xcTextView getControl(string name) {
            var type = this.Child.GetType();
            if (type == typeof(xcCanvas)){
                xcCanvas canvas = (xcCanvas)this.Child;
                return getValueControl(canvas.Children, name);
            } else if (type == typeof(xcStackPanel)) {
                xcStackPanel stackPanel = (xcStackPanel)this.Child;
                return getValueControl(stackPanel.Children, name);
            }
            return null;
        }

        private xcTextView getValueControl(UIElementCollection children, string name) {
            foreach (var child in children) {
                if (child.GetType() == typeof(xcTextView)) {
                    xcTextView textView = (xcTextView)child;
                    if (textView.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        return textView;
                }
            }
            return null;
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
                    if (property.String.IndexOf("|") < 0) {
                        this.Background = property.Brush;
                    } else {
                        setBackgrounds(property.Brushes);
                    }
                    break;
                case "backgrounds":
                    setBackgrounds(property.Brushes);
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
                case "focusable":
                    this.Focusable = property.Boolean;
                    break;
                case "flowdirection":
                    this.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "layout":
                    xcLayout layout = property.Layout;
                    this.Child = (System.Windows.UIElement)layout.Panel;
                    break;
                case "control":
                    xcControl control = property.Control;
                    this.Child = (System.Windows.UIElement)control.Control;
                    break;
                case "effect":
                    this.Effect = property.Effect;
                    break;
                case "value":
                    this.Value = property.String;
                    break;
                case "displayvalue":
                    this.displayValue = property.String;
                    break;
                case "shiftvalue":
                    this.ShiftValue = property.String;
                    break;
                case "displayshiftvalue":
                    this.displayShiftValue = property.String;
                    break;
                default:
                    
                    break;
            }
        }

        private void setBackgrounds(xcBrushes brushes) {
            this.Backgrounds = brushes;
            this.Background = Backgrounds.Normal;
        }


        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseEnter(e);
            if (Backgrounds != null)
                this.Background = e.LeftButton == System.Windows.Input.MouseButtonState.Pressed ? Backgrounds.Pressed : Backgrounds.Hover;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseLeave(e);
            if (Backgrounds != null)
                this.Background = Backgrounds.Normal;
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            if (Backgrounds != null)
                this.Background = Backgrounds.Pressed;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            if (Backgrounds != null)
                this.Background = Backgrounds.Normal;

            string value = Value;
            if (IsShifted && !string.IsNullOrEmpty(ShiftValue))
                value = ShiftValue;

            if (!string.IsNullOrEmpty(value)) {
                if (Value.Length == 1)
                    xcKeyEvent.raiseKeybdUpDownEvent(value[0]);
                else
                    xcKeyEvent.raiseFnKeyUpDownEvent(value);
            }

        }
    }
}
