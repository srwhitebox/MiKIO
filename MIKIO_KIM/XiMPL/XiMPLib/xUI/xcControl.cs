using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XiMPLib.xDocument;
using XiMPLib.Type;
using XiMPLib.xBinding;
using WXiMPLib.xChart;

namespace XiMPLib.xUI {
    /// <summary>
    /// 
    /// </summary>
    public class xcControl : xcJObject {
        public System.Windows.UIElement Control {
            get;
            set;
        }

        /// <summary>
        /// Control parent class
        /// </summary>
        /// <param name="jProperties"></param>
        public xcControl(xcJObject jProperties, Uri parentUri = null)
            : base(jProperties) {
                this.PathUri = parentUri;
            generateInstance(jProperties);
        }

        /// <summary>
        /// Gernerate control instance
        /// </summary>
        /// <param name="jProperties"></param>
        private void generateInstance(xcJObject jProperties) {
            String type = getString("type").ToLower();
            // Generate control
            String name = null;
            switch (type) {
                case "button":
                    var button = new xcButton(jProperties, PathUri);
                    name = button.Name;
                    Control = button;
                    break;
                case "keybutton":
                    var keyButton = new xcKeyButton(jProperties, PathUri);
                    name = keyButton.Name;
                    Control = keyButton;
                    break;
                case "text":
                case "textview":
                    var textView = new xcTextView(jProperties, PathUri);
                    name = textView.Name;
                    Control = textView;
                    break;
                case "edittext":
                case "textbox":
                    var editText = new xcEditText(jProperties, PathUri);
                    name = editText.Name;
                    Control = editText;
                    break;
                case "password":
                case "passwordbox":
                case "passwordtext":
                case "editpassword":
                    var passwordBox = new xcPasswordText(jProperties, PathUri).PasswordBox;
                    name = passwordBox.Name;
                    Control = passwordBox;
                    break;
                case "autocompletebox":
                    var autoCompleteBox = new xcAutoCompleteBox(jProperties, PathUri);
                    name = autoCompleteBox.Name;
                    Control = autoCompleteBox;
                    break;
                case "image":
                    var image = new xcImage(jProperties, PathUri);
                    name = image.Name;
                    Control = image;
                    break;
                case "border":
                    var border = new xcBorder(jProperties, PathUri);
                    name = border.Name;
                    Control = border;
                    break;
                case "mediaplayer":
                    var mediaPlayer = new xcMediaPlayer(jProperties, PathUri);
                    name = mediaPlayer.Name;
                    Control = mediaPlayer;
                    break;
                case "imageslider":
                    var imageSlider = new xcImageSlider(jProperties, PathUri);
                    name = imageSlider.Name;
                    Control = imageSlider;
                    break;
                case "datagrid":
                    var dataGrid = new xcDataGrid(jProperties, PathUri);
                    name = dataGrid.Name;
                    Control = dataGrid;
                    break;
                case "webview":
                    var webView = new xcWebView(jProperties, PathUri);
                    name = webView.Name;
                    Control = webView;
                    break;
                case "webbrowser":
                    var webView2 = new xcWebView2(jProperties, PathUri);
                    name = webView2.Name;
                    Control = webView2;
                    break;
                case "datepicker":
                    var datePicker = new xcDatePicker(jProperties, PathUri);
                    name = datePicker.Name;
                    Control = datePicker;
                    break;
                case "calendar":
                    var calendar = new xcCalendar(jProperties, PathUri);
                    name = calendar.Name;
                    Control = calendar;
                    break;
                case "phschart":
                    var phsChart = new xcPhsChart(jProperties, PathUri);
                    name = phsChart.Name;
                    Control = phsChart;
                    break;
                case "checkbox":
                    var checkBox = new xcCheckBox(jProperties, PathUri);
                    name = checkBox.Name;
                    Control = checkBox;
                    break;
                case "radiobutton":
                case "radio":
                    var radioButton = new xcRadioButton(jProperties, PathUri);
                    name = radioButton.Name;
                    Control = radioButton;
                    break;
            }

            if (Control != null) {
                if (string.IsNullOrEmpty(name))
                    return;
                if (xcBinder.ControlDictionary.ContainsKey(name))
                    xcBinder.ControlDictionary.Remove(name);
                xcBinder.ControlDictionary.Add(name, Control);
            }
        }
    }
}
