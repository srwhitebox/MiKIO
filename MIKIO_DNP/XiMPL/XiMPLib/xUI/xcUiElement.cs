using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using XiMPLib.xDocument;

namespace XiMPLib.xUI {
    public class xcUiElement {
        public enum ELEMENT_TYPE {
            window,

            canvas,
            stackpanel,
            grid,
            
            textview,
            edittext,
            button,
            image,
            webbrowser,
            unknown
        }

        public ELEMENT_TYPE Type {
            get;
            set;
        }

        public UIElement Element {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jUiProperties"></param>
        public xcUiElement(xcJObject jUiProperties) {
            generateElement(jUiProperties);
        }

        private void generateElement(xcJObject jUiProperties) {
            string elementType = jUiProperties.getString("type");
            this.Type = ELEMENT_TYPE.unknown;

            if (string.IsNullOrWhiteSpace(elementType)) {
                return;
            }

            elementType = elementType.Trim();

            if (elementType == string.Empty) {
                return;
            }

            switch (elementType) {
                case "window":
                    this.Type = ELEMENT_TYPE.window;
                    this.Element = new xcWindow(jUiProperties);
                    break;
                case "canvas":
                    this.Type = ELEMENT_TYPE.canvas;
                    this.Element = new Canvas();
                    break;
                case "stack":
                case "stackpanel":
                case "stack_panel":
                case "linearlayout":
                case "linear_layout":
                    this.Type = ELEMENT_TYPE.stackpanel;
                    this.Element = new Canvas();
                    break;
                case "grid":
                case "gridview":
                case "grid_view":
                case "gridlayout":
                case "grid_layout":
                    this.Type = ELEMENT_TYPE.grid;
                    this.Element = new Grid();
                    break;
                case "text":
                case "label":
                case "textview":
                case "text_view":
                    this.Type = ELEMENT_TYPE.textview;
                    this.Element = new TextBlock();
                    break;
                case "input":
                case "edittext":
                case "edit_text":
                case "textbox":
                case "text_box":
                    this.Type = ELEMENT_TYPE.edittext;
                    this.Element = new TextBox();
                    break;
                case "button":
                    this.Type = ELEMENT_TYPE.button;
                    this.Element = new Button();
                    break;
                case "web":
                case "webbrowser":
                case "web_browser":
                    this.Type = ELEMENT_TYPE.webbrowser;
                    this.Element = new WebBrowser();
                    break;
                default:
                    break;
            }
        }
    }
}
