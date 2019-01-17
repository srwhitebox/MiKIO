﻿using System;
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

namespace XiMPLib.xUI {
    public class xcTextView : TextBlock {
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

        public String StringFormat
        {
            get; set;
        }

        public String BindingStringFormat
        {
            get
            {
                return Binding.StringFormat;
            }
            set
            {
                Binding.StringFormat = value;
            }
        }

        private Binding Binding;

        public xcTextView(xcJObject jProperties, Uri parentUri) {
            ParentUri = parentUri;

            if (!string.IsNullOrEmpty(xcBinder.AppProperties.Language)) {
                this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(xcBinder.AppProperties.Language);
            }

            apply(jProperties);

            if (Binding != null)
                SetBinding(TextBlock.TextProperty, Binding);
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
                case "value":
                case "text":
                case "content":
                    if (property.isBindProperty) {
                        bindSource(property.BindProperty);
                    } else
                        this.Text = property.String;
                    break;
                case "stringformat":
                    StringFormat = property.String;
                    if (Name.Equals("txtEyeTestResult"))
                        break;
                    if (Binding == null)
                        Binding = new Binding();
                    Binding.StringFormat = property.String;
                    break;
                case "textwrapping":
                    this.TextWrapping = property.TextWrapping;
                    break;
                case "lineheight":
                    this.LineHeight = property.getFloat(100);
                    break;
                case "textalignment":
                    this.TextAlignment = property.TextAlignment;
                    break;
                case "datasource":
                case "datacontext":
                    this.DataContext = xcBinder.getBinder(property.String);
                    break;
                case "control":
                    xcControl control = property.Control;
                    this.Inlines.Add((System.Windows.UIElement)control.Control);
                    break;
                default:
                    // Add controls
                    break;
            }
        }

        private void bindSource(String bindProperty) {
            if (Binding == null)
                Binding = new Binding();
            try {
                Uri uri = new Uri(bindProperty);
                if (uri.Scheme.Equals("xmpl")) {
                    this.DataContext = xcBinder.getBinder(uri);
                    NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);
                    String bindingPath = queryParameters["bindingPath"];
                    if (!String.IsNullOrEmpty(bindingPath))
                        Binding.Path = new PropertyPath(queryParameters["bindingPath"]);
                }
            }
            catch {
                Binding.Path = new PropertyPath(bindProperty);
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

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseEnter(e);
            if (Backgrounds != null)
                this.Background = e.LeftButton == System.Windows.Input.MouseButtonState.Pressed ? Backgrounds.Pressed : Backgrounds.Hover;
            if (Foregrounds != null)
                this.Foreground = e.LeftButton == System.Windows.Input.MouseButtonState.Pressed ? Foregrounds.Pressed : Foregrounds.Hover;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseLeave(e);
            if (Backgrounds != null)
                this.Background = Backgrounds.Normal;
            if (Foregrounds != null)
                this.Foreground = Foregrounds.Normal;
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            if (Backgrounds != null)
                this.Background = Backgrounds.Pressed;
            if (Foregrounds != null)
                this.Foreground = Foregrounds.Pressed;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            if (Backgrounds != null)
                this.Background = Backgrounds.Normal;
            if (Foregrounds != null)
                this.Foreground = Foregrounds.Normal;

            if (ClickEventUri == null)
                return;

            string host = ClickEventUri.Host.Replace("/", "").ToLower();
            string block = ClickEventUri.Segments[1].Replace("/", "").ToLower();
            string binder = ClickEventUri.Segments[2].Replace("/", "").ToLower();
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(ClickEventUri.Query);
            switch (block) {
                case "layout":
                    switch (binder) {
                        case "visibility":
                            foreach (string key in queryParameters.Keys) {
                                Panel panel = xcBinder.PanelDictionary[key];
                                if (panel != null) {
                                    panel.Visibility = xcUiProperty.toVisibility(queryParameters[key]);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}