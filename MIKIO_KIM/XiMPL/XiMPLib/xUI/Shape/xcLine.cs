using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;

using XiMPLib.xDocument;

namespace XiMPLib.xUI.Shape{
    public class xcLine{
        public System.Windows.Shapes.Shape Shape {
            get {
                return Line;
            }
        }

        private Polyline Line;

        public xcLine(xcJObject jProperties) {
            Line = new Polyline();
            apply(jProperties);
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
                    Line.Name = property.String;
                    break;
                case "left":
                case "x":
                    Canvas.SetLeft(Line, property.Float);
                    break;
                case "top":
                case "y":
                    Canvas.SetTop(Line, property.Float);
                    break;
                case "location":
                    Point location = property.Point;
                    Canvas.SetLeft(Line, location.X);
                    Canvas.SetTop(Line, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(Line, property.Integer);
                    break;
                case "points":
                    Line.Points = new System.Windows.Media.PointCollection();
                    break;
                case "width":
                    Line.Width = property.Float;
                    break;
                case "height":
                    Line.Height = property.Float;
                    break;
                case "pen":
                case "stroke":
                    Line.Stroke = property.Brush;
                    break;
                case "thickness":
                    Line.StrokeThickness = property.Float;
                    break;
                case "pattern":
                    Line.StrokeDashArray = property.DoubleCollection;
                    break;
                case "cap":
                    Line.StrokeDashCap = property.PenLineCap;
                    break;
                case "startcap":
                    Line.StrokeStartLineCap = property.PenLineCap;
                    break;
                case "endcap":
                    Line.StrokeEndLineCap = property.PenLineCap;
                    break;
                case "fill":
                    Line.Fill = property.Brush;
                    break;
                case "effect":
                    Line.Effect = property.Effect;
                    break;
                case "opacity":
                    Line.Opacity = property.getFloat(100);
                    break;
                case "cusor":
                    Line.Cursor = property.Cursor;
                    break;
                case "margin":
                    Line.Margin = property.Thickness;
                    break;
                case "horizontalalignment":
                    Line.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    Line.VerticalAlignment = property.VerticalAlignment;
                    break;
                case "flowdirection":
                    Line.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    Line.RenderTransform = property.Transform;
                    break;
                case "visibility":
                    Line.Visibility = property.Visibility;
                    break;
                default:
                    break;
            }
        }
    }
}
