using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
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
using XiMPLib.xBinding;
using XiMPLib.xDevice;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xBpMeter.AMPAll;
using XiMPLib.xDevice.xBpMeter.Omron;
using XiMPLib.xDevice.xBpMeter.ParamaTech;
using XiMPLib.xDevice.xCardReader;
using XiMPLib.xDevice.xFatAnalyser.Tanita;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.xDevice.xFatMeter.GTech;
using XiMPLib.xDevice.xFatMeter.SuperView;
using XiMPLib.xDevice.xPrinter;

namespace PiConfig {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        private string[] peripheralTypes = { "Blood Pressure", "Fat meter", "Fat Analyser", "Any Type"};

        private string[] bpMeters = { "ParamaTech FT201", "AMPall BP868F", "Fora D40", "Omron HBP9020", "Any Model" };
        private string[] fatMeters = { "SuperView HW3030", "GTech GL150", "BC418", "Any Model" };
        private string[] fatAnalysers = {"BC418", "Any Model" };
        private string[] anyModels = { "Any Model" };
        private string[] printers = { "Advantech USB Printer"};
        private string[] cardReaders = { "Advantech USB Card Reader"};

        private string[] InterfaceType = {"USB", "Serial", "Any Interface"};
        private string[] SerialBaudRate = {"1200", "2400", "4800", "9600", "14400", "19200", "28800", "38400", "56000", "57600", "115200", "128000", "153600", "230400", "256000", "460800", "921600"};
        private string[] SerialParity = {"Even", "Mark", "None", "Odd", "Space" };
        private string[] SerialDataBits = {"5", "6", "7", "8"};
        private string[] SerialStopBits = { "One", "One Point Five", "Two"};

        private ArrayList mSerialPorts;

        private xcDeviceList DeviceList = new xcDeviceList();
        private xcDeviceList ConnectedDeviceList = new xcDeviceList();

        private xcSmartCardReader SmartCardReader;
        private XiMPLib.xDevice.xBpMeter.Fora.D40 D40;

        private AppProperties AppProperties;

        //private SerialPort mSerialPort;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow() {

            this.Loaded += MainWindow_Loaded;
            InitializeComponent();
            this.AppProperties = new XiMPLib.xBinding.AppProperties();

            //byte[] bufferData = { 0x22, 0x32, 0x32, 0x2F, 0x30, 0x35, 0x2F, 0x31, 0x35, 0x22, 0x2C, 0x22, 0x31, 0x36, 0x3A, 0x33, 0x34, 0x22, 0x2C, 0x30, 0x2C, 0x31, 0x2C, 0x30, 0x30, 0x31, 0x36, 0x38, 0x2C, 0x30, 0x36, 0x38, 0x2E, 0x39, 0x2C, 0x31, 0x39, 0x2E, 0x38, 0x2C, 0x30, 0x31, 0x33, 0x2E, 0x36, 0x2C, 0x30, 0x35, 0x35, 0x2E, 0x33, 0x2C, 0x30, 0x33, 0x37, 0x2E, 0x38, 0x2C, 0x32, 0x39, 0x2C, 0x30, 0x32, 0x34, 0x2E, 0x34, 0x2C, 0x30, 0x36, 0x35, 0x32, 0x33, 0x2C, 0x35, 0x36, 0x33, 0x2C, 0x32, 0x32, 0x33, 0x2C, 0x32, 0x34, 0x33, 0x2C, 0x32, 0x38, 0x37, 0x2C, 0x32, 0x38, 0x39, 0x2C, 0x31, 0x39, 0x2E, 0x38, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x36, 0x2C, 0x30, 0x31, 0x30, 0x2E, 0x34, 0x2C, 0x30, 0x30, 0x39, 0x2E, 0x38, 0x2C, 0x32, 0x31, 0x2E, 0x35, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x37, 0x2C, 0x30, 0x30, 0x39, 0x2E, 0x37, 0x2C, 0x30, 0x30, 0x39, 0x2E, 0x32, 0x2C, 0x31, 0x35, 0x2E, 0x34, 0x2C, 0x30, 0x30, 0x30, 0x2E, 0x36, 0x2C, 0x30, 0x30, 0x33, 0x2E, 0x30, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x38, 0x2C, 0x31, 0x36, 0x2E, 0x32, 0x2C, 0x30, 0x30, 0x30, 0x2E, 0x36, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x39, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x37, 0x2C, 0x31, 0x39, 0x2E, 0x39, 0x2C, 0x30, 0x30, 0x37, 0x2E, 0x33, 0x2C, 0x30, 0x32, 0x39, 0x2E, 0x33, 0x2C, 0x30, 0x32, 0x37, 0x2E, 0x39, 0x2C, 0x30, 0x38, 0x0D, 0x0A };
            //var device = new BC418();
            //device.Packet.setPacketData(bufferData, bufferData.Length);

            //var record = device.Packet.FatCompositionRecord;
            initDevices();
            initControls();

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e) {
            addLog(string.Format("KeyDown : {0}-{1}", e.Key, e.KeyStates));
        }

        /// <summary>
        /// Loaded Event Handler
        /// This needs to to adjust locale.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
            if (dpiTransform.CanFreeze)
                dpiTransform.Freeze();
            this.UpdateLayout();
            this.LayoutTransform = dpiTransform;
            this.Width *= dpiTransform.ScaleX;
            this.Height *= dpiTransform.ScaleY;
        }

        /// <summary>
        /// Initialize devices
        /// </summary>
        private void initDevices() {
            DeviceList.Add(new FT201());
            DeviceList.Add(new BP868F());
            DeviceList.Add(new GL150());
            DeviceList.Add(new HW3030());
            DeviceList.Add(new BC418());
            DeviceList.Add(new HBP9020());

            this.SmartCardReader = new xcSmartCardReader();
            this.SmartCardReader.OnStateChanged += SmartCardReader_OnStateChanged;
            this.SmartCardReader.Open();

            D40 = new XiMPLib.xDevice.xBpMeter.Fora.D40();
            D40.Open();
            //D40.Close();

            xcPrinter printer = new xcPrinter();
            //printer.Open();
            if (printer.IsReady) {
                addConnections(null, printer);
            }

            if (AppProperties.SerialDeviceList != null) {
                foreach (xcDevice device in AppProperties.SerialDeviceList)
                    ConnectedDeviceList.add(device);
            }
        }

        /// <summary>
        /// Smart Card Reader state change event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmartCardReader_OnStateChanged(object sender, EventArgs e) {
            XiMPLib.xDevice.xCardReader.xcSmartCardReader.CardReaderEventArgs args = (XiMPLib.xDevice.xCardReader.xcSmartCardReader.CardReaderEventArgs)e;
            switch (args.ReaderState) {
                case xcSmartCardReader.ReaderState.CONNECTED:
                    addConnections(null, this.SmartCardReader);
                    addLog(string.Format("Smart Card Reader({0}) has been connected.", this.SmartCardReader.Name));
                    break;
                case xcSmartCardReader.ReaderState.DISCONNECTED:
                    removeConnections(null, this.SmartCardReader);
                    addLog(string.Format("Smart Card Reader({0}) has been disconnected.", this.SmartCardReader.Name));
                    break;
                case xcSmartCardReader.ReaderState.CARD_INSERTED:
                    if (args.NhiCardInfo != null) {
                        addLog("Smart Card has been inserted.\n" + args.NhiCardInfo.ToString());
                    } else {
                        addLog("Smart Card has been inserted.");
                    }
                    break;
                case xcSmartCardReader.ReaderState.CARD_REMOVED:
                    addLog("Smart Card is not exist or it has been removed.");
                    break;
                case xcSmartCardReader.ReaderState.CARD_UNKNOWN:
                    addLog("Card has been inserted, but the card is not able to recognize.");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initialize Controls
        /// </summary>
        private void initControls() {
            
            dataGridConnections.ItemsSource = ConnectedDeviceList;

            ComboType.ItemsSource = peripheralTypes;
            ComboType.SelectionChanged += ComboType_SelectionChanged;
            ComboType.SelectedIndex = peripheralTypes.Length - 1;

            foreach (string portName in xcSerialPort.getAvailablePorts()) {
                comboPort.Items.Add(portName);
            }

            comboPort.Items.Add("Any port");
            comboPort.SelectedIndex = comboPort.Items.Count-1;

            comboBaudRate.ItemsSource = SerialBaudRate;
            comboBaudRate.SelectedValue = "9600";
            comboParity.ItemsSource = SerialParity;
            comboParity.SelectedValue = "None";
            comboDatabit.ItemsSource = SerialDataBits;
            comboDatabit.SelectedValue = "8";
            comboStopbit.ItemsSource = SerialStopBits;
            comboStopbit.SelectedValue = "One";

            ComboModel.SelectionChanged += ComboModel_SelectionChanged;
        }

        /// <summary>
        /// Model Selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboModel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string model = ComboModel.SelectedValue.ToString();
            foreach (xcDevice device in DeviceList){
                if (model.EndsWith(device.Name)) {
                    comboBaudRate.SelectedValue = device.BaudRate.ToString();
                    comboParity.SelectedValue = device.Parity.ToString();
                    comboDatabit.SelectedValue = device.DataBits.ToString();
                    comboStopbit.SelectedValue = device.StopBits.ToString();
                    break;
                }
            }
        }

        /// <summary>
        /// Start Serial Port Monitor
        /// </summary>
        private void startSerialPortMonitor() {
            //Open the port which is listed first.
            foreach (xcDevice deviceItem in this.ConnectedDeviceList){
                if (deviceItem.Interface == xcDevice.DEVICE_INTERFACE.SERIAL) {
                    xcSerialDevice serialDevice = (xcSerialDevice)deviceItem;
                    SerialPort port = new SerialPort(serialDevice.PortName, serialDevice.BaudRate, serialDevice.Parity, serialDevice.DataBits, serialDevice.StopBits);
                    port.DataReceived += port_DataReceived;
                }
            }


            mSerialPorts = new ArrayList();
            int baudRate = int.Parse(comboBaudRate.SelectedValue.ToString());
            Parity parity = (Parity) Enum.Parse(typeof(Parity), comboParity.SelectedValue.ToString());
            int dataBit = int.Parse(comboDatabit.SelectedValue.ToString());
            StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), comboStopbit.SelectedValue.ToString());

            if (comboPort.SelectedValue.Equals("Any port")) {
                string[] ports = xcSerialPort.getAvailablePorts();
                foreach (string portName in ports) {
                    SerialPort port = null;

                    foreach (xcDevice deviceItem in this.ConnectedDeviceList) {
                        if (!string.IsNullOrEmpty(deviceItem.PortName) && deviceItem.PortName.Equals(portName)) {
                            port = deviceItem.SerialPort; // new SerialPort(serialDevice.PortName, serialDevice.BaudRate, serialDevice.Parity, serialDevice.DataBits, serialDevice.StopBits);
                            break;
                        }
                    }
                    if (port == null)
                        port = new SerialPort(portName, baudRate, parity, dataBit, stopBits);

                    port.DataReceived += port_DataReceived;
                    try {
                        port.Open();
                        mSerialPorts.Add(port);
                        addLog(port, "Port has been opened and start watching.");
                    } catch {
                        addLog(port, "Port couldn't be opened. It may be in use.");
                    }
                }
            } else {
                string portName = comboPort.SelectedValue.ToString();
                SerialPort port = new SerialPort(portName, baudRate, parity, dataBit, stopBits);

                port.DataReceived += port_DataReceived;
                try {
                    port.Open();
                    mSerialPorts.Add(port);
                    addLog(port, "Port has been opened and start watching.");
                } catch {
                    addLog(port, "Port couldn't be opened. It may be in use.");
                }
            }
        }

        /// <summary>
        /// Serial Port Monitor
        /// </summary>
        private void stopSerialPortMonitor() {
            foreach (SerialPort port in mSerialPorts) {
                if (port.IsOpen) {
                    port.Close();
                    addLog(port, "Port has been closed.");
                }
            }
        }

        private bool isReading = false;
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort port = (SerialPort)sender;
            if (isReading)
                return;
            isReading = true;
            byte[] buffer = new byte[512];
            int i = 0;
            while (port.BytesToRead > 0) {
                buffer[i++] = (byte)port.ReadByte();
                System.Threading.Thread.Sleep(5);
            }
            
            //port.Read(buffer, 0, port.BytesToRead);
            addLog(port, buffer, i);

            foreach(xcSerialDevice device in DeviceList){
                device.Packet.setPacketData(buffer, i);
                if (device.Packet.isPacket) {
                    if (!ConnectedDeviceList.contains(device)) {
                        addConnections(port, device);
                    }
                    addLog(port, device);
                    break;
                }
            }

            isReading = false;
        }

        private delegate void ConnectionDelegate(SerialPort port, xcDevice device);

        private void addConnections(SerialPort port, xcDevice device) {
            if (btnSaveConnections.Dispatcher.CheckAccess()) {
                if (port!=null)
                    device.PortName = port.PortName;
                ConnectedDeviceList.add(device);
                dataGridConnections.Items.Refresh();
                btnSaveConnections.IsEnabled = btnResetConnections.IsEnabled = true;
            } else {
                btnSaveConnections.Dispatcher.BeginInvoke(new ConnectionDelegate(addConnections), port, device);
            }
        }

        private void removeConnections(SerialPort port, xcDevice device) {
            if (btnSaveConnections.Dispatcher.CheckAccess()) {
                ConnectedDeviceList.remove(device);
                dataGridConnections.Items.Refresh();
                btnSaveConnections.IsEnabled = btnResetConnections.IsEnabled = true;
            } else {
                btnSaveConnections.Dispatcher.BeginInvoke(new ConnectionDelegate(removeConnections), port, device);
            }
        }


        void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            switch (ComboType.SelectedIndex) {
                case 0:
                    ComboModel.ItemsSource = bpMeters;
                    ComboModel.SelectedIndex = bpMeters.Length-1;
                    break;
                case 1:
                    ComboModel.ItemsSource = fatMeters;
                    ComboModel.SelectedIndex = fatMeters.Length-1;
                    break;
                case 2:
                    ComboModel.ItemsSource = fatAnalysers;
                    ComboModel.SelectedIndex = fatMeters.Length-1;
                    break;
                case 3:
                    ComboModel.ItemsSource = anyModels;
                    ComboModel.SelectedIndex = 0;
                    break;
            }
        }

        bool isLookup = false;
        private void btnLookup_Click(object sender, RoutedEventArgs e) {
            checkBoxSerialEnabled.IsEnabled = checkBoxUSBEnabled.IsEnabled = ComboType.IsEnabled = 
                ComboModel.IsEnabled = comboPort.IsEnabled = comboBaudRate.IsEnabled = 
                comboParity.IsEnabled = comboDatabit.IsEnabled = comboStopbit.IsEnabled = isLookup;
            isLookup = !isLookup;
            if (isLookup) {
                btnLookup.Content = "Stop";
                startSerialPortMonitor();
            } else {
                btnLookup.Content = "Lookup";
                stopSerialPortMonitor();
            }
        }

        private void checkBoxSerialEnabled_Checked(object sender, RoutedEventArgs e) {
            comboPort.IsEnabled = comboBaudRate.IsEnabled = comboParity.IsEnabled = comboDatabit.IsEnabled = comboStopbit.IsEnabled = true;
        }

        private void checkBoxSerialEnabled_Unchecked(object sender, RoutedEventArgs e) {
            comboPort.IsEnabled = comboBaudRate.IsEnabled = comboParity.IsEnabled = comboDatabit.IsEnabled = comboStopbit.IsEnabled = false;
        }

        private delegate void AddLogDelegate(String text);

        private void addLog(string log) {
            if (textBoxLogs.Dispatcher.CheckAccess()) {
                string displayLog = string.Format("[{0:yyyyMMdd HH:mm}] {1}\n", DateTime.Now, log);
                textBoxLogs.AppendText(displayLog);
                textBoxLogs.ScrollToEnd();
            } else {
                textBoxLogs.Dispatcher.BeginInvoke(new AddLogDelegate(addLog), log);
            }
        }

        private void addLog(SerialPort port, string log) {
            log = string.Format("{0}({1},{2},{3},{4}) - {5}", port.PortName, port.BaudRate, port.Parity, port.DataBits, port.StopBits, log);
            addLog(log);
        }


        private void addLog(SerialPort port, byte[] data, int count) {
            string log = BitConverter.ToString(data, 0, count);
            log = log.Replace("-", " ");
            log = string.Format("{0} Received - {1}", port.PortName, log);
            addLog(log);
        }

        private void addLog(SerialPort port, xcSerialDevice device) {
            string log = string.Format("{0} is connected with {1}.", port.PortName, device.Name);
            addLog(log);
            if (device.Type == xcDevice.DEVICE_TYPE.BP_METER)
                addLog(device.Packet.BpRecord.ToString());
            else if (device.Type == xcDevice.DEVICE_TYPE.FAT_METER)
                addLog(device.Packet.FatRecord.ToString());
            if (device.Type == xcDevice.DEVICE_TYPE.FAT_ANALYSER)
                addLog(device.Packet.FatCompositionRecord.ToString());

        }

        private void btnResetConnections_Click(object sender, RoutedEventArgs e) {
            ConnectedDeviceList.Clear();
            dataGridConnections.Items.Refresh();
            btnSaveConnections.IsEnabled = btnResetConnections.IsEnabled = false;
        }

        private void btnSaveConnections_Click(object sender, RoutedEventArgs e) {
            this.AppProperties.SerialDeviceList = ConnectedDeviceList;
            this.AppProperties.save();
        }

        private void btnResetLogs_Click(object sender, RoutedEventArgs e) {
            textBoxLogs.Text = "";
        }


        private void btnSaveLogs_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Log file (*.log)|*.log|All file (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            if (saveFileDialog.ShowDialog() == true) {
                string fileName = saveFileDialog.FileName;
                File.WriteAllText(saveFileDialog.FileName, textBoxLogs.Text);
            }
        }

        private void textBoxLogs_TextChanged(object sender, TextChangedEventArgs e) {
            btnSaveLogs.IsEnabled = btnResetLogs.IsEnabled = textBoxLogs.Text.Length > 0;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.SmartCardReader.Close();
            System.Environment.Exit(0);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            //string dataString = "\"04/05/15\",\"15:52\",0,1,00170,078.3,25.9,020.3,058.0,038.9,37,027.1,06837,572,225,229,304,311,23.9,003.5,011.1,010.5,24.0,003.4,010.8,010.2,19.9,000.7,003.0,002.8,20.5,000.7,002.8,002.6,28.3,012.0,030.4,028.9\r\n";

            //byte[] data = System.Text.Encoding.ASCII.GetBytes(dataString); // { 0x02, 0x20, 0x31, 0x35, 0x30, 0x35, 0x30, 0x32, 0x20, 0x30, 0x39, 0x34, 0x33, 0x20, 0x31, 0x32, 0x35, 0x20, 0x30, 0x37, 0x35, 0x20, 0x30, 0x39, 0x39, 0x03 };
            //int baudRate = int.Parse(comboBaudRate.SelectedValue.ToString());
            //Parity parity = (Parity)Enum.Parse(typeof(Parity), comboParity.SelectedValue.ToString());
            //int dataBit = int.Parse(comboDatabit.SelectedValue.ToString());
            //StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), comboStopbit.SelectedValue.ToString());

            SerialPort mSerialPort = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One);
            ////mSerialPort.Handshake = Handshake.RequestToSend;
            byte[] bufferData = { 0x0C, 0x0D};
            mSerialPort.Open();

            mSerialPort.Write(bufferData, 0, bufferData.Length);
            byte[] readData = new byte[1024];
            //mSerialPort.Read(readData, 0, 1024)
            mSerialPort.Close();
        }


    }
}
