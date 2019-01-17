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
    public class xcPolygon{
        public System.Windows.Shapes.Shape Shape {
            get {
                return Polygon;
            }
        }

        private Polygon Polygon;

        public xcPolygon(xcJObject jProperties) {
            Polygon = new Polygon();
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
                    Polygon.Name = property.String;
                    break;
                case "left":
                case "x":
                    Canvas.SetLeft(Polygon, property.Float);
                    break;
                case "top":
                case "y":
                    Canvas.SetTop(Polygon, property.Float);
                    break;
                case "location":
                    Point location = property.Point;
                    Canvas.SetLeft(Polygon, location.X);
                    Canvas.SetTop(Polygon, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(Polygon, property.Integer);
                    break;
                case "points":
                    Polygon.Points = new System.Windows.Media.PointCollection();
                    break;
                case "width":
                    Polygon.Width = property.Float;
                    break;
                case "height":
                    Polygon.Height = property.Float;
                    break;
                case "pen":
                case "stroke":
                    Polygon.Stroke = property.Brush;
                    break;
                case "thickness":
                    Polygon.StrokeThickness = property.Float;
                    break;
                case "pattern":
                    Polygon.StrokeDashArray = property.DoubleCollection;
                    break;
                case "cap":
                    Polygon.StrokeDashCap = property.PenLineCap;
                    break;
                case "startcap":
                    Polygon.StrokeStartLineCap = property.PenLineCap;
                    break;
                case "endcap":
                    Polygon.StrokeEndLineCap = property.PenLineCap;
                    break;
                case "fill":
                    Polygon.Fill = property.Brush;
                    break;
                case "effect":
                    Polygon.Effect = property.Effect;
                    break;
                case "opacity":
                    Polygon.Opacity = property.getFloat(100);
                    break;
                case "cusor":
                    Polygon.Cursor = property.Cursor;
                    break;
                case "margin":
                    Polygon.Margin = property.Thickness;
                    break;
                case "horizontalalignment":
                    Polygon.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    Polygon.VerticalAlignment = property.VerticalAlignment;
                    break;
                case "flowdirection":
                    Polygon.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    Polygon.RenderTransform = property.Transform;
                    break;
                case "visibility":
                    Polygon.Visibility = property.Visibility;
                    break;
                default:
                    break;
            }
        }
    }
}
