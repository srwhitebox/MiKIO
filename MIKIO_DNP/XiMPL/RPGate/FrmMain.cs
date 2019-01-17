using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Net;
using System.Collections;
using SpringCard.PCSC;

using Kayak;
using Kayak.Http;

using XiMPLib.xDevice;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xFatMeter;

namespace RPGate {

    public partial class FrmMain : Form {
        //private const string DEFAULT_HOST_ROOT_URL = "http://175.98.131.247/XimplWeb/";
        private const string DEFAULT_HOST_ROOT_URL = "http://192.168.10.162:8181/ximplweb/";
        private SCardReaderList mCardReaderList;
        private Fora_D40 mBloodPressureReader = new Fora_D40();
        uint PrevReaderState = SCARD.STATE_UNAWARE;
        uint ShareMode = SCARD.SHARE_SHARED;
        private System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

        private HttpService.HttpServer HttpServer = new HttpService.HttpServer(8282);

        private XiMPLib.xType.xcNHICardInfo mCardInfo;
        private xcBpRecord mBpRecord;
        private xcFatRecord mFatRecord;

        public FrmMain() {
            InitializeComponent();

        }

        private void FrmMain_Load(object sender, EventArgs e) {
            //HttpServer.setOnHostRequestDelegate(onHostRequestDelegate);
            //HttpServer.start();
            //requestToUpdate("http://localhost:8080/ximplweb/msg");
            startCardReaderMonitor();
            startBpReaderMonitor();
            startSerialPortMonitor();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e) {
            if (mCardReaderList != null)
                mCardReaderList.StopMonitor();
            HttpServer.stop();
        }

        ///----- RS232 Monitoring ------
        ///
        ArrayList mSerialPorts;
        private void startSerialPortMonitor() {
            mSerialPorts = new ArrayList();
            string[] ports = xcSerialPort.getAvailablePorts();
            foreach (string portName in ports) {
                SerialPort port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                
                port.DataReceived += port_DataReceived;
                try {
                    port.Open();
                    mSerialPorts.Add(port);
                } catch {
                }
            }
        }

        private bool isReading = false;
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort port = (SerialPort)sender;
            if (isReading)
                return;
            isReading = true;
            byte[] buffer = new byte[256];
            int i=0;
            while (port.BytesToRead > 0) {
                buffer[i++] = (byte)port.ReadByte();
                System.Threading.Thread.Sleep(5);
            }
            port.Read(buffer, 0, port.BytesToRead);
            xcBpRecord record = AMPall_BP868F.getRecord(buffer);
            //xcFatRecord weightHeight = mFatRecord = new xcFatRecord(buffer);
            isReading = false;
            //requestToUpdate("update/weightheight", weightHeight.ToString());
            //addLog(weightHeight.ToString());
        }

        ///----- HTTP Request -----
        ///
        private void requestToUpdate(string uri, string dataText = "") {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(DEFAULT_HOST_ROOT_URL+uri);
            request.Proxy = null;
            request.Timeout = 5000;
            
            if (!string.IsNullOrWhiteSpace(dataText)) {
                request.ContentType = "application/json";
                request.Method = "POST";
                try {
                    Stream dataStream = request.GetRequestStream();
                    byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(dataText);
                    dataStream.Write(dataBytes, 0, dataBytes.Length);
                    dataStream.Close();
                } catch {

                }
            }else
                request.Method = "GET";
            try {
                request.BeginGetResponse(new AsyncCallback(RequestCallback), request);
            } catch {

            }
        }

        private void RequestCallback(IAsyncResult result) {
            if (result.AsyncState == null)
                return;
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            try {
                HttpWebResponse response = request.EndGetResponse(result) as HttpWebResponse;
                String host = response.ResponseUri.Host;
                StreamReader stIn = new StreamReader(response.GetResponseStream());
                String resultBody = stIn.ReadToEnd();
                stIn.Close();
            } catch {
            }

        }

        private void onHostRequestDelegate(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response) {
            sendHttpResponse(requestBody, response);
            switch (request.Path.Substring(1)) {
                case "printAll":
                    print();
                    break;
                case "printBPInfo":
                    print(mBpRecord);
                    break;
                case "printWHInfo":
                    print(mFatRecord);
                    break;
            }
        }

        private void sendHttpResponse(IDataProducer requestBody, IHttpResponseDelegate response) {
            var headers = new HttpResponseHead() {
                Status = "200 OK",
                Headers = new Dictionary<string, string>() {
                        { "Content-Type", "text/plain" },
                    }
            };
            response.OnResponse(headers, requestBody);
        }

        /// ------- BP Reader -------

        /// <summary>
        /// 
        /// </summary>
        void readBPRecord() {
            if (mBloodPressureReader.open()) {
                xcBpRecord bpRecord = mBloodPressureReader.getLatestRecord();
                mBloodPressureReader.close();
            }
        }

        /// <summary>
        /// Print Blood Pressure Record
        /// </summary>
        private void print(XiMPLib.xType.xcNHICardInfo cardInfo) {
            if (cardInfo == null || string.IsNullOrWhiteSpace(cardInfo.HolderIDN))
                return;

            //Boolean isReady = XiMPLib.Peripheral.Printer.xcPrinter.isReady("LK-T100");
            //if (!isReady) {
            //    addLog("Printer is not ready.");
            //    return;
            //}

            Uri uri = new Uri(@"D:\Temp\ximpl\print_cardinfo.xpl");
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", "量測日期:" + DateTime.Now.ToString(ciTaiwan.DateTimeFormat));
            pd.addModelAttribute("name", cardInfo.HolderName);
            pd.addModelAttribute("idn", cardInfo.HolderIDN);
            pd.addModelAttribute("birth_date", cardInfo.BirthDate.ToShortDateString());
            pd.print();
        }

        /// <summary>
        /// Print Blood Pressure Record
        /// </summary>
        private void print(xcBpRecord bpRecord) {
            if (bpRecord == null || bpRecord.Systolic == 0)
                return;

            //Boolean isReady = XiMPLib.Peripheral.Printer.xcPrinter.isReady("LK-T100");
            //if (!isReady) {
            //    addLog("Printer is not ready.");
            //    return;
            //}

            Uri uri = new Uri(@"D:\Temp\ximpl\print_bpinfo.xpl");
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", "量測日期:" + bpRecord.MeasuredAt.ToString(ciTaiwan.DateTimeFormat));
            pd.addModelAttribute("systolic_pressure", bpRecord.Systolic);
            pd.addModelAttribute("diastolic_pressure", bpRecord.Diastolic);
            pd.addModelAttribute("pulse", bpRecord.Pulse);
            pd.print();
        }
        
        private void print(xcFatRecord whRecord) {
            if (whRecord == null || whRecord.Height == 0)
                return;

            //Boolean isReady = XiMPLib.Peripheral.Printer.xcPrinter.isReady("LK-T100");
            //if (!isReady) {
            //    addLog("Printer is not ready.");
            //    return;
            //}

            Uri uri = new Uri(@"D:\Temp\ximpl\print_weight_height.xpl");
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", "量測日期:" + DateTime.Now.ToString(ciTaiwan.DateTimeFormat));
            pd.addModelAttribute("weight", whRecord.Weight);
            pd.addModelAttribute("height", whRecord.Height);
            pd.addModelAttribute("bmi", whRecord.BMI);
            pd.print();
        }

        private void print() {
            print(mCardInfo);
            print(mBpRecord);
            print(mFatRecord);
        }


        void startBpReaderMonitor() {
            if (mBloodPressureReader != null) {
                mBloodPressureReader.stopMonitor();
                mBloodPressureReader = null;
            }

            mBloodPressureReader = new Fora_D40();
            mBloodPressureReader.startMonitor(new xcBpMeter.statusChangedCallback(bpReaderStatusChanged));

            //mCardReaderList = new SCardReaderList(SCARD.SCOPE_SYSTEM, SCARD.ALL_READERS);
            //mCardReaderList.StartMonitor(new SCardReaderList.StatusChangeCallback(cardReaderListChanged));
        }

        delegate void bpReaderStatusChangedInvoker(string readerName, uint readerState, xcBpRecord record);
        void bpReaderStatusChanged(string readerName, uint readerState, xcBpRecord record) {
            if (InvokeRequired) {
                this.BeginInvoke(new bpReaderStatusChangedInvoker(bpReaderStatusChanged), readerName, readerState, record);
                return;
            }

            updateBpReaderState(readerName, readerState, record);
        }

        private void updateBpReaderState(string readerName, uint readerState, xcBpRecord record) {
            switch (readerState) {
                case xcBpMeter.STATUS_CONNECTED:
                    addLog(readerName + " : Blood Pressure Reader connected");
                    break;
                case xcBpMeter.STATUS_DISCONNECTED:
                    addLog("Blood Pressure Reader disconnected");
                    // print(record);
                    break;
                case xcBpMeter.STATUS_NEW_RECORD:
                    mBpRecord = record;
                    addLog(readerName + " : Blood Pressure recored found.");
                    addLog(record.ToString());
                    requestToUpdate("update/bpinfo", record.ToString());
                    if (mBloodPressureReader.open()) {
                        mBloodPressureReader.clearAllRecord();
                        mBloodPressureReader.close();
                    }            
                    break;
            }
        }

        /// <summary>
        /// ------------------- for NHI card reading
        /// </summary>
     
        void startCardReaderMonitor() {
            if (mCardReaderList != null) {
                mCardReaderList.StopMonitor();
                mCardReaderList = null;
            }

            mCardReaderList = new SCardReaderList(SCARD.SCOPE_SYSTEM, SCARD.ALL_READERS);
            mCardReaderList.StartMonitor(new SCardReaderList.StatusChangeCallback(cardReaderListChanged));
            //reader_list.StopMonitor();
            
        }

        delegate void cardReaderListChangedInvoker(string readerName, uint readerState, CardBuffer cardAtr);
        void cardReaderListChanged(string readerName, uint readerState, CardBuffer cardAtr) {
            if (InvokeRequired)
			{
				Console.WriteLine("ReaderListChanged (in background thread)");
				this.BeginInvoke(new cardReaderListChangedInvoker(cardReaderListChanged), readerName, readerState, cardAtr);
				return;
			}

            updateCardReaderState(readerName, readerState, cardAtr);
        }

        private uint mCardPrevStatus = SCARD.STATE_UNKNOWN;
        void updateCardReaderState(string readerName, uint readerState, CardBuffer cardAtr) {
            if (mCardPrevStatus == readerState)
                return;
            else
                mCardPrevStatus = readerState;

            // If Card is present
            if ((readerState & SCARD.STATE_PRESENT) != 0) {
                // If Card in use
                if ((readerState & SCARD.STATE_INUSE) != 0) {
                    /* Card in exclusive use */
                    if ((readerState & SCARD.STATE_EXCLUSIVE) != 0) {
                        // In use (exclusive);
                        ShareMode = SCARD.SHARE_EXCLUSIVE;
                        addLog("In use : Exclusive");
                    } else {
                        // In use (shared);
                        ShareMode = SCARD.SHARE_SHARED;
                        addLog(readerName + " : In use : Shared");
                    }
                } else
                    // Card is mute
                    if ((readerState & SCARD.STATE_MUTE) != 0) {
                        addLog(readerName + " : Card may not smart card or defected.");
                    } else
                        // Card is not powered
                        if ((readerState & SCARD.STATE_UNPOWERED) != 0) {
                            // Present, not powered
                            addLog(readerName + " : Card reading has been done.");
                        } else {
                            // Card is powered
                            if ((PrevReaderState & SCARD.STATE_INUSE) == 0) {
                                addLog(readerName + " : Card has been inserted. : ATR - " + cardAtr.AsString());

                                readNicCardInfo(readerName, SCARD.SHARE_SHARED);
                            }
                        }
            } else { // If Card is not present
                if ((readerState & SCARD.STATE_UNAVAILABLE) != 0) {
                    // Problem.. 
                    // Reserved (direct) 
                    addLog("Reader has been disconnected :" + readerState);
                } else
                    if ((readerState & SCARD.STATE_IGNORE) != 0) {
                        // Problem
                        // Error (ignore)
                        addLog(readerName + " : Reader has been disconnected.");

                    } else
                        if ((readerState & SCARD.STATE_UNKNOWN) != 0) {
                            // Problem
                            // Error (status unknown)
                            addLog("Error : Unknown :" + readerState);
                        } else
                            if ((readerState & SCARD.STATE_EMPTY) != 0) {
                                // No card
                                //Absent // check previous status
                                if (PrevReaderState == SCARD.STATE_UNAWARE || PrevReaderState == 65536)
                                    addLog(readerName + " : CardReader is ready. Card is not exist.");
                                else
                                    addLog(readerName + " : Card has been removed.");
                            } else {
                                // Problem
                                // Bad status;
                                if (readerState == SCARD.STATE_UNAWARE)
                                    addLog("Reader is not available.");
                                else
                                    addLog("Reader has been connected.");
                            }
            }
            
            PrevReaderState = readerState;
        }

        private void readNicCardInfo(string readerName, uint shareMode) {
            SCardChannel channel;
            channel = new SCardChannel(readerName);
            channel.ShareMode = shareMode;
            channel.Protocol = (uint)(SCARD.PROTOCOL_T0 | SCARD.PROTOCOL_T1);
            if (shareMode == SCARD.SHARE_DIRECT) {
                /* DIRECT mode opens Control page */
                /* ------------------------------ */
            } else {
                /* SHARED or EXCLUSIVE mode opens Transmit page */
                /* -------------------------------------------- */
            }

            if (!channel.Connect()) {
                // ShowError;
                return;
            }

            //Send command to prepare the information
            CAPDU capdu = new CAPDU(XiMPLib.xType.xcNHICardInfo.CmdSelectAdpu);
            RAPDU rapdu = channel.Transmit(capdu);
            //Send command to read information
            capdu = new CAPDU(XiMPLib.xType.xcNHICardInfo.CmdReadCardInfo);
            rapdu = channel.Transmit(capdu);

            //Dispatch to NHI card information
            XiMPLib.xType.xcNHICardInfo cardInfo = mCardInfo = new XiMPLib.xType.xcNHICardInfo(rapdu);

            if (cardInfo.isNHICard()) {
                addLog(cardInfo.ToString());
                requestToUpdate("update/cardinfo", cardInfo.ToString());
            } else {
                addLog("The card may not be the NHI card.");
            }
        }

        private void addLog(string log) {
            try {
                textBoxLogs.AppendText("[" + DateTime.Now.ToString(ciTaiwan.DateTimeFormat.FullDateTimePattern) + ":" + DateTime.Now.Millisecond + "] " + log + "\n");
                requestToUpdate("log/add", log);
            } catch {

            }
        }

        private void buttonBPRecord_Click(object sender, EventArgs e) {
            print(mBpRecord);
        }

        [DllImport("winscard.dll", EntryPoint = "SCardListCards", SetLastError = true)]
        public static extern uint SCardListCards(IntPtr phContext,
                                               byte[] pbAtr,
                                               byte[] rgguiInterfaces,
                                                   uint cguidInterfaceCount,
                                                   string mszCards,
                                                   ref int pcchCards);

        [DllImport ("winscard.dll", EntryPoint = "SCardIntroduceCardType", SetLastError = true)]
        public static extern uint SCardIntroduceCardType(IntPtr phContext,
                                               string szCardName,
                                               byte[] pguidPrimaryProvider,
                                               byte[] rgguidInterfaces,
                                               uint dwInterfaceCount,
                                               byte[] atr,
                                               byte[] pbAtrMask,
                                               uint cbAtrLen);

        [DllImport ("winscard.dll", EntryPoint = "SCardSetCardTypeProviderName", SetLastError = true)]
        public static extern uint SCardSetCardTypeProviderName(IntPtr phContext,
                                               string szCardName,
                                               uint dwProviderId,
                                               string szProvider);

        private void buttonConfigureClock_Click(object sender, EventArgs e) {

            //print(new XiMPLib.Peripheral.WeightHeight.xcFatRecord("167.2,64.70,M,105.4,023.2"));
            //print(mCardInfo);

            if (mBloodPressureReader.open()) {
                mBloodPressureReader.DateTime = DateTime.Now;
                mBloodPressureReader.close();
            }            
        }


    }
}
