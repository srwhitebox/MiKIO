using NPOI.HSSF.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XiMPLib.xDevice.xFatAnalyser;
using XiMPLib.xDocument;
using XiMPLib.xSystem;
using XiMPLib.xUI;

namespace WpfExcel {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            xcDictionary dic = new xcDictionary();
            dic.addXmlSource(new Uri(@"D:\Documents\MiKIO\lang.xpr"));
        }
    }
}
