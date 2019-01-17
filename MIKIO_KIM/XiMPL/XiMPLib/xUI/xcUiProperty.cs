using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using XiMPLib.xType;

namespace XiMPLib.xUI {
    public class xcUiProperty {
        public static double getDpiX() {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);

            return (int)dpiXProperty.GetValue(null, null);
        }

        public static double getDpiY() {
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            return (int)dpiYProperty.GetValue(null, null);
        }

        public static WindowStyle toWindowStyle(string style, WindowStyle defaultStyle = WindowStyle.SingleBorderWindow) {
            if (string.IsNullOrWhiteSpace(style)) {
                return defaultStyle;
            }
            switch(style.ToLower().Trim()){
                case "none":
                    return WindowStyle.None;
                case "single":
                case "singleboarder":
                case "singleborderwindow":
                    return WindowStyle.SingleBorderWindow;
                case "3d":
                case "3DBoarder":
                case "threedborderwindow":
                    return WindowStyle.ThreeDBorderWindow;
                case "toolwindow":
                    return WindowStyle.ToolWindow;
                default:
                    return defaultStyle;
            }
        }

        public static WindowState toWindowState(string state, WindowState defaultState = WindowState.Normal) {
            switch (state.ToLower().Trim()) {
                case "maximized":
                    return WindowState.Maximized;
                case "minimized":
                    return WindowState.Minimized;
                case "normal":
                    return WindowState.Normal;
                default:
                    return defaultState;
            }
        }

        public static WindowStartupLocation toStartupLocation(string location, WindowStartupLocation defaultLocation = WindowStartupLocation.Manual) {
            switch (location.ToLower().Trim()) {
                case "centerscreen":
                    return WindowStartupLocation.CenterScreen;
                case "centerowner":
                    return WindowStartupLocation.CenterOwner;
                case "manual":
                    return WindowStartupLocation.Manual;
                default:
                    return defaultLocation;
            }
        }

        public static Cursor toCursor(String value) {
            switch (value.ToLower().Trim()) {
                case "appstarting":
                    return Cursors.AppStarting;
                case "arrowcd":
                    return Cursors.ArrowCD;
                case "arrow":
                    return Cursors.Arrow;
                case "cross":
                    return Cursors.Cross;
                case "hand":
                case "handcursor":
                    return Cursors.Hand;
                case "help":
                    return Cursors.Help;
                case "ibeam":
                    return Cursors.IBeam;
                case "no":
                    return Cursors.No;
                case "none":
                    return Cursors.None;
                case "pen":
                    return Cursors.Pen;
                case "scrollse":
                    return Cursors.ScrollSE;
                case "scrollwe":
                    return Cursors.ScrollWE;
                case "sizeall":
                    return Cursors.SizeAll;
                case "sizenesw":
                    return Cursors.SizeNESW;
                case "sizens":
                    return Cursors.SizeNS;
                case "sizenwse":
                    return Cursors.SizeNWSE;
                case "sizewe":
                    return Cursors.SizeWE;
                case "uparrow":
                    return Cursors.UpArrow;
                case "wait":
                case "waitcursor":
                    return Cursors.Wait;
                default:
                    if (string.IsNullOrWhiteSpace(value))
                        return null;
                    else {
                        if (System.IO.File.Exists(value)) {
                            new Cursor(value);
                        }
                        return null;
                    }
            }
        }

        public static HorizontalAlignment toHorizontalAlignment(string alignment) {
            if (string.IsNullOrWhiteSpace(alignment))
                return HorizontalAlignment.Stretch;
            switch (alignment.ToLower().Trim()) {
                case "center":
                    return HorizontalAlignment.Center;
                case "left":
                    return HorizontalAlignment.Left;
                case "right":
                    return HorizontalAlignment.Right;
                case "stretch":
                    return HorizontalAlignment.Stretch;
                default:
                    return HorizontalAlignment.Stretch;
            }
        }

        public static VerticalAlignment toVerticalAlignment(string alignment) {
            if (string.IsNullOrWhiteSpace(alignment))
                return VerticalAlignment.Stretch;
            switch (alignment.ToLower().Trim()) {
                case "center":
                    return VerticalAlignment.Center;
                case "top":
                    return VerticalAlignment.Top;
                case "bottom":
                    return VerticalAlignment.Bottom;
                case "stretch":
                    return VerticalAlignment.Stretch;
                default:
                    return VerticalAlignment.Stretch;
            }
        }

        public static ResizeMode toResizeMode(string resizeMode) {
            if (string.IsNullOrWhiteSpace(resizeMode))
                return ResizeMode.CanResize;
            switch (resizeMode.ToLower().Trim()) {
                case "canminimize":
                    return ResizeMode.CanMinimize;
                case "canresize":
                    return ResizeMode.CanResize;
                case "canresizewithgrip":
                    return ResizeMode.CanResizeWithGrip;
                case "noresize":
                case "no":
                    return ResizeMode.NoResize;
                default:
                    return ResizeMode.CanResize;
            }
        }

        public static Point toPoint(string point) {
            string[] tokens = point.Split(',');
            return new Point((double)xcDecimal.Parse(tokens[0]), (double)xcDecimal.Parse(tokens[1]));
        }

        public static Color toColor(string colorValue) {
            return toColor(colorValue, Colors.Black);
        }
        
        public static Color toColor(string colorValue, Color defaultColor) {
            try {
                return (Color)ColorConverter.ConvertFromString(colorValue);
            } catch {
                return defaultColor;
            }
        }

        public static xcBrushes toBrushes(string brushesDefine, Uri parentUri = null) {
            return new xcBrushes(brushesDefine, parentUri);
        }

        public static Brush toBrush(string brush, Uri parentUri=null) {
            if (string.IsNullOrWhiteSpace(brush))
                return null;
            if (brush.ToLower().Equals("none") || brush.ToLower().Equals("null"))
                    return null;

            int indexSeparator = brush.IndexOfAny(new char[]{':', '='});
            string brushType = indexSeparator >= 0 ? brush.Substring(0, indexSeparator) : "color";
            string brushParams = indexSeparator >= 0 ? brush.Substring(indexSeparator+1) : brush;

            if (string.IsNullOrWhiteSpace(brushParams))
                return null;

            switch (brushType.Trim().ToLower()) {
                case "color":
                case "solidcolor":
                    return new SolidColorBrush(toColor(brushParams));
                case "linear":
                case "lineargradient":
                    return toLinearGradientBrush(brushParams);
                case "radial":
                case "radialgradient":
                    return toRadialGradientBrush(brushParams);
                case "image":
                    return toImageBrush(parentUri == null ? brushParams : new System.Uri(parentUri, brushParams).AbsolutePath);
            }
            return null;
        }

        public static LinearGradientBrush toLinearGradientBrush(string brushParams){
            if (string.IsNullOrWhiteSpace(brushParams)){
                return null;
            }

            string[] tokens = brushParams.Split(';');
            
            LinearGradientBrush brush = new LinearGradientBrush();
            
            int startIndex = tokens[0].IndexOf('(') +1;
            int endIndex = tokens[0].IndexOf(')');
            brush.StartPoint = toPoint(tokens[0].Substring(startIndex, endIndex-startIndex));
            startIndex = tokens[0].LastIndexOf('(') +1;
            endIndex = tokens[0].LastIndexOf(')');
            brush.EndPoint = toPoint(tokens[0].Substring(startIndex, endIndex-startIndex));

            for (int i = 1; i < tokens.Length;  i++) {
                startIndex = tokens[i].IndexOf('(') + 1;
                endIndex = tokens[i].IndexOf(')');

                GradientStop stop = toGradientStop(tokens[i].Substring(startIndex, endIndex - startIndex));
                if (stop!=null)
                    brush.GradientStops.Add(stop);
            }
            
            return brush;
        }

        public static RadialGradientBrush toRadialGradientBrush(string brushParams) {
            if (string.IsNullOrWhiteSpace(brushParams)) {
                return null;
            }

            string[] tokens = brushParams.Split(';');

            RadialGradientBrush brush = new RadialGradientBrush();

            int startIndex = tokens[0].IndexOf('(') + 1;
            int endIndex = tokens[0].IndexOf(')');
            brush.GradientOrigin = toPoint(tokens[0].Substring(startIndex, endIndex - startIndex));

            for (int i = 1; i < tokens.Length; i++) {
                startIndex = tokens[i].IndexOf('(') + 1;
                endIndex = tokens[i].IndexOf(')');

                GradientStop stop = toGradientStop(tokens[i].Substring(startIndex, endIndex - startIndex));
                if (stop != null)
                    brush.GradientStops.Add(stop);
            }

            return brush;
        }

        public static ImageBrush toImageBrush(string brushParams) {
            if (string.IsNullOrWhiteSpace(brushParams))
                return null;
            string[] tokens = brushParams.Split(';');
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(tokens[0], UriKind.RelativeOrAbsolute));
            if (tokens.Length > 1) {
                brush.TileMode = toTileMode(tokens[1]);
            }
            if (tokens.Length > 2) {
                brush.Stretch = toStretchMode(tokens[2]);
            }
            return brush;
        }

        public static GradientStop toGradientStop(string stopValue) {
            if (string.IsNullOrWhiteSpace(stopValue))
                return null;
            
            string[] tokens = stopValue.Split(',');
            
            Color color = toColor(tokens[0]);
            double offset = (double)xcDecimal.Parse(tokens[1]);

            return new GradientStop(color, offset);
        }

        public static TileMode toTileMode(string tileMode) {
            if (string.IsNullOrWhiteSpace(tileMode))
                return TileMode.None;
            switch (tileMode.Trim().ToLower()) {
                case "none":
                    return TileMode.None;
                case "flipx":
                    return TileMode.FlipX;
                case "flipxy":
                    return TileMode.FlipXY;
                case "flipy":
                    return TileMode.FlipY;
                case "tile":
                    return TileMode.Tile;
                default:
                    return TileMode.None;
            }
        }

        public static Stretch toStretchMode(string stretchMode) {
            if (string.IsNullOrWhiteSpace(stretchMode))
                return Stretch.None;
            switch (stretchMode.Trim().ToLower()) {
                case "none":
                    return Stretch.None;
                case "fill":
                    return Stretch.Fill;
                case "uniform":
                    return Stretch.Uniform;
                case "uniformtofill":
                    return Stretch.UniformToFill;
                default:
                    return Stretch.None;
            }
        }

        public static FlowDirection toFlowDirection(string direction) {
            if (string.IsNullOrWhiteSpace(direction))
                return FlowDirection.LeftToRight;
            switch (direction.ToLower().Trim()) {
                case "toright":
                case "lefttoright":
                    return FlowDirection.LeftToRight;
                case "toleft":
                case "righttoleft":
                    return FlowDirection.RightToLeft;
                default:
                    return FlowDirection.LeftToRight;
            }
        }

        public static System.Windows.Controls.Orientation toOrientation(string orientation) {
            if (string.IsNullOrWhiteSpace(orientation))
                return System.Windows.Controls.Orientation.Horizontal;
            switch (orientation.Trim().ToLower()) {
                case "h":
                case "horizontal":
                    return System.Windows.Controls.Orientation.Horizontal;
                case "v":
                case "vertical":
                    return System.Windows.Controls.Orientation.Vertical;
                default:
                    return System.Windows.Controls.Orientation.Horizontal;
            }
        }

        public static Visibility toVisibility(string visibility) {
            if (string.IsNullOrWhiteSpace(visibility))
                return Visibility.Visible;
            switch (visibility.ToLower().Trim()) {
                case "visible":
                    return Visibility.Visible;
                case "collapsed":
                    return Visibility.Collapsed;
                case "hidden":
                    return Visibility.Hidden;
                default:
                    return Visibility.Visible;
            }
        }

        public static Transform toTransform(string matrixValue) {
            if (string.IsNullOrWhiteSpace(matrixValue))
                return null;
            
            TransformGroup transformGroup = new TransformGroup();
            string[] tokens = matrixValue.Split(';');
            foreach (string formatData in tokens) {
                string  format = formatData.Trim().ToLower();
                if (format.Length == 0)
                    continue;
                int startIndex = 0;
                int endIndex = format.IndexOf('(');
                string function = format.Substring(startIndex, endIndex - startIndex).Trim();
                startIndex = endIndex + 1;
                endIndex = format.LastIndexOf(')');
                string[] values = format.Substring(startIndex, endIndex - startIndex).Split(',');;
                switch (function) {
                    case "scale":
                        if (values.Length == 1) {
                            ScaleTransform transform = new ScaleTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[0]));
                            transformGroup.Children.Add(transform);
                        } else if (values.Length == 2) {
                            ScaleTransform transform = new ScaleTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[1]));
                            transformGroup.Children.Add(transform);
                        } else {
                            ScaleTransform transform = new ScaleTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[1]), XiMPLib.xType.xcString.toFloat(values[2]), values.Length == 3 ? XiMPLib.xType.xcString.toFloat(values[2]) : XiMPLib.xType.xcString.toFloat(values[3]));
                            transformGroup.Children.Add(transform);
                        }
                        break;
                    case "rotate":
                        if (values.Length == 1) {
                            RotateTransform transform = new RotateTransform(XiMPLib.xType.xcString.toFloat(values[0]));   
                            transformGroup.Children.Add(transform);
                        } else {
                            RotateTransform transform = new RotateTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[1]), values.Length == 2 ? XiMPLib.xType.xcString.toFloat(values[1]) : XiMPLib.xType.xcString.toFloat(values[2]));
                            transformGroup.Children.Add(transform);
                        }
                        break;
                    case "skew":
                        if (values.Length == 1) {
                            SkewTransform transform = new SkewTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[0]));
                            transformGroup.Children.Add(transform);
                        }else if (values.Length == 2) {
                            SkewTransform transform = new SkewTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[1]));
                            transformGroup.Children.Add(transform);
                        } else {
                            SkewTransform transform = new SkewTransform(XiMPLib.xType.xcString.toFloat(values[0]), XiMPLib.xType.xcString.toFloat(values[1]), XiMPLib.xType.xcString.toFloat(values[2]), values.Length == 3 ? XiMPLib.xType.xcString.toFloat(values[2]) : XiMPLib.xType.xcString.toFloat(values[3]));
                            transformGroup.Children.Add(transform);
                        }
                        break;
                }
            }
            return transformGroup;
        }

        public TextTrimming toTrimming(string trimming) {
            if (string.IsNullOrWhiteSpace(trimming))
                return TextTrimming.None;
            switch (trimming.ToLower().Trim()) {
                case "none":
                    return TextTrimming.None;
                case "characterellipsis":
                    return TextTrimming.CharacterEllipsis;
                case "wordellipsis":
                    return TextTrimming.WordEllipsis;
                default:
                    return TextTrimming.None;
            }
        }

        public TextWrapping toWrapping(string wrapping) {
            if (string.IsNullOrWhiteSpace(wrapping)) {
                return TextWrapping.NoWrap;
            }
            switch (wrapping.ToLower().Trim()) {
                case "none":
                case "nowrap":
                    return TextWrapping.NoWrap;
                case "wrap":
                    return TextWrapping.Wrap;
                case "wrapwithoverflow":
                    return TextWrapping.WrapWithOverflow;
                default :
                    return TextWrapping.NoWrap;
            }
        }

        public static TextAlignment toTextAlignment(string alignment) {
            if (string.IsNullOrWhiteSpace(alignment))
                return TextAlignment.Left;
            switch (alignment.ToLower().Trim()) {
                case "center":
                    return TextAlignment.Center;
                case "justify":
                    return TextAlignment.Justify;
                case "left":
                    return TextAlignment.Left;
                case "right":
                    return TextAlignment.Right;
                default:
                    return TextAlignment.Left;
            }
        }
    }
}
