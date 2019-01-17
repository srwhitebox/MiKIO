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
using XiMPLib.MiPHS;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.IO;
using System.Windows.Markup;
using XiMPLib.MiHIS;
using XiMPLib.MiHealth;

namespace XiMPLib.xUI {
    public class xcDataGrid : DataGrid {
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

        private Uri ItemClickEventUri {
            get;
            set;
        }

        public Predicate<object> Filter {
            get;
            set;
        }

        public xcDataGrid(xcJObject jProperties, Uri parentUri) {
            ParentUri = parentUri;
            this.HeadersVisibility = DataGridHeadersVisibility.Column;
            this.GridLinesVisibility = DataGridGridLinesVisibility.None;
            this.SelectionUnit = DataGridSelectionUnit.FullRow;
            this.CanUserAddRows = false;
            //this.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            
            // setStyles();
            apply(jProperties);
            
            this.CanSelectMultipleItems = false;
            this.IsVisibleChanged += xcDataGrid_IsVisibleChanged;
            this.SelectionChanged += xcDataGrid_SelectionChanged;
            this.PreviewMouseLeftButtonDown += xcDataGrid_PreviewMouseLeftButtonDown;

        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            foreach (var column in this.Columns) {
                if (column.SortDirection != null) {
                    this.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(column.SortMemberPath, column.SortDirection.Value));
                }
            }
            base.OnItemsChanged(e);
            
        }

        void xcDataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.SelectedIndex = -1;
        }

        void xcDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0) {
                var item = e.AddedItems[0];
                if (ItemClickEventUri != null) {
                    xcBinder.doAction(ItemClickEventUri);
                }
                if (item.GetType().Equals(typeof(PhsAnnouncementRecord))) {
                    xcBinder.AnnouncementRecord.copyFrom((PhsAnnouncementRecord)item);
                } else if (item.GetType().Equals(typeof(RegInfo))) {
                    xcBinder.RegInfo.copyFrom((RegInfo)item);
                    xcBinder.RegInfo.notifyChanged();
                } else if (item.GetType().Equals(typeof(OpdSchedule))) {
                    xcBinder.OpdSchedule.copyFrom((OpdSchedule)item);
                } else if (item.GetType().Equals(typeof(MiHealthCareInfo))) {
                    xcBinder.MiHealthCareInfo.copyFrom((MiHealthCareInfo)item);
                }
            } else
                xcBinder.AnnouncementRecord.clear();
        }

        void xcDataGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if ((bool)e.NewValue) {
                var cv = CollectionViewSource.GetDefaultView(this.ItemsSource);
                if (cv == null)
                    return;
                cv.Filter = this.Filter != null ? this.Filter : null;
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
                case "selectionmode":
                    this.SelectionMode = property.GridSelectionMode;
                    break;
                case "selectionunit":
                    this.SelectionUnit = DataGridSelectionUnit.FullRow;
                    break;
                case "headerheight":
                    this.ColumnHeaderHeight = property.Float;
                    break;
                case "rowheight":
                    this.RowHeight = property.Float;
                    break;
                case "datasource":
                    var sourceUri = property.Uri;
                    var list = xcBinder.getBinder(sourceUri);
                    if (list != null) {
                        if (list.GetType().Equals(typeof(RegInfoList))) {
                            var regionInfoList = (RegInfoList)list;
                            regionInfoList.addBoundedList(this);
                            this.ItemsSource = regionInfoList;
                            NameValueCollection queryParameters = HttpUtility.ParseQueryString(sourceUri.Query);
                            if (queryParameters.Count > 0) {
                                if (queryParameters["OpdDate"] != null) {
                                    DateTime opdDate = xcJProperty.getDate(queryParameters["OpdDate"]);
                                    Filter = new Predicate<object>(item => ((RegInfo)item).OpdDate.Equals(opdDate));
                                }
                            }
                        }
                        else if (list.GetType().Equals(typeof(OpdScheduleList))) {
                            var opdScheduleList = (OpdScheduleList)list;
                            opdScheduleList.addBoundedList(this);
                            this.ItemsSource = opdScheduleList;
                        }
                        else if (list.GetType().Equals(typeof(PhsAnnouncementList)))
                            this.ItemsSource = (PhsAnnouncementList)list;
                        else if (list.GetType().Equals(typeof(List<MiHealthCareInfo>))) {
                            xcBinder.MiHealth.addBoundedList(this);
                            this.ItemsSource = (List<MiHealthCareInfo>)list;
                        }
                    }
                    break;
                case "autocolumns":
                    this.AutoGenerateColumns = property.Boolean;
                    break;
                case "textcolumn":
                    Columns.Add(property.TextColumn);
                    break;
                case "buttoncolumn":
                    Columns.Add(property.ButtonColumn);
                    break;
                case "verticalscrollbarvisibility":
                    this.VerticalScrollBarVisibility = property.ScrollBarVisiblity;
                    break;
                case "horizontalscrollbarvisibility":
                    this.HorizontalScrollBarVisibility = property.ScrollBarVisiblity;
                    break;
                case "itemclick":
                    ItemClickEventUri = property.Uri;
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

        private void setForegrounds(xcBrushes brushes) {
            this.Foregrounds = brushes;
            this.Foreground = Foregrounds.Normal;
        }

        private void setStyles() {
            Style scrollBarStyle = new Style(typeof(ScrollBar));
            scrollBarStyle.Setters.Add(new Setter(ScrollBar.SnapsToDevicePixelsProperty, true));
            scrollBarStyle.Setters.Add(new Setter(ScrollBar.OverridesDefaultStyleProperty, true));
            
            Trigger verticalScrollBarTrigger = new Trigger();
            verticalScrollBarTrigger.Property = ScrollBar.OrientationProperty;
            verticalScrollBarTrigger.Value = Orientation.Vertical;
            verticalScrollBarTrigger.Setters.Add(new Setter(ScrollBar.WidthProperty, 40.0));
            verticalScrollBarTrigger.Setters.Add(new Setter(ScrollBar.HeightProperty, Double.NaN));

            string xaml = "<ControlTemplate x:Key=\"VerticalScrollBar\" TargetType=\"{x:Type ScrollBar}\"> <Grid> <Grid.RowDefinitions> <RowDefinition MaxHeight=\"40\"/> <RowDefinition Height=\"*\"/> <RowDefinition MaxHeight=\"40\"/> </Grid.RowDefinitions> <Border Grid.RowSpan=\"3\" Background=\"Transparent\" BorderBrush=\"Transparent\" /> <RepeatButton Grid.Row=\"0\"  Height=\"40\" Command=\"ScrollBar.LineUpCommand\" Content=\"M 0 4 L 8 4 L 4 0 Z\" /> <Track Name=\"PART_Track\" Grid.Row=\"1\" IsDirectionReversed=\"true\"> <Track.DecreaseRepeatButton> <RepeatButton Margin=\"9,2,9,2\" Command=\"ScrollBar.PageUpCommand\" /> </Track.DecreaseRepeatButton> <Track.Thumb> <Thumb Margin=\"6,1,6,1\"/> </Track.Thumb> <Track.IncreaseRepeatButton> <RepeatButton Margin=\"9,2,9,2\" Command=\"ScrollBar.PageDownCommand\" /> </Track.IncreaseRepeatButton> </Track> <RepeatButton Grid.Row=\"3\" Height=\"40\" Command=\"ScrollBar.LineDownCommand\" Content=\"M 0 0 L 4 4 L 8 0 Z\"/> </Grid>  </ControlTemplate>";

            ControlTemplate template = parseXAML(xaml);
            
            verticalScrollBarTrigger.Setters.Add(new Setter(ScrollBar.TemplateProperty, template));
            scrollBarStyle.Triggers.Add(verticalScrollBarTrigger);

            Trigger horizontalScrollBarTrigger = new Trigger();
            horizontalScrollBarTrigger.Property = ScrollBar.OrientationProperty;
            horizontalScrollBarTrigger.Value = Orientation.Horizontal;
            horizontalScrollBarTrigger.Setters.Add(new Setter(ScrollBar.WidthProperty, Double.NaN));
            horizontalScrollBarTrigger.Setters.Add(new Setter(ScrollBar.HeightProperty, 40.0));
            scrollBarStyle.Triggers.Add(horizontalScrollBarTrigger);

            this.Resources.Add(typeof(ScrollBar), scrollBarStyle);
        }

        private ControlTemplate parseXAML(string xaml) {
            MemoryStream sr = new MemoryStream(Encoding.ASCII.GetBytes(xaml));
            ParserContext pc = new ParserContext();
            pc.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            pc.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            return (ControlTemplate)XamlReader.Load(sr, pc);
        }

        public void Scroll(string cmd) {
            var scrollViewer = GetScrollViewer();
            if (scrollViewer == null)
                return;
            switch (cmd.ToLower()) {
                case "lineup":
                    scrollViewer.LineUp();
                    break;
                case "linedown":
                    scrollViewer.LineDown();
                    break;
                case "lineleft":
                    scrollViewer.LineLeft();
                    break;
                case "lineRight":
                    scrollViewer.LineRight();
                    break;
                case "pagedown":
                    scrollViewer.PageDown();
                    break;
                case "pageup":
                    scrollViewer.PageUp();
                    break;
                case "pageleft":
                    scrollViewer.PageLeft();
                    break;
                case "pageright":
                    scrollViewer.PageRight();
                    break;
                case "top":
                    scrollViewer.ScrollToTop();
                    break;
                case "bottom":
                    scrollViewer.ScrollToBottom();
                    break;
                case "home":
                    scrollViewer.ScrollToHome();
                    break;
                case "end":
                    scrollViewer.ScrollToEnd();
                    break;
                case "leftend":
                    scrollViewer.ScrollToLeftEnd();
                    break;
                case "rightend":
                    scrollViewer.ScrollToLeftEnd();
                    break;
            }
        }

        private ScrollViewer GetScrollViewer() {
            if (VisualTreeHelper.GetChildrenCount(this) == 0) return null;
            var x = VisualTreeHelper.GetChild(this, 0);
            if (x == null) return null;
            if (VisualTreeHelper.GetChildrenCount(x) == 0) return null;
            return VisualTreeHelper.GetChild(x, 0) as ScrollViewer;
        }
    }
}
