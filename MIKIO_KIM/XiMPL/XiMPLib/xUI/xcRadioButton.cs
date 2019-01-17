using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using XiMPLib.xBinding;
using XiMPLib.xDocument;
using XiMPLib.xType;

namespace XiMPLib.xUI {
    public class xcRadioButton : RadioButton {
        public Uri ParentUri {
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

        private Binding ContentBinding;
        private Binding CheckedBinding;

        public xcRadioButton(xcJObject jProperties, Uri parentUri) {
            ParentUri = parentUri;

            // setStyles();
            apply(jProperties);

            this.Checked += XcCheckBox_Checked;
            if (ContentBinding != null) {
                SetBinding(CheckBox.ContentProperty, ContentBinding);
                SetBinding(CheckBox.IsCheckedProperty, CheckedBinding);
            }
        }

        private void XcCheckBox_Checked(object sender, RoutedEventArgs e) {
            if (CheckedBinding != null) {
                xcBinder.Patient.notifyChanged();
            }
        }

        private void apply(xcJObject jproperties) {
            foreach (var property in jproperties.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    foreach (JToken childProperty in property.Value) {
                        apply(new xcJProperty(new JProperty(property.Name, childProperty), ParentUri));
                    }
                }
                else {
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
                case "horizontalcontentalignment":
                case "textalignment":
                    this.HorizontalContentAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    this.VerticalAlignment = property.VerticalAlignment;
                    break;
                case "verticalcontentalignment":
                    this.VerticalContentAlignment = property.VerticalAlignment;
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
                case "visibility":
                    this.Visibility = property.Visibility;
                    break;
                case "enabled":
                case "isenabled":
                    this.IsEnabled = property.Boolean;
                    break;
                case "background":
                    if (property.String.IndexOf("|") < 0)
                        this.Background = property.Brush;
                    else
                        setBackgrounds(property.Brushes);
                    break;
                case "backgrounds":
                    setBackgrounds(property.Brushes);
                    break;
                case "foreground":
                    if (property.String.IndexOf("|") < 0)
                        this.Foreground = property.Brush;
                    else
                        setForegrounds(property.Brushes);
                    break;
                case "foregrounds":
                    setForegrounds(property.Brushes);
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
                case "content":
                case "text":
                    if (property.isBindProperty) {
                        if (ContentBinding == null)
                            ContentBinding = new Binding();
                        ContentBinding.Path = new PropertyPath(property.BindProperty);
                        // this.SetBinding(TextProperty, property.BindProperty);
                    }
                    else
                        this.Content = property.String;
                    break;
                case "stringformat":
                    if (ContentBinding == null)
                        ContentBinding = new Binding();
                    ContentBinding.StringFormat = property.String;
                    break;
                case "ischecked":
                    if (property.isBindProperty) {
                        if (CheckedBinding == null)
                            CheckedBinding = new Binding();
                        CheckedBinding.Path = new PropertyPath(property.BindProperty);
                        // this.SetBinding(TextProperty, property.BindProperty);
                    } else
                        this.IsChecked = property.Boolean;
                    break;
                case "datasource":
                case "datacontext":
                    this.DataContext = xcBinder.getBinder(property.String);
                    break;
                case "itemsource":
                    
                    break;
                default:
                    break;
            }
        }

        private void setBackgrounds(xcBrushes brushes) {
            this.Backgrounds = brushes;
            this.Background = Backgrounds.Normal;
        }

        private void setForegrounds(xcBrushes brushes) {
            this.Foregrounds = brushes;
            this.Foreground = Foregrounds.Normal;
        }

    }
}
