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
using System.Diagnostics;

namespace MiKIO {
    class AppMain {

        [STAThread]
        public static void Main() {
            Application app = new Application();
            app.Startup += app_Startup;
            app.Run();
            //xcMessageBox.Show("test", "test", xcMessageBox.Buttons.YesNo, xcMessageBox.Icon.Exclamation);
        }

        static void app_Startup(object sender, StartupEventArgs e) {
            AppProperties Properties = xcBinder.AppProperties;
            Process pro = null;
            string[] card = null;
            //判斷有無插卡
            pro = new Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;

            pro.Start();

            pro.StandardInput.WriteLine("cd C:\\dnp");
            pro.StandardInput.WriteLine("DNP " + "123");
            pro.StandardInput.WriteLine("exit");
            pro.StandardInput.AutoFlush = true;

            card = pro.StandardOutput.ReadToEnd().ToString().Split('\n');
            
            pro.WaitForExit();

            pro.Close();
            for (int i = 0; i < card.Length; i++)
            {
                if(card[i].ToLower().Trim().Contains("the function call failed."))
                {
                    if (card[i].ToLower().Trim().Contains("80100069"))
                    {
                        MessageBox.Show("Please insert the SAM card", "Error");
                        System.Environment.Exit(0);
                    }
                    else if (card[i].ToLower().Trim().Contains("a0100000"))
                    {
                        MessageBox.Show("Please insert the correct SAM card", "Error");
                        System.Environment.Exit(0);
                    }
                }
            }                          

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
