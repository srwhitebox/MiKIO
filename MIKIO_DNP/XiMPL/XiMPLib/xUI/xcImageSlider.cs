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
using System.Windows.Media;
using XiMPLib.xUI.xStoryBoard;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace XiMPLib.xUI {
    public class xcImageSlider : Border {
        public Uri ParentUri {
            get;
            set;
        }

        private xcImageSources Sources {
            get;
            set;
        }

        private Image CurImage {
            get;
            set;
        }

        private Image NextImage {
            get;
            set;
        }

        public Stretch Stretch {
            get;
            set;
        }

        public TimeSpan Interval{
            get;
            set;
        }

        private List<Image> ImageList = new List<Image>();
        private DispatcherTimer timerImageChange;

        public xcImageSlider(xcJObject jProperties, Uri parentUri=null) {
            ParentUri = parentUri;
            this.Stretch = System.Windows.Media.Stretch.UniformToFill;
            this.Interval = new TimeSpan(0, 0, 5);
            apply(jProperties);

            Canvas canvas = new Canvas();
            canvas.Height = this.Height;
            canvas.Width = this.Width;
            this.Child = canvas;
            
            for(int i=0; i<2; i++){
                Image imgItem = new Image();
                imgItem.Height = this.Height;
                imgItem.Width = this.Width;
                imgItem.Stretch = this.Stretch;
                ImageList.Add(imgItem);
                var trGroup = new TransformGroup();
                var tr = new TranslateTransform(0, 0);
                trGroup.Children.Add(tr);
                imgItem.RenderTransform = tr;
                imgItem.RenderTransformOrigin = new Point(0.5, 0.5);
                canvas.Children.Add(imgItem);
                ImageList.Add(imgItem);
            }

            timerImageChange = new DispatcherTimer();
            timerImageChange.Interval = Interval;
            timerImageChange.Tick += new EventHandler(timerImageChange_Tick);

            this.Loaded += xcImageSlider_Loaded;
        }

        void xcImageSlider_Loaded(object sender, RoutedEventArgs e) {
            PlaySlideShow();
            timerImageChange.IsEnabled = true;
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
                case "stretch":
                    this.Stretch = property.MediaStretch;
                    break;
                case "source":
                case "sources":
                    setSources(property.String, ParentUri);
                    break;
                case "interval":
                    Interval = property.Time;
                    break;
                default:
                    // Add controls
                    break;
            }
        }

        private void setSources(string uri, Uri parentUri) {
            this.Sources = new xcImageSources(uri, ParentUri);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseUp(e);
        }

        private void timerImageChange_Tick(object sender, EventArgs e) {
            PlaySlideShow();
        }

        private int CurrentSourceIndex, CurrentCtrlIndex;

        private void PlaySlideShow() {
            try {
                if (Sources.Count == 0)
                    return;
                var oldCtrlIndex = CurrentCtrlIndex;
                CurrentCtrlIndex = (CurrentCtrlIndex + 1) % 2;
                CurrentSourceIndex = (CurrentSourceIndex + 1) % Sources.Count;

                Image imgFadeOut = ImageList[oldCtrlIndex];
                Image imgFadeIn = ImageList[CurrentCtrlIndex];
                ImageSource newSource = Sources[CurrentSourceIndex];
                
                imgFadeIn.Source = newSource;

                var sbFadeOut = new xcStoryBoard(XiMPLib.xUI.xStoryBoard.xcStoryBoard.ANIMATION_EFFECT.FADE_OUT);
                sbFadeOut.setTarget(imgFadeOut);
                sbFadeOut.setTargetProperty(Image.OpacityProperty);
                sbFadeOut.Begin();

                var sbFadeIn = new xcStoryBoard(XiMPLib.xUI.xStoryBoard.xcStoryBoard.ANIMATION_EFFECT.FADE_IN);
                sbFadeIn.setTarget(imgFadeIn);
                sbFadeIn.setTargetProperty(Image.OpacityProperty);
                sbFadeIn.Begin();
            } catch{ }
        }

        public Storyboard FadeIn {
            get;
            set;
        }

        public Storyboard FadeOut {
            get;
            set;
        }

    }
}
