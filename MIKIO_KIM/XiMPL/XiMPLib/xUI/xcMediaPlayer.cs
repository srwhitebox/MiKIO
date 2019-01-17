using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media;
using XiMPLib.xDocument;
using XiMPLib.xType;
using XiMPLib.xBinding;

namespace XiMPLib.xUI {
    public class xcMediaPlayer : MediaElement{
        public Uri ParentUri {
            get;
            set;
        }

        public TimeSpan StartTime {
            get;
            set;
        }

        public TimeSpan EndTime {
            get;
            set;
        }

        public List<xcPlayItem> PlayList {
            get;
            set;
        }

        public bool IsVideo {
            get {
                return this.NaturalDuration.HasTimeSpan && this.NaturalVideoHeight > 0;
            }
        }

        public Uri SourceUri {
            get;
            set;
        }
        public xcPlayItem CurMedia {
            get;
            set;
        }

        private AppProperties AppProperties {
            get {
                return xcBinder.AppProperties;
            }
        }

        private bool IsPlaying = false;

        public xcMediaPlayer(xcJObject jProperties, Uri parentUri = null) {
            ParentUri = parentUri;
            
            setDefault();
            apply(jProperties);
            this.IsVisibleChanged += XcMediaPlayer_IsVisibleChanged;
            if (this.SourceUri!=null && this.SourceUri.Scheme.Equals("xmpl")) {
                switch (this.SourceUri.Segments[1].Replace("/", "").ToLower()) {
                    case "app":
                        switch (this.SourceUri.Segments[2].Replace("/", "").ToLower()) {
                            case "properties":
                                switch (this.SourceUri.Segments[3].Replace("/", "").ToLower()) {
                                    case "advlist":
                                        PlayList = AppProperties.AdvList;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            } else if (this.SourceUri != null){
                PlayList = new List<xcPlayItem>();
                PlayList.Add(new xcPlayItem(0, this.SourceUri.AbsoluteUri, StartTime, EndTime));
            }

            if (PlayList != null && PlayList.Count > 0) {
                CurMedia = PlayList[0];
                this.LoadedBehavior = MediaState.Manual;
                this.Stop();
                this.Source = CurMedia.Uri;
                this.MediaOpened += new RoutedEventHandler(media_MediaOpened);
            }
        }

        private void XcMediaPlayer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            bool isVisible = (bool)e.NewValue;
            if (isVisible && PlayList.Count > 0)
                this.Play();
            else
                this.Pause();
        }

        private void setDefault() {
            this.Stretch = System.Windows.Media.Stretch.UniformToFill;
            this.CacheMode = new BitmapCache(1.0);
            this.VisualCacheMode = new BitmapCache(1.0);
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e) {
            this.LoadedBehavior = MediaState.Manual;
            this.UnloadedBehavior = MediaState.Manual;
            startPlay();
        }

        private void startPlay() {
            if (IsPlaying)
                return;
            
            if (this.IsVideo) {
                this.Position = StartTime = CurMedia.StartTime;
                EndTime = CurMedia.EndTime;
                if (EndTime.Equals(new TimeSpan(0))) {
                    EndTime = this.NaturalDuration.TimeSpan;
                }
            } else {
                StartTime = new TimeSpan(0);
                EndTime = new TimeSpan(0, 0, 0, 0, CurMedia.Duration);
            }
            Timer Timer = new Timer();
            if (EndTime.Equals(new TimeSpan(0))) {
                Timer.Interval = 2000;
                Timer.Tick +=Timer_Tick_Again;
            } else {
                Timer.Interval = Convert.ToInt32((EndTime - StartTime).TotalMilliseconds);
                Timer.Tick += Timer_Tick;                
                this.Play();
                IsPlaying = true;
            }
            Timer.Start();
        }


        private void Timer_Tick_Again(object sender, EventArgs e) {
            this.InvalidateArrange();
            //startPlay();
        }

        void Timer_Tick(object sender, EventArgs e) {
            this.Stop();
            ((Timer)sender).Stop();

            IsPlaying = false;
            int prevIndex = PlayList.IndexOf(CurMedia);
            int index = prevIndex +1;
            if (index == PlayList.Count)
                index = 0;
            if (index != prevIndex) {
                CurMedia = PlayList[index];
                Source = CurMedia.Uri;
            } else {
                startPlay();
            }
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

        private void apply(xcJProperty property) {
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
                case "margin":
                    this.Margin = property.Thickness;
                    break;
                case "cursor":
                    Cursor = property.Cursor;
                    break;
                case "opacity":
                    this.Opacity = property.getPercent(100);
                    break;
                case "visibility":
                    this.Visibility = property.Visibility;
                    break;
                case "enabled":
                case "isenabled":
                    this.IsEnabled = property.Boolean;
                    break;
                case "transform":
                    this.RenderTransform = property.Transform;
                    break;
                case "stretch":
                    this.Stretch = property.MediaStretch;
                    break;
                case "effect":
                    this.Effect = property.Effect;
                    break;
                case "source":
                    SourceUri = ParentUri == null ? property.Uri : new System.Uri(ParentUri, property.Uri);                   
                    break;
                case "starttime":
                    this.StartTime = property.Time;
                    break;
                case "endtime":
                    this.EndTime = property.Time;
                    break;
                case "isMuted":
                    this.IsMuted = property.Boolean;
                    break;
                case "volume":
                    this.Volume = property.getPercent(50);
                    break;
                default:
                    break;
            }
        }
    }
}
