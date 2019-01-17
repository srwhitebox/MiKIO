using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using XiMPLib.xBinding;
using XiMPLib.xDocument;

namespace XiMPLib.xUI {
    public class xcButtonColumn : DataGridTemplateColumn {
        public Uri ParentUri {
            get;
            set;
        }

        public Uri ClickUri {
            get;
            set;
        }

        public Uri ActionUri {
            get;
            set;
        }

        private FrameworkElementFactory ButtonTemplate = new FrameworkElementFactory(typeof(Button));
        public xcButtonColumn(xcJObject jProperties, Uri parentUri) {
            ParentUri = parentUri;
            apply(jProperties);

            ButtonTemplate.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler((o, e) => doAction(o, e)));

            CellTemplate = new DataTemplate() { VisualTree = ButtonTemplate };
        }

        private void doAction(object o, RoutedEventArgs e) {
            if (ActionUri != null)
                xcBinder.doAction(ActionUri);
            if (ClickUri != null)
                xcBinder.doAction(ClickUri);
        }

        public void apply(xcJObject jproperties) {
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
                case "header":
                    this.Header = property.String;
                    break;
                case "text":
                    ButtonTemplate.SetValue(Button.ContentProperty, property.String);
                    break;
                case "binding":
                    ButtonTemplate.SetBinding(Button.ContentProperty, new Binding(property.String));
                    break;
                case "width":
                    Width = property.Float;
                    break;
                case "isreadonly":
                    IsReadOnly = property.Boolean;
                    break;
                case "foreground":
                    ButtonTemplate.SetValue(Button.ForegroundProperty, property.Brush);
                    break;
                case "canresize":
                    this.CanUserResize = property.Boolean;
                    break;
                case "canreorder":
                    this.CanUserReorder = property.Boolean;
                    break;
                case "cansort":
                    this.CanUserSort = property.Boolean;
                    break;
                case "action":
                    this.ActionUri = property.Uri;
                    break;
                case "click":
                    this.ClickUri = property.Uri;
                    break;
                default:
                    break;
            }
        }

    }
}
