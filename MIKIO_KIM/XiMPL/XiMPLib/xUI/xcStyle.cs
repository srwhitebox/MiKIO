using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace XiMPLib.xUI {
    public class xcStyle : Style {
        public xcStyle(){
            this.TargetType = typeof(Button);
            this.Setters.Add(
                new Setter {
                    Property = Button.BackgroundProperty,
                    Value = xcUiProperty.toColor("Navy")
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.ForegroundProperty,
                    Value = xcUiProperty.toColor("White")
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.FontSizeProperty,
                    Value = 14
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.WidthProperty,
                    Value = 30
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.HeightProperty,
                    Value = 30
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.MarginProperty,
                    Value = "10"
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.HorizontalContentAlignmentProperty,
                    Value = "Center"
                }
            );

            this.Setters.Add(
                new Setter {
                    Property = Button.VerticalContentAlignmentProperty,
                    Value = "Center"
                }
            );


            ControlTemplate template = new ControlTemplate();
            template.TargetType = typeof(Button);
            var elementFactory = new FrameworkElementFactory(typeof(StackPanel));
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var ellipseFactory = new FrameworkElementFactory(typeof(Ellipse));
            ellipseFactory.SetValue(Ellipse.FillProperty, "White");
            gridFactory.AppendChild(ellipseFactory);
            var presentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            presentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, "Center");
            presentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, "Center");

            gridFactory.AppendChild(presentFactory);
            elementFactory.AppendChild(gridFactory);
            template.VisualTree.AppendChild(elementFactory);
            
            this.Setters.Add(
                new Setter {
                    Property = Button.TemplateProperty,
                    Value = template
                }
            );


        }
    }
}
