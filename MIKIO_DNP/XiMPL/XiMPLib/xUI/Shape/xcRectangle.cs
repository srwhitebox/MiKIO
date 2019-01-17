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
    public class xcRectangle{
        public System.Windows.Shapes.Shape Shape {
            get {
                return Rectangle;
            }
        }

        private Rectangle Rectangle;

        public xcRectangle(xcJObject jProperties) {
            Rectangle = new Rectangle();
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
                    Rectangle.Name = property.String;
                    break;
                case "left":
                case "x":
                    Canvas.SetLeft(Rectangle, property.Float);
                    break;
                case "top":
                case "y":
                    Canvas.SetTop(Rectangle, property.Float);
                    break;
                case "location":
                    Point location = property.Point;
                    Canvas.SetLeft(Rectangle, location.X);
                    Canvas.SetTop(Rectangle, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(Rectangle, property.Integer);
                    break;
                case "width":
                    Rectangle.Width = property.Float;
                    break;
                case "height":
                    Rectangle.Height = property.Float;
                    break;
                case "pen":
                case "stroke":
                    Rectangle.Stroke = property.Brush;
                    break;
                case "thickness":
                    Rectangle.StrokeThickness = property.Float;
                    break;
                case "pattern":
                    Rectangle.StrokeDashArray = property.DoubleCollection;
                    break;
                case "cap":
                    Rectangle.StrokeDashCap = property.PenLineCap;
                    break;
                case "startcap":
                    Rectangle.StrokeStartLineCap = property.PenLineCap;
                    break;
                case "endcap":
                    Rectangle.StrokeEndLineCap = property.PenLineCap;
                    break;
                case "fill":
                    Rectangle.Fill = property.Brush;
                    break;
                case "effect":
                    Rectangle.Effect = property.Effect;
                    break;
                case "opacity":
                    Rectangle.Opacity = property.getFloat(100);
                    break;
                case "cusor":
                    Rectangle.Cursor = property.Cursor;
                    break;
                case "margin":
                    Rectangle.Margin = property.Thickness;
                    break;
                case "horizontalalignment":
                    Rectangle.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    Rectangle.VerticalAlignment = property.VerticalAlignment;
                    break;
                case "flowdirection":
                    Rectangle.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    Rectangle.RenderTransform = property.Transform;
                    break;
                case "visibility":
                    Rectangle.Visibility = property.Visibility;
                    break;
                default:
                    break;
            }
        }
    }
}
