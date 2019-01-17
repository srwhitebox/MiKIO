using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XiMPLib.xDocument;
using Newtonsoft.Json.Linq;
using XiMPLib.xType;
using System.Windows;
using XiMPLib.xBinding;
using System.Collections.Specialized;
using System.Web;
using System.Timers;
using System.Threading;

namespace XiMPLib.xUI {
    public class xcStackPanel : StackPanel{
        public Uri ParentUri {
            get;
            set;
        }

        public xcBrushes Backgrounds {
            get;
            set;
        }

        public xcBrushes Foregrounds {
            get;
            set;
        }

        private Uri VisibleCondition
        {
            get;
            set;
        }

        private double VisibleTime
        {
            get;
            set;
        }

        private Uri VisibleTimeoutEventUri
        {
            get;
            set;
        }

        private Uri OnShowEventUri
        {
            get;
            set;
        }

        private Uri OnHideEventUri
        {
            get;
            set;
        }

        public static DependencyProperty HasCardProperty = DependencyProperty.Register(
            "HasCard",
            typeof(bool),
            typeof(xcStackPanel),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasCardPropertyChanged))
            );

        public static DependencyProperty IsLoggedInProperty = DependencyProperty.Register(
            "IsLoggedIn",
            typeof(bool),
            typeof(xcStackPanel),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsLoggedInPropertyChanged))
            );

        private static void OnHasCardPropertyChanged(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            xcStackPanel c = sender as xcStackPanel;
            if (c != null)
            {
                if (c.Visibility == Visibility.Visible && (bool)e.NewValue)
                    c.setVisibiity();
                //c.OnPropertyChanged(e);
            }
        }

        private static void OnIsLoggedInPropertyChanged(
        DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            xcStackPanel c = sender as xcStackPanel;
            if (c != null)
            {
                if (c.Visibility == Visibility.Visible && (bool)e.NewValue)
                    c.setVisibiity();
                //c.OnPropertyChanged(e);
            }
        }

        public bool CanChangeVisiblity
        {
            get
            {
                return PrevVisibilityChangedTime < DateTime.Now.Ticks;
            }
        }

        private long PrevVisibilityChangedTime
        {
            get;
            set;
        }

        private System.Timers.Timer VisibleTimer = null;


        public xcStackPanel(xcJObject jProperties, Uri parentUri=null) {
            this.ParentUri = parentUri;
            apply(jProperties);
            this.IsVisibleChanged += xcStackPanel_IsVisibleChanged;
            this.Focusable = false;
            VisibleTimer = new System.Timers.Timer();
            VisibleTimer.AutoReset = false;
            VisibleTimer.Elapsed += timer_Elapsed;
        }

        private void xcStackPanel_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            PrevVisibilityChangedTime = DateTime.Now.Ticks + 1;
            if (xcBinder.BpRecord != null)
                xcBinder.BpRecord.notifyChanged();
            if ((bool)e.NewValue)
            {
                if (VisibleCondition != null)
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    setVisibiity();
                    //Console.WriteLine("Visibility Change : " + this.Name + " " + PrevVisibilityChangedTime + " - " + e.OldValue + " > " + e.NewValue);
                }
                if (VisibleTime > 0 && VisibleTimeoutEventUri != null)
                {
                    VisibleTimer.Interval = VisibleTime;
                    if (this.Visibility == System.Windows.Visibility.Visible)
                        VisibleTimer.Start();
                }
                if (OnShowEventUri != null)
                {
                    xcBinder.doAction(OnShowEventUri);
                }
            }
            else
            {
                if (VisibleTime > 0)
                {
                    VisibleTimer.Stop();
                    //Console.WriteLine("Visibility Change : " + this.Name + " " + PrevVisibilityChangedTime + " - " + e.OldValue + " > " + e.NewValue);
                }

                if (OnHideEventUri != null)
                {
                    xcBinder.doAction(OnHideEventUri);
                }
            }

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;
            timer.Stop();
            xcBinder.doAction(VisibleTimeoutEventUri);
        }

        public void setVisibiity()
        {
            if (VisibleCondition == null)
                return;
            if (VisibleCondition.Scheme.Equals("xmpl") && VisibleCondition.Segments.Length >= 4)
            {
                bool hasCondtion = xcBinder.HasCondition(VisibleCondition);

                NameValueCollection queryParameters = HttpUtility.ParseQueryString(VisibleCondition.Query);
                string layoutName = queryParameters["show"];
                Panel panel = xcBinder.PanelDictionary[layoutName];
                if (hasCondtion)
                {
                    this.Visibility = System.Windows.Visibility.Collapsed;
                    if (panel != null)
                    {
                        panel.Visibility = System.Windows.Visibility.Visible;
                        //setConditionalVisible(panel, 1);
                    }
                }
                else
                {
                    if (panel != null)
                    {
                        panel.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    this.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public async void setConditionalVisible(Panel panel, int timeout)
        {
            await Task.Delay(timeout);
            panel.Visibility = System.Windows.Visibility.Visible;
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
                case "orientation":
                    Orientation = property.Orientation;
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
                    this.Opacity = property.Float;
                    break;
                case "visibility":
                    this.Visibility = property.Visibility;
                    break;
                case "visiblecondition":
                    var visbileConditionuri = property.Uri;
                    this.VisibleCondition = visbileConditionuri;
                    this.DataContext = xcBinder.getBinder(visbileConditionuri);
                    var bindingPropertyName = visbileConditionuri.Segments[visbileConditionuri.Segments.Length - 1];
                    switch (bindingPropertyName)
                    {
                        case "HasCard":
                            this.SetBinding(HasCardProperty, bindingPropertyName);
                            break;
                        case "IsLoggedIn":
                            this.SetBinding(IsLoggedInProperty, bindingPropertyName);
                            break;
                    }
                    break;
                case "enabled":
                case "isenabled":
                    this.IsEnabled = property.Boolean;
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
                case "hscroll":
                case "horizontalscroll":
                case "canhorizontallyscroll":
                    CanHorizontallyScroll = property.Boolean;
                    break;
                case "vscroll":
                case "verticalscroll":
                case "canverticallyscroll":
                    CanVerticallyScroll = property.Boolean;
                    break;
                case "flowdirection":
                    FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "layout":
                    xcLayout layout = property.Layout;
                    if (layout.Panel != null)
                        this.Children.Add((System.Windows.UIElement)layout.Panel);
                    break;
                case "control":
                    xcControl control = property.Control;
                    this.Children.Add((System.Windows.UIElement)control.Control);
                    break;
                case "effect":
                    this.Effect = property.Effect;
                    break;
                case "visibletime":
                    this.VisibleTime = property.Time.TotalMilliseconds;
                    break;
                case "visibletimeout":
                    this.VisibleTimeoutEventUri = property.Uri;
                    break;
                case "onshow":
                    this.OnShowEventUri = property.Uri;
                    break;
                case "onhide":
                    this.OnHideEventUri = property.Uri;
                    break;
                default:
                    // Add controls
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
        }

    }
}
