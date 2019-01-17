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
using WpfAnimatedGif;
using System.Windows.Input;
using XiMPLib.xType;
using System.Collections.Specialized;
using System.Web;
using XiMPLib.xBinding;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Data;

namespace XiMPLib.xUI {
    public class xcImage : Image {
        public Uri ParentUri {
            get;
            set;
        }

        private Uri ClickEventUri {
            get;
            set;
        }

        private Uri LongClickEventUri {
            get;
            set;
        }

        private Uri ActionUri {
            get;
            set;
        }

        private Uri VisibleCondition {
            get;
            set;
        }

        private xcImageSources Sources {
            get;
            set;
        }

        public static DependencyProperty IsLoggedInProperty = DependencyProperty.Register(
            "IsLoggedIn",
            typeof(bool),
            typeof(xcImage),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsLoggedInPropertyChanged))
            );

        private static void OnIsLoggedInPropertyChanged(
        DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            xcImage c = sender as xcImage;
            if (c != null) {
                //if (c.Visibility == Visibility.Visible && (bool)e.NewValue)
                    c.setVisibiity();
                //c.OnPropertyChanged(e);
            }
        }

        private DispatcherTimer MouseDownDispatcherTimer;

        public xcImage(xcJObject jProperties, Uri parentUri=null) {
            ParentUri = parentUri;
            apply(jProperties);
            this.IsVisibleChanged += XcImage_IsVisibleChanged;
            MouseDownDispatcherTimer = new DispatcherTimer();
            MouseDownDispatcherTimer.Tick += MouseDownDispatcherTimer_Tick;
            MouseDownDispatcherTimer.Interval = new TimeSpan(0, 0, 3);
        }

        private void XcImage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.NewValue) {
                if (VisibleCondition != null) {
                    setVisibiity();
                    //Console.WriteLine("Visibility Change : " + this.Name + " " + PrevVisibilityChangedTime + " - " + e.OldValue + " > " + e.NewValue);
                }
            }
        }

        public void setVisibiity() {
            if (VisibleCondition == null)
                return;
            if (VisibleCondition.Scheme.Equals("xmpl")) {
                bool hasCondtion = xcBinder.HasCondition(VisibleCondition);
                if (hasCondtion) {
                    this.Visibility = System.Windows.Visibility.Visible;
                }
                else {
                    this.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        private void MouseDownDispatcherTimer_Tick(object sender, EventArgs e) {
            MouseDownDispatcherTimer.Stop();
            OnMouseLongClick();
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
                    if (property.isBindProperty)
                        setBinding(xcBorder.VisibilityProperty, property.BindProperty, new BooleanToVisibilityConverter());
                    else
                        this.Visibility = property.Visibility;
                    break;
                case "enabled":
                case "isenabled":
                    if (property.isBindProperty)
                        setBinding(xcBorder.IsEnabledProperty, property.BindProperty);
                    else
                        this.IsEnabled = property.Boolean;
                    break;
                case "flowdirection":
                    this.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "transformorigin":
                    this.RenderTransformOrigin = property.Point;
                    break;
                case "stretch":
                    this.Stretch = property.MediaStretch;
                    break;
                case "source":
                    if (property.String.IndexOf("|") < 0) {
                        Uri absUri = ParentUri == null ? property.Uri : new System.Uri(ParentUri, property.Uri);
                        if (this.Name == "imgEyeTestLandolf")
                        {
                            if (xcBinder.AppProperties.VisionTestTypeface.ToLower().StartsWith("e"))
                                absUri = new System.Uri(ParentUri, new Uri("image/ic_typeface_e.png", UriKind.RelativeOrAbsolute));
                            else
                                absUri = new System.Uri(ParentUri, new Uri("image/ic_typeface_c.png", UriKind.RelativeOrAbsolute));
                        }
                        var Image = new BitmapImage(absUri);
                        if (property.String.ToLower().EndsWith("gif")) {
                            ImageBehavior.SetAnimatedSource(this, Image);
                        } else {
                            this.Source = new BitmapImage(absUri); ;
                        }
                    } else {
                        setSources(property.String, ParentUri);
                    }
                    break;
                case "sources":
                    setSources(property.String, ParentUri);
                    break;
                case "visiblecondition":
                    var visbileConditionuri = property.Uri;
                    this.VisibleCondition = visbileConditionuri;
                    this.DataContext = xcBinder.getBinder(visbileConditionuri);
                    var bindingPropertyName = visbileConditionuri.Segments[visbileConditionuri.Segments.Length - 1];
                    switch (bindingPropertyName) {
                        case "IsLoggedIn":
                            this.SetBinding(IsLoggedInProperty, bindingPropertyName);
                            break;
                    }
                    break;

                case "click":
                    ClickEventUri = property.Uri;
                    break;
                case "longclick":
                    LongClickEventUri = property.Uri;
                    break;
                case "action":
                    this.ActionUri = property.Uri;
                    break;
                default:
                    // Add controls
                    break;
            }
        }

        private Boolean setBinding(DependencyProperty dp, String bindProperty, IValueConverter converter = null) {
            try {
                Binding binding = new Binding();
                Uri uri = new Uri(bindProperty);
                if (uri.Scheme.Equals("xmpl")) {
                    binding.Source = xcBinder.getBinder(uri);
                    NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);
                    String bindingPath = queryParameters["bindingPath"];
                    if (!String.IsNullOrEmpty(bindingPath)) {
                        binding.Path = new PropertyPath(queryParameters["bindingPath"]);
                        if (converter != null)
                            binding.Converter = converter;
                        this.SetBinding(dp, binding);
                        return true;
                    }
                }
            }
            catch {
            }
            return false;
        }

        private void setSources(string uri, Uri parentUri) {
            this.Sources = new xcImageSources(uri, ParentUri);
            this.Source = Sources.Normal;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseEnter(e);
            if (e.LeftButton == MouseButtonState.Pressed) {
                MouseDownDispatcherTimer.Start();
            }
            if (Sources != null) {
                this.Source = e.LeftButton == System.Windows.Input.MouseButtonState.Pressed ? Sources.Pressed : Sources.Hover;
                if (Sources.isGif(this.Source)) {
                    ImageBehavior.SetAnimatedSource(this, Source);
                }
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseLeave(e);
            if (e.LeftButton == MouseButtonState.Pressed) {
                MouseDownDispatcherTimer.Stop();
            }
            if (Sources != null) {
                this.Source = Sources.Normal;
                if (Sources.isGif(this.Source)) {
                    ImageBehavior.SetAnimatedSource(this, Source);
                }
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed) {
                MouseDownDispatcherTimer.Start();
            }
            if (Sources != null) {
                this.Source = Sources.Pressed;
                if (Sources.isGif(this.Source)) {
                    ImageBehavior.SetAnimatedSource(this, Source);
                }
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseUp(e);

            if (e.ButtonState == MouseButtonState.Released) {
                MouseDownDispatcherTimer.Stop();
            }

            if (e.ChangedButton == MouseButton.Right && e.ButtonState == MouseButtonState.Released) {
                OnMouseLongClick();
                return;
            }


            if (Sources != null) {
                this.Source = Sources.Normal;
                if (Sources.isGif(this.Source)) {
                    ImageBehavior.SetAnimatedSource(this, Source);
                }
            }
            
            if (ClickEventUri == null)
                return;

            SetCursorPos(0, 0);

            xcBinder.doAction(ClickEventUri);
        }

        private void OnMouseLongClick() {
            if (LongClickEventUri == null)
                return;

            xcBinder.doAction(LongClickEventUri);
        }
    }
}
