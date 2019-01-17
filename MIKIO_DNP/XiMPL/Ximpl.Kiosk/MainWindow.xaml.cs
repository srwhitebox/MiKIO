using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XiMPLib.UI;
using XiMPLib.XiMPL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using XiMPLib.Document;
using System.Data;
using XiMPLib.Type;

using XiMPLib.xShell;
using XiMPLib.xDevice.xCardReader;

namespace Ximpl.Kiosk
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ////XiMPLib.xDevice.xBpMeter.BP868F.getRecord(XiMPLib.xDevice.xBpMeter.BP868F.packet);
            xcWindow wndObject = new xcWindow(new Uri(@"c:\\ximpl\layout\kisok/body_bpr.xpl"));
            XiMPLib.Type.xcObject.copyProperties(this, wndObject);

            //SmartCardReader.startCardReaderMonitor(); 
            //opdProgress();

        }



        private void wndMain_Closed(object sender, EventArgs e) {
            xcTaskbar.show();
            Environment.Exit(0);
        }

    }
}
