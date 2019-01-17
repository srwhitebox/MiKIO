using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using XiMPLib.xDocument;

namespace XiMPLib.xUI {
    public class xcTextColumn : DataGridTextColumn {
        public Uri ParentUri {
            get;
            set;
        }

        public TextAlignment TextAlignment
        {
            get; set;
        }

        public SortDescription SortDescription
        {
            get
            {
                if (this.SortDirection == null)
                    return new SortDescription();

                var sortDescription = new SortDescription();
                sortDescription.PropertyName = this.SortMemberPath;
                sortDescription.Direction = this.SortDirection.Value;
                return sortDescription;
            }
        }

        public xcTextColumn(xcJObject jProperties, Uri parentUri) {
            TextAlignment = TextAlignment.Center;

            ParentUri = parentUri;
            apply(jProperties);

            Setter headerSetter = new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
            Style headerStyle = new System.Windows.Style();
            headerStyle.Setters.Add(headerSetter);
            this.HeaderStyle = headerStyle;

            Style style = new System.Windows.Style();
            Setter setterAlignment = new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            style.Setters.Add(setterAlignment);

            Setter setterTextAlignment = new Setter(TextBlock.TextAlignmentProperty, TextAlignment);
            style.Setters.Add(setterTextAlignment);

            Setter setterBorderThickness = new Setter(DataGridCell.BorderThicknessProperty, new Thickness(0));
            style.Setters.Add(setterBorderThickness);

            Setter setterFocusedVisualStyle = new Setter(DataGridCell.FocusVisualStyleProperty, null);
            style.Setters.Add(setterFocusedVisualStyle);
            this.CellStyle = style;


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
                case "binding":
                    this.Binding = new Binding(property.String);
                    break;
                case "width":
                    Width = property.Float;
                    break;
                case "isreadonly":
                    IsReadOnly = property.Boolean;
                    break;
                case "foreground":
                    this.Foreground = property.Brush;
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
                case "sortdirection":
                    this.SortDirection = property.SortDirection;
                    break;
                case "stringformat":
                    this.Binding.StringFormat = property.String;
                    break;
                case "textalignment":
                    this.TextAlignment = property.TextAlignment;
                    break;
                default:
                    break;
            }
        }
    }
}
