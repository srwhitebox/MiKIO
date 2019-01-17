using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

using System.Windows.Input;
using Newtonsoft.Json.Linq;

using XiMPLib.xDocument;
using XiMPLib.xShell;
using System.Windows.Controls;
using XiMPLib.xBinding;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;

namespace XiMPLib.xUI {
    public class xcWindow : Window {
        public static ScaleTransform DpiTransform {
            get;
            set;
        }

        public xcWindow(Uri uri){
            apply(new XiMPLib.XiMPL.xcXimplObject(uri));

            RoutedCommand cmdSimulateNhiCard = new RoutedCommand();
            cmdSimulateNhiCard.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift));
            this.CommandBindings.Add(new CommandBinding(cmdSimulateNhiCard, simulateCard));

            RoutedCommand cmdSimulateGl150 = new RoutedCommand();
            cmdSimulateGl150.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control | ModifierKeys.Shift));
            this.CommandBindings.Add(new CommandBinding(cmdSimulateGl150, simulateGl150));

            RoutedCommand cmdSimulateBp868 = new RoutedCommand();
            cmdSimulateBp868.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Shift));
            this.CommandBindings.Add(new CommandBinding(cmdSimulateBp868, simulateBp868));

            RoutedCommand cmdSimulateHbp9020 = new RoutedCommand();
            cmdSimulateHbp9020.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Shift));
            this.CommandBindings.Add(new CommandBinding(cmdSimulateHbp9020, simulateHbp9020));


            RoutedCommand cmdSimulateBc418 = new RoutedCommand();
            cmdSimulateBc418.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift));
            this.CommandBindings.Add(new CommandBinding(cmdSimulateBc418, simulateBc418));

            RoutedCommand cmdSimulateRfReader = new RoutedCommand();
            cmdSimulateRfReader.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift));
            this.CommandBindings.Add(new CommandBinding(cmdSimulateRfReader, simulateRfReader));

            this.Loaded += xcWindow_Loaded;
            this.KeyDown += XcWindow_KeyDown;
        }

        private void simulateRfReader(object sender, ExecutedRoutedEventArgs e)
        {
            xcBinder.onRfidCardRead("20-04a47c5c000104e0");
        }

        private string cardId = "";
        private int prevKeyPressed = 0;
        private void XcWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Timestamp - prevKeyPressed > 1000)
            {
                cardId = "";
            }
            prevKeyPressed = e.Timestamp;

            if (cardId.Length > 0 && (e.Key == Key.Return || e.Key == Key.Return))
            {
                xcBinder.onRfidCardRead(cardId);
                prevKeyPressed = 0;
                cardId = "";
                return;
            }
            else if (e.Key != Key.Return && e.Key != Key.Return)
            {
                prevKeyPressed = e.Timestamp;
                var keyValue = e.Key.ToString();
                switch (e.Key)
                {
                    case Key.OemPlus:
                        cardId += "+";
                        break;
                    case Key.OemMinus:
                        cardId += "-";
                        break;
                    case Key.Multiply:
                        cardId += "*";
                        break;
                    case Key.Divide:
                        cardId += "/";
                        break;
                    case Key.D0:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += ")";
                        else
                            cardId += "0";
                        break;
                    case Key.D1:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "!";
                        else
                            cardId += "1";
                        break;
                    case Key.D2:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "@";
                        else
                            cardId += "2";
                        break;
                    case Key.D3:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "#";
                        else
                            cardId += "3";
                        break;
                    case Key.D4:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "$";
                        else
                            cardId += "4";
                        break;
                    case Key.D5:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "%";
                        else
                            cardId += "5";
                        break;
                    case Key.D6:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "^";
                        else
                            cardId += "6";
                        break;
                    case Key.D7:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "&";
                        else
                            cardId += "7";
                        break;
                    case Key.D8:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "*";
                        else
                            cardId += "8";
                        break;
                    case Key.D9:
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            cardId += "(";
                        else
                            cardId += "9";
                        break;
                    default:
                        if (keyValue.Length == 1) {
                            if (Keyboard.IsKeyToggled(Key.CapsLock))
                            {
                                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                                {
                                    keyValue = keyValue.ToLower();
                                }
                            }
                            else
                            {
                                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                                    keyValue = keyValue.ToLower();
                            }
                            cardId += keyValue;
                        }
                        else
                        {
                            cardId = "";
                        }
                        break;
                }
            }
        }

        private bool isPrintableKey(KeyEventArgs e)
        {
            return e.Key.ToString().Length == 1;
        }

        private void simulateCard(object sender, ExecutedRoutedEventArgs e) {
            if (xcBinder.CardInfo.HasCard)
                xcBinder.CardInfo.simulateRemoved();
            else
                xcBinder.CardInfo.simulateInserted();
        }

        // Load the windows regardless to scale.
        void xcWindow_Loaded(object sender, RoutedEventArgs e) {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            DpiTransform = xcUiUtil.getScaleTransform(this);
            if (DpiTransform.CanFreeze)
                DpiTransform.Freeze();
            this.UpdateLayout();
            if(this.Content!=null)
                ((Panel)this.Content).LayoutTransform = DpiTransform;
        }

        public xcWindow(xcJObject jProperties) {
            apply(jProperties);
        }

        private void simulateGl150(object sender, ExecutedRoutedEventArgs e)
        {
            if (xcBinder.AppProperties.Gl150 != null)
                xcBinder.AppProperties.Gl150.simulate();
        }

        private void simulateBp868(object sender, ExecutedRoutedEventArgs e)
        {
            if (xcBinder.AppProperties.Bp868 != null)
                xcBinder.AppProperties.Bp868.simulate();
        }

        private void simulateHbp9020(object sender, ExecutedRoutedEventArgs e) {
            if (xcBinder.AppProperties.Hbp9020 != null)
                xcBinder.AppProperties.Hbp9020.simulate();
        }

        private void simulateBc418(object sender, ExecutedRoutedEventArgs e) {
            if (xcBinder.AppProperties.Bc418 != null)
                xcBinder.AppProperties.Bc418.simulate();
        }

        private void apply(xcJObject jproperties) {
            foreach (var property in jproperties.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    foreach (JToken childProperty in property.Value) {
                        apply(new xcJProperty(new JProperty(property.Name, childProperty), jproperties.PathUri));
                    }
                } else {
                    // process properites
                    apply(new xcJProperty(property, jproperties.PathUri));
                }
            }
        }

        private void apply(xcJProperty jProperty) {
            switch (jProperty.Key.ToLower()) {
                case "title":
                    Title = jProperty.String;
                    break;
                case "name":
                    Name = jProperty.String;
                    break;
                case "icon":
                    this.Icon = jProperty.ImageSource;
                    break;
                case "resizemode":
                    this.ResizeMode = jProperty.ResizeMode;
                    break;
                case "background":
                    Background = jProperty.Brush;
                    break;
                case "foreground":
                    Foreground = jProperty.Brush;
                    break;
                case "borderbrush":
                    BorderBrush = jProperty.Brush;
                    break;
                case "windowstyle":
                case "borderstyle":
                    this.WindowStyle = jProperty.WindowStyle;
                    break;
                case "borderthickness":
                    this.BorderThickness = jProperty.Thickness;
                    break;
                case "windowstate":
                    WindowState = jProperty.WindowState;
                    break;
                case "startuplocation":
                    WindowStartupLocation = jProperty.WindowStartupLocation;
                    break;
                case "left":
                case "x":
                    this.Left = jProperty.Float;
                    break;
                case "top":
                case "y":
                    this.Top = jProperty.Float;
                    break;
                case "location":
                    Point location = jProperty.Point;
                    this.Left = location.X;
                    this.Top = location.Y;
                    break;
                case "showintaskbar":
                    ShowInTaskbar = jProperty.getBoolean(true);
                    break;
                case "hidetaskbar":
                    if (jProperty.getBoolean(false))
                        xcTaskbar.hide();
                    break;
                case "fullscreen":
                    if (jProperty.getBoolean(false)) {
                        this.WindowStyle = System.Windows.WindowStyle.None;
                        this.ResizeMode = System.Windows.ResizeMode.NoResize;
                        this.WindowState = System.Windows.WindowState.Maximized;
                    } else {
                        this.WindowState = System.Windows.WindowState.Normal;
                    }
                    break;
                case "topmost":
                    this.Topmost = jProperty.getBoolean(true);
                    break;
                case "cursor":
                    Cursor = jProperty.Cursor;
                    break;
                case "size":
                    XiMPLib.xType.xcSize size = jProperty.Size;
                    if (size.hasSize()) {
                        Width = size.Width;
                        Height = size.Height;
                    }
                    break;
                case "width":
                    XiMPLib.xType.xcLength width = new XiMPLib.xType.xcLength(jProperty.String);
                    if (width.hasLength()) {
                        this.Width = width.Pixel;
                    }
                    break;
                case "height":
                    XiMPLib.xType.xcLength height = new XiMPLib.xType.xcLength(jProperty.String);
                    if (height.hasLength()) {
                        this.Height = height.Pixel;
                    }
                    break;
                case "font":
                    xcFontProperty fontProperty = jProperty.FontProperty;
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
                    this.FontFamily = jProperty.FontFamily;
                    break;
                case "fontsize":
                    XiMPLib.xType.xcLength length = new XiMPLib.xType.xcLength(jProperty.String, XiMPLib.xType.xcLength.UNIT.POINT, xcUiProperty.getDpiY());
                    this.FontSize = length.Point;
                    break;
                case "fontweight":
                    this.FontWeight = jProperty.FontWeight;
                    break;
                case "fontstyle":
                    this.FontStyle = jProperty.FontStyle;
                    break;
                case "fontstretch":
                    this.FontStretch = jProperty.FontStretch;
                    break;
                case "horizontalalignment":
                    this.HorizontalAlignment = jProperty.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    this.VerticalAlignment = jProperty.VerticalAlignment;
                    break;
                case "margin":
                    this.Margin = jProperty.Thickness;
                    break;
                case "allowstransparency":
                    this.AllowsTransparency = jProperty.getBoolean(false);
                    break;
                case "opacity":
                    this.Opacity = jProperty.getFloat(100);
                    break;
                case "layout":
                    Content = new xcLayout(new xcJObject((JObject)jProperty.Value), jProperty.ParentUri).Panel;
                    break;
                default:
                    break;
            }
        }

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
