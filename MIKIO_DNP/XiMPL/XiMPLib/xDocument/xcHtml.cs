using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace XiMPLib.xDocument {
    class xcHtml {
        public WebBrowser WebBrowser {
            get;
            set;
        }

        public void Print(string html) {
            PrintDialog pd = new PrintDialog();
            
            WebBrowser = new WebBrowser();
            WebBrowser.DocumentText = html;
            while (WebBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            WebBrowser.Print();
            
        }

        private void loadHtml(){

        }
    }
}
