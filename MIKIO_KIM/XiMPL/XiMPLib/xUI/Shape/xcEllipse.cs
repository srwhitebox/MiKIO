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
    public class xcEllipse{
        public System.Windows.Shapes.Shape Shape {
            get {
                return Ellipse;
            }
        }

        private Ellipse Ellipse;

        public xcEllipse(xcJObject jProperties) {
            Ellipse = new Ellipse();
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
                    Ellipse.Name = property.String;
                    break;
                case "left":
                case "x":
                    Canvas.SetLeft(Ellipse, property.Float);
                    break;
                case "top":
                case "y":
                    Canvas.SetTop(Ellipse, property.Float);
                    break;
                case "location":
                    Point location = property.Point;
                    Canvas.SetLeft(Ellipse, location.X);
                    Canvas.SetTop(Ellipse, location.Y);
                    break;
                case "zindex":
                    Canvas.SetZIndex(Ellipse, property.Integer);
                    break;
                case "width":
                    Ellipse.Width = property.Float;
                    break;
                case "height":
                    Ellipse.Height = property.Float;
                    break;
                case "pen":
                case "stroke":
                    Ellipse.Stroke = property.Brush;
                    break;
                case "thickness":
                    Ellipse.StrokeThickness = property.Float;
                    break;
                case "pattern":
                    Ellipse.StrokeDashArray = property.DoubleCollection;
                    break;
                case "cap":
                    Ellipse.StrokeDashCap = property.PenLineCap;
                    break;
                case "startcap":
                    Ellipse.StrokeStartLineCap = property.PenLineCap;
                    break;
                case "endcap":
                    Ellipse.StrokeEndLineCap = property.PenLineCap;
                    break;
                case "fill":
                    Ellipse.Fill = property.Brush;
                    break;
                case "effect":
                    Ellipse.Effect = property.Effect;
                    break;
                case "opacity":
                    Ellipse.Opacity = property.getFloat(100);
                    break;
                case "cusor":
                    Ellipse.Cursor = property.Cursor;
                    break;
                case "margin":
                    Ellipse.Margin = property.Thickness;
                    break;
                case "horizontalalignment":
                    Ellipse.HorizontalAlignment = property.HorizontalAlignment;
                    break;
                case "verticalalignment":
                    Ellipse.VerticalAlignment = property.VerticalAlignment;
                    break;
                case "flowdirection":
                    Ellipse.FlowDirection = property.FlowDirection;
                    break;
                case "transform":
                    Ellipse.RenderTransform = property.Transform;
                    break;
                case "visibility":
                    Ellipse.Visibility = property.Visibility;
                    break;
                default:
                    break;
            }
        }
    }
}
