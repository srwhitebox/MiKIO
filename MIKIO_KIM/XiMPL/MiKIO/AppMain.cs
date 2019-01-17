using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using XiMPLib.xDocument;
using XiMPLib.xIO;
using XiMPLib.MiPHS;
using XiMPLib.xUI;
using XiMPLib.xUI.xStoryBoard;
using XiMPLib.xBinding;
using XiMPLib.xDevice.xFatMeter;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using XiMPLib.MiHIS;
using XiMPLib.xDevice.xFatAnalyser;
using System.Xml;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using XiMPLib.xType;
using XiMPLib.xNetwork;

namespace MiKIO {
    class AppMain {

        [STAThread]
        public static void Main() {
            Application app = new Application();
            app.Startup += app_Startup;
            app.Run();
            
        }

        static void app_Startup(object sender, StartupEventArgs e) {
            AppProperties Properties = xcBinder.AppProperties;

            if (new xcNetwork().IsNotAvailable) {
                MessageBox.Show("Network is not available.\nCheck the network status and try again.", "MiKIO " + Properties.AppVersion);
                System.Environment.Exit(0);
            }

            //CalledInfoList callingList = new CalledInfoList();

            //xcBinder.MiHIS = new MitacHis.xcHis();
            //xcBinder.Patient = new XiMPLib.MiKIO.Patient("A127689604", new DateTime(1983, 12, 09), 'M');
            //xcBinder.Patient.readPatientInfo();
            //HisDeptList deptList = new HisDeptList();
            //deptList.loads();
            //OpdScheduleList opdScheduleList = new OpdScheduleList();
            //opdScheduleList.loads("01", "", DateTime.Today.AddDays(3), DateTime.Today, 3);
            //RegInfoList list = new RegInfoList();
            //list.loads();

            //xcBinder.MiHIS.DoReg(DateTime.Today, 5, "01", "02");


            string home = e.Args.Length > 0 ? e.Args[0] : Properties.HomeUrl;
            Application app = (Application)sender;
            if (string.IsNullOrEmpty(home)) {
                Properties.editProperties();
                app.Shutdown();
            } else {
                xcWindow wndObject = new xcWindow(new Uri(home));
                xcBinder.mihealthClient.open();
                //wndObject.Closing += wndObject_Closing;
                if (wndObject.Icon  == null)
                {
                    wndObject.Icon = xcImageSources.getApplicationIconSource();
                }
                wndObject.Closed += wndObject_Closed;
                wndObject.Show();
            }
        }

        static void wndObject_Closed(object sender, EventArgs e) {
            System.Environment.Exit(0);
        }
    }
}
