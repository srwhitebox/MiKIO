using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XiMPLib.xDocument;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using XiMPLib.XiMPL;
using XiMPLib.xBinding;

namespace XiMPLib.xUI {
    public class xcLayout : xcJObject {
        public Panel Panel {
            get;
            set;
        }

        public xcLayout(Uri uri)
            : base(uri) {
            generate();
        }

        public xcLayout(xcJObject jProperty, Uri parentUri=null)
            : base(jProperty) {
            this.PathUri = parentUri;
            generate();
        }

        private void generate() {
            string type = getString("type").ToLower();     
            //xcJProperty property = new xcJProperty(Property("type"));
            switch (type) {
                case "canvas":
                    Panel = new xcCanvas(this);
                    break;
                case "dockpanel":
                    Panel = new DockPanel();
                    break;
                case "tabpanel":
                    Panel = new TabPanel();
                    break;
                case "uniformgrid":
                    Panel = new UniformGrid();
                    break;
                case "stackpanel":
                    Panel = new xcStackPanel(this, PathUri);
                    break;
                case "wrappanel":
                    Panel = new WrapPanel();
                    break;
                default:
                    String src = getString("src");
                    if (!String.IsNullOrEmpty(src))
                    {
                        Uri srcUri = new Uri(new Uri(this.PathUri, "."), src);
                        Panel = new xcLayout(new xcJObject(srcUri, "Layout"), PathUri).Panel;
                    }
                    else
                        Panel = null;
                    break;
            }
            if (Panel != null) {
                if (string.IsNullOrEmpty(Panel.Name))
                    return;
                if (xcBinder.PanelDictionary.ContainsKey(Panel.Name))
                    xcBinder.PanelDictionary.Remove(Panel.Name);
                xcBinder.PanelDictionary.Add(Panel.Name, Panel);
            }
        }
    }
}
