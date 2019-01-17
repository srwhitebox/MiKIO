using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XiMPLib.xDevice;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xBpMeter.AMPAll;

namespace BP868F_Test {
    public partial class frmMain : Form {
        BP868F bpMeter = new BP868F();
        private bool isReading = false;

        public frmMain() {
            InitializeComponent();
            this.KeyPreview = true;

            this.KeyDown += FrmMain_KeyDown;
            tsBtnSerialPort.DropDownItems.Clear();
            
            string[] ports = getSerialPorts();
            if (ports != null && ports.Length > 0) {
                foreach (string portName in ports) {
                    tsBtnSerialPort.DropDownItems.Add(portName);
                }

                tsBtnSerialPort.Text = tsBtnSerialPort.DropDownItems[tsBtnSerialPort.DropDownItems.Count - 1].Text;

                bpMeter.OnNewRecord = onNewRecord;
                setPortStatus();
            }
            else {
                tsBtnSerialPort.Text = "No ports available";
            }
        }

        private void onNewRecord(object sender, object record) {
            xcBpRecord bpRecord = (xcBpRecord)record;

            textMeasured.Text = DateTime.Now.ToString();
            textSystolic.Text = bpRecord.Systolic.ToString();
            textDiastolic.Text = bpRecord.Diastolic.ToString();
            textPulse.Text = bpRecord.Pulse.ToString();
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e) {
            if (e.Shift && e.Control && e.KeyCode == Keys.P) {
                bpMeter.simulate();
            }
        }

        public string[] getSerialPorts() {
            string[] ports = SerialPort.GetPortNames();
            return ports;
        }

        private void btnReset_Click(object sender, EventArgs e) {
            textMeasured.Clear();
            textSystolic.Clear();
            textDiastolic.Clear();
            textPulse.Clear();

        }

        private void btnListen_Click(object sender, EventArgs e) {
            if (!bpMeter.SerialPort.IsOpen) {
                bpMeter.PortName = tsBtnSerialPort.Text;
                bpMeter.Open();
                if (bpMeter.SerialPort.IsOpen) {
                    tsListeningStaus.Text = "Listening...";
                    btnListen.Text = "Stop";
                }
            }
            else {
                bpMeter.Close();
                tsListeningStaus.Text = "Ready to listen.";
                btnListen.Text = "Listen";
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
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

            bpMeter.Packet.setPacketData(buffer, i);
            if (bpMeter.Packet.isPacket) {
                textMeasured.Text = DateTime.Now.ToString();
                textSystolic.Text = bpMeter.Packet.BpRecord.Systolic.ToString();
                textDiastolic.Text = bpMeter.Packet.BpRecord.Diastolic.ToString();
                textPulse.Text = bpMeter.Packet.BpRecord.Pulse.ToString();
            }

            isReading = false;
        }

        private void tsBtnSerialPort_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            tsBtnSerialPort.Text = e.ClickedItem.Text;
            setPortStatus();
        }

        private void setPortStatus() {
            SerialPort port  = new SerialPort(tsBtnSerialPort.Text);
            try {
                port.Open();
                tsListeningStaus.Text = "Ready to listen.";
                port.Close();
                btnListen.Enabled = true;
            }
            catch {
                tsListeningStaus.Text = "Using by other...";
                btnListen.Enabled = false;
            }
        }
    }
}
