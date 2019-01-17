using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XiMPLib.xDocument;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Globalization;

using XiMPLib.xBinding;
using XiMPLib.xType;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Data;
using System.Reflection;

namespace XiMPLib.xUI {
    public class xcPasswordText{
        public Uri ParentUri {
            get;
            set;
        }

        private Uri ClickEventUri {
            get;
            set;
        }

        private xcBrushes Backgrounds {
            get;
            set;
        }

        private xcBrushes Foregrounds {
            get;
            set;
        }

        private bool RequireFocus { 
            get; 
            set; 
        }

        private Binding Binding;
        public PasswordBox PasswordBox {
            get;
            set;
        }

        private TextBox TextBlock;
        public xcPasswordText(xcJObject jProperties, Uri parentUri) {
            PasswordBox = new System.Windows.Controls.PasswordBox();
            TextBlock = new TextBox();
            TextBlock.Focusable = false;
            ParentUri = parentUri;
            this.RequireFocus = false;
            apply(jProperties);

            PasswordBox.IsVisibleChanged += xcEditText_IsVisibleChanged;
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            if (Binding != null) {
                TextBlock.SetBinding(TextBox.TextProperty, Binding);
                TextBlock.TextChanged += TextBlock_TextChanged;
            }
        }

        void TextBlock_TextChanged(object sender, TextChangedEventArgs e) {
            PasswordBox.Password = TextBlock.Text;
        }

        void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) {
            //TextBlock.Text = PasswordBox.Password;
            //TextBlock.InvalidateProperty(TextBox.TextProperty);
            moveCursorToEnd();
            xcBinder.Progress.clearMessage();
            //xcBinder.Patient.Password = this.PasswordBox.Password;
            TextBlock.Text = PasswordBox.Password;
            
        }

        private void setSelection(int start, int length) {
            PasswordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(PasswordBox, new object[] { start, length });
        }

        private void moveCursorToEnd() {
            setSelection(PasswordBox.Password.Length, 0);
        }

        void xcEditText_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (this.RequireFocus)
                PasswordBox.Focus();
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
                    PasswordBox.Name = property.String;
                    break;
                case "width":
                    PasswordBox.Width = property.Float;
                    break;
                case "height":
                    PasswordBox.Height = property.Float;
                    break;
                case "size":
                    XiMPLib.xType.xcSize size = property.Size;
                    if (size.hasSize()) {
                        PasswordBox.Width = size.Width;
                        PasswordBox.Height = size.Height;
                    }
                    break;
                case "left":
                case "x":
                    Canvas.SetLeft(PasswordBox, property.Float);
                    break;
                case "top":
                case "y":
                    Canvas.SetTop(PasswordBox, property.Float);
                    break;
                case "location":
                    Point location = property.Point;
                    Canvas.SetLeft(PasswordBox, location.X);
                    Canvas.SetTop(PasswordBox, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(PasswordBox, property.Integer);
                    break;
                case "font":
                    xcFontProperty fontProperty = property.FontProperty;
                    if (fontProperty.isFont()) {
                        PasswordBox.FontFamily = fontProperty.FontFamily;
                        if (fontProperty.FontWeight != null)
                            PasswordBox.FontWeight = fontProperty.FontWeight;
                        if (fontProperty.FontSize > 0)
                            PasswordBox.FontSize = fontProperty.FontSize;
                        if (fontProperty.FontStyle != null)
                            PasswordBox.FontStyle = fontProperty.FontStyle;
                        if (fontProperty.FontStretch != null)
                            PasswordBox.FontStretch = fontProperty.FontStretch;
                    }
                    break;
                case "fontfamily":
                    PasswordBox.FontFamily = property.FontFamily;
                    break;
                case "fontsize":
                    XiMPLib.xType.xcLength length = new XiMPLib.xType.xcLength(property.String, XiMPLib.xType.xcLength.UNIT.POINT, xcUiProperty.getDpiY());
                    PasswordBox.FontSize = length.Point;
                    break;
                case "fontweight":
                    PasswordBox.FontWeight = property.FontWeight;
                    break;
                case "fontstyle":
                    PasswordBox.FontStyle = property.FontStyle;
                    break;
                case "fontstretch":
                    PasswordBox.FontStretch = property.FontStretch;
                    break;
                case "horizontalalignment":
                    PasswordBox.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    PasswordBox.VerticalContentAlignment = property.VerticalAlignment;
                    break;
                case "textalignment":
                    PasswordBox.HorizontalContentAlignment = property.HorizontalAlignment;
                    break;
                case "margin":
                    PasswordBox.Margin = property.Thickness;
                    break;
                case "padding":
                    PasswordBox.Padding = property.Thickness;
                    break;
                case "cursor":
                    PasswordBox.Cursor = property.Cursor;
                    break;
                case "opacity":
                    PasswordBox.Opacity = property.getFloat(100);
                    break;
                case "visibility":
                    PasswordBox.Visibility = property.Visibility;
                    break;
                case "enabled":
                case "isenabled":
                    PasswordBox.IsEnabled = property.Boolean;
                    break;
                case "focusable":
                    PasswordBox.Focusable = property.Boolean;
                    break;
                case "requirefocus":
                    this.RequireFocus = property.Boolean;
                    break;
                case "background":
                    if (property.String.IndexOf("|") < 0)
                        PasswordBox.Background = property.Brush;
                    else
                        setBackgrounds(property.Brushes);
                    break;
                case "backgrounds":
                    setBackgrounds(property.Brushes);
                    break;
                case "foreground":
                    if (property.String.IndexOf("|") < 0)
                        PasswordBox.Foreground = property.Brush;
                    else
                        setForegrounds(property.Brushes);
                    break;
                case "foregrounds":
                    setForegrounds(property.Brushes);
                    break;
                case "flowdirection":
                    PasswordBox.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    PasswordBox.RenderTransform = property.Transform;
                    break;
                case "effect":
                    PasswordBox.Effect = property.Effect;
                    break;
                case "maxlength":
                    PasswordBox.MaxLength = property.Integer;
                    break;
                case "password":
                case "value":
                case "text":
                case "content":
                    if (property.isBindProperty) {
                        if (Binding == null) {
                            Binding = new Binding();
                            Binding.Mode = BindingMode.TwoWay;
                            Binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        }
                        Binding.Path = new PropertyPath(property.BindProperty);
                        // this.SetBinding(TextProperty, property.BindProperty);
                    } else
                        PasswordBox.Password = property.String;
                    break;
                case "stringformat":
                        if (Binding == null)
                            Binding = new Binding();
                        Binding.StringFormat = property.String;
                    break;
                case "datasource":
                    this.TextBlock.DataContext = xcBinder.getBinder(property.String);
                    break;
                case "passwordchar":
                    var passwordChar = property.String;
                    if (!string.IsNullOrEmpty(passwordChar) && passwordChar.Length>0)
                        PasswordBox.PasswordChar = property.String[0];
                    break;
                default:
                    // Add controls
                    break;
            }
        }

        private void setBackgrounds(xcBrushes brushes) {
            this.Backgrounds = brushes;
            PasswordBox.Background = Backgrounds.Normal;
        }

        private void setForegrounds(xcBrushes brushes) {
            this.Foregrounds = brushes;
            PasswordBox.Foreground = Foregrounds.Normal;
        }

    }
}
