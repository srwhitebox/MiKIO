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
using XiMPLib.xBinding;
using System.Windows.Media;
using Awesomium.Windows.Controls;
using System.Web;
using Awesomium.Core;

namespace XiMPLib.xUI {
    public class xcWebView2 : WebControl {
        public static DependencyProperty HtmlContentProperty = DependencyProperty.Register("HtmlContent", typeof(string), typeof(xcWebView));

        private Uri ParentUri {
            get;
            set;
        }

        public string HtmlContent {
            get { 
                return (string)GetValue(HtmlContentProperty); 
            }
            set { 
                SetValue(HtmlContentProperty, value); 
            }
        }

        public int ZoomRate {
            get;
            set;
        }

        public bool ShowScrollbar {
            get;
            set;
        }

        public int ClientHeight {
            get {
                return (int)this.ExecuteJavascriptWithResult("document.documentElement.clientHeight");
            }
        }

        public int ScrollHeight {
            get {
                return (int)this.ExecuteJavascriptWithResult("document.documentElement.scrollHeight");
            }
        }

        public int InnerHeight {
            get {
                return (int)this.ExecuteJavascriptWithResult("window.innerHeight");
            }
        }

        public int PageYOffset {
            get {
                return (int)this.ExecuteJavascriptWithResult("window.pageYOffset");
            }
        }

        public xcWebView2(xcJObject jProperties, Uri parentUri = null) {
            ParentUri = parentUri;
                
            //this.Child = this.WebBrowser;
            this.ShowScrollbar = false;
            apply(jProperties);
            this.InitializeView += xcWebView2_InitializeView;
            this.ShowContextMenu += xcWebView2_ShowContextMenu;
            this.NativeViewInitialized += xcWebView2_NativeViewInitialized;
            this.IsVisibleChanged += xcWebView2_IsVisibleChanged;
        }

        void xcWebView2_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            
            if (!(bool)e.NewValue) {
                xcBinder.AnnouncementRecord.clear();
            }
        }

        void xcWebView2_InitializeView(object sender, WebViewEventArgs e) {
            var dipTransform = xcUiUtil.getScaleTransform(this);
            ZoomRate = (int)(ZoomRate * (1 / dipTransform.ScaleX));
            this.WebSession = WebCore.CreateWebSession(new WebPreferences() { CustomCSS = string.Format("body {{ zoom:{0}% ; overflow:{1};}}", ZoomRate, ShowScrollbar ? "" : "hidden") });
        }

        void xcWebView2_ShowContextMenu(object sender, Awesomium.Core.ContextMenuEventArgs e) {
            e.Handled = true;
        }

        void xcWebView2_NativeViewInitialized(object sender, Awesomium.Core.WebViewEventArgs e) {
            this.LoadHTML(getHtmlContent());
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (e.Property == HtmlContentProperty) {
                this.LoadHTML(getHtmlContent());
             }
        }

        private string getHtmlContent() {
            
            string tagOffed = string.IsNullOrEmpty(this.HtmlContent) ? "" : System.Text.RegularExpressions.Regex.Replace(this.HtmlContent, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1");
            if (string.IsNullOrEmpty(tagOffed)){
                return "<html/>";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<html lang=\"zh-TW\"><head>");
            sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
            sb.Append("</head><body>");
            sb.Append(tagOffed);
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private void apply(xcJObject jproperties) {
            foreach (var property in jproperties.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    foreach (JToken childProperty in property.Value) {
                        apply(new xcJProperty(new JProperty(property.Name, childProperty)));
                    }
                } else {
                    // process properites
                    apply(new xcJProperty(property));
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
                case "background":
                    this.Background = property.Brush;
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
                case "flowdirection":
                    this.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "source":
                    this.Source = property.Uri;
                    break;
                case "content":
                    if (property.isBindProperty) {
                        this.SetBinding(HtmlContentProperty, property.BindProperty);
                    } else
                        this.Source = property.Uri;
                    break;
                case "datasource":
                    this.DataContext = xcBinder.getBinder(property.String);
                    break;
                case "showscrollbar":
                    ShowScrollbar = property.Boolean;
                    break;
                case "hidescrollbar":
                    ShowScrollbar = !property.Boolean;
                    break;
                case "zoom":
                    this.ZoomRate = (int)(property.Percent * 100);
                    break;
                default:
                    // Add controls
                    break;
            }
        }

        public void Scroll(string cmd) {
            int yOffset = PageYOffset;
            switch (cmd.ToLower()) {
                case "lineup":
                    break;
                case "linedown":
                    break;
                case "lineleft":
                    break;
                case "lineRight":
                    break;
                case "pagedown":
                    if (yOffset < ScrollHeight)
                        yOffset += InnerHeight;
                    else
                        yOffset = ScrollHeight - InnerHeight;
                    this.ExecuteJavascript(string.Format("window.scrollTo(0, {0})", yOffset));
                    break;
                case "pageup":
                    if (yOffset > InnerHeight)
                        yOffset -= InnerHeight;
                    else
                        yOffset = 0;
                    this.ExecuteJavascript(string.Format("window.scrollTo(0, {0})", yOffset));
                    break;
                case "pageleft":
                    break;
                case "pageright":
                    break;
                case "top":
                    break;
                case "bottom":
                    break;
                case "home":
                    break;
                case "end":
                    break;
                case "leftend":
                    break;
                case "rightend":
                    break;
            }
        }
    }
}
