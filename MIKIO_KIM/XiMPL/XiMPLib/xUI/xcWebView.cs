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
using System.Runtime.InteropServices;
using System.Reflection;

namespace XiMPLib.xUI {
    public class xcWebView : Border {
        public static DependencyProperty HtmlContentProperty = DependencyProperty.Register("HtmlContent", typeof(string), typeof(xcWebView));

        private Uri ParentUri {
            get;
            set;
        }

        public WebBrowser WebBrowser {
            get;
            set;
        }

        public float Zoom {
            get;
            set;
        }

        public Uri HomeUri
        {
            get; set;
        }

        public string HtmlContent {
            get { 
                return (string)GetValue(HtmlContentProperty); 
            }
            set { 
                SetValue(HtmlContentProperty, value); 
            }
        }

        public mshtml.IHTMLDocument2 Document {
            get;
            set;
        }

        public bool ShowScrollbar {
            get;
            set;
        }

        public xcWebView(xcJObject jProperties, Uri parentUri = null) {
            ParentUri = parentUri;
                
            this.WebBrowser = new WebBrowser();
            
            this.WebBrowser.NavigateToString(" ");
            Document = WebBrowser.Document as mshtml.IHTMLDocument2;
            this.Child = this.WebBrowser;
            this.Zoom = 1;
            this.ShowScrollbar = false;
            apply(jProperties);
            this.WebBrowser.IsVisibleChanged += WebBrowser_IsVisibleChanged;

            SetSilent(true);
        }

        void WebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (!(bool)e.NewValue)
                return;
        }

        protected override void OnRender(DrawingContext dc) {
            var dipTransform = xcUiUtil.getScaleTransform(this);
            double scale = dipTransform.ScaleX;
            this.Zoom *= (float)scale;
            try {
                this.WebBrowser.Refresh();
            }
            catch {

            }
            base.OnRender(dc);
        }

        void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e) {
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (e.Property == HtmlContentProperty) {
                Document.open();
                if (!string.IsNullOrEmpty(HtmlContent)) {
                    Document.write(getHtmlContent());
                } else {
                    Document.write(" ");
                }
                Document.close();
            }            
        }

        private string getHtmlContent() {
            if (!string.IsNullOrEmpty(this.HtmlContent)) {
                string tagOffed = System.Text.RegularExpressions.Regex.Replace(this.HtmlContent, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1");
                StringBuilder sb = new StringBuilder();
                sb.Append("<html lang=\"zh-TW\"><head>");
                sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
                sb.Append(string.Format("</head><body style='zoom={0};overflow={1}'>", Zoom, ShowScrollbar ? "" : "hidden"));
                sb.Append(tagOffed);
                sb.Append("</body></html>");
                return sb.ToString();
            }
            return null;
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
                case "padding":
                    this.Padding = property.Thickness;
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
                    this.HomeUri = this.WebBrowser.Source = property.Uri;
                    break;
                case "content":
                    if (property.isBindProperty) {
                        this.SetBinding(HtmlContentProperty, property.BindProperty);
                    } else
                        this.WebBrowser.NavigateToString(property.String);
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
                    this.Zoom = property.Percent;
                    break;
                default:
                    // Add controls
                    break;
            }
        }
        
        public void command(String cmd) {
            if (string.IsNullOrEmpty(cmd))
                return;

            switch (cmd.Trim().ToLower()) {
                case "gohome":
                case "home":
                    WebBrowser.Navigate(this.HomeUri);
                    break;
                case "goback":
                case "back":
                    WebBrowser.GoBack();
                    break;
                case "goforward":
                case "forward":
                    WebBrowser.GoForward();
                    break;
                case "refresh":
                case "reload":
                    WebBrowser.Refresh();
                    break;
                default:
                    break;

            }
        }
        public void Scroll(string cmd) {
            mshtml.HTMLDocument htmlDoc = WebBrowser.Document as mshtml.HTMLDocument;
            if (htmlDoc == null)
                return;

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
                    htmlDoc.parentWindow.scrollBy(0, (int)(this.Height/this.Zoom));
                    break;
                case "pageup":
                    htmlDoc.parentWindow.scrollBy(0, (int)(-this.Height/this.Zoom));
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

        private void SetSilent(bool silent) {
            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = this.WebBrowser.Document as IOleServiceProvider;
            if (sp != null) {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null) {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
        }

    }

    [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleServiceProvider {
        [PreserveSig]
        int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
    }
}

