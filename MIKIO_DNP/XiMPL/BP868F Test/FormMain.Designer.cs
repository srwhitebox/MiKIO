namespace BP868F_Test {
    partial class frmMain {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsBtnSerialPort = new System.Windows.Forms.ToolStripSplitButton();
            this.tsListeningStaus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsPortSettings = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnListen = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblSystolic = new System.Windows.Forms.Label();
            this.lblDiastolic = new System.Windows.Forms.Label();
            this.lblPulse = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.textDiastolic = new System.Windows.Forms.TextBox();
            this.textSystolic = new System.Windows.Forms.TextBox();
            this.textMeasured = new System.Windows.Forms.TextBox();
            this.textPulse = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Arial", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnSerialPort,
            this.tsListeningStaus,
            this.tsPortSettings});
            this.statusStrip1.Location = new System.Drawing.Point(0, 131);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(365, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsBtnSerialPort
            // 
            this.tsBtnSerialPort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnSerialPort.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSerialPort.Image")));
            this.tsBtnSerialPort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnSerialPort.Name = "tsBtnSerialPort";
            this.tsBtnSerialPort.Size = new System.Drawing.Size(57, 20);
            this.tsBtnSerialPort.Text = "COM1";
            this.tsBtnSerialPort.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsBtnSerialPort_DropDownItemClicked);
            // 
            // tsListeningStaus
            // 
            this.tsListeningStaus.Name = "tsListeningStaus";
            this.tsListeningStaus.Size = new System.Drawing.Size(153, 17);
            this.tsListeningStaus.Spring = true;
            // 
            // tsPortSettings
            // 
            this.tsPortSettings.Name = "tsPortSettings";
            this.tsPortSettings.Size = new System.Drawing.Size(107, 17);
            this.tsPortSettings.Text = "9600-None-8-One";
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(264, 11);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(87, 51);
            this.btnListen.TabIndex = 1;
            this.btnListen.Text = "Start";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(264, 99);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(87, 21);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblSystolic
            // 
            this.lblSystolic.AutoSize = true;
            this.lblSystolic.Location = new System.Drawing.Point(14, 41);
            this.lblSystolic.Name = "lblSystolic";
            this.lblSystolic.Size = new System.Drawing.Size(49, 15);
            this.lblSystolic.TabIndex = 3;
            this.lblSystolic.Text = "Systolic";
            // 
            // lblDiastolic
            // 
            this.lblDiastolic.AutoSize = true;
            this.lblDiastolic.Location = new System.Drawing.Point(14, 71);
            this.lblDiastolic.Name = "lblDiastolic";
            this.lblDiastolic.Size = new System.Drawing.Size(55, 15);
            this.lblDiastolic.TabIndex = 4;
            this.lblDiastolic.Text = "Diastolic";
            // 
            // lblPulse
            // 
            this.lblPulse.AutoSize = true;
            this.lblPulse.Location = new System.Drawing.Point(14, 99);
            this.lblPulse.Name = "lblPulse";
            this.lblPulse.Size = new System.Drawing.Size(39, 15);
            this.lblPulse.TabIndex = 5;
            this.lblPulse.Text = "Pulse";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(14, 16);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(74, 15);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "Measured At";
            // 
            // textDiastolic
            // 
            this.textDiastolic.Enabled = false;
            this.textDiastolic.Location = new System.Drawing.Point(93, 71);
            this.textDiastolic.Name = "textDiastolic";
            this.textDiastolic.Size = new System.Drawing.Size(163, 21);
            this.textDiastolic.TabIndex = 7;
            // 
            // textSystolic
            // 
            this.textSystolic.Enabled = false;
            this.textSystolic.Location = new System.Drawing.Point(93, 41);
            this.textSystolic.Name = "textSystolic";
            this.textSystolic.Size = new System.Drawing.Size(163, 21);
            this.textSystolic.TabIndex = 8;
            // 
            // textMeasured
            // 
            this.textMeasured.Enabled = false;
            this.textMeasured.Location = new System.Drawing.Point(93, 13);
            this.textMeasured.Name = "textMeasured";
            this.textMeasured.Size = new System.Drawing.Size(163, 21);
            this.textMeasured.TabIndex = 9;
            // 
            // textPulse
            // 
            this.textPulse.Enabled = false;
            this.textPulse.Location = new System.Drawing.Point(93, 99);
            this.textPulse.Name = "textPulse";
            this.textPulse.Size = new System.Drawing.Size(163, 21);
            this.textPulse.TabIndex = 10;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 153);
            this.Controls.Add(this.textPulse);
            this.Controls.Add(this.textMeasured);
            this.Controls.Add(this.textSystolic);
            this.Controls.Add(this.textDiastolic);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblPulse);
            this.Controls.Add(this.lblDiastolic);
            this.Controls.Add(this.lblSystolic);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "AmpAll BP868F Test";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton tsBtnSerialPort;
        private System.Windows.Forms.ToolStripStatusLabel tsListeningStaus;
        private System.Windows.Forms.ToolStripStatusLabel tsPortSettings;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblSystolic;
        private System.Windows.Forms.Label lblDiastolic;
        private System.Windows.Forms.Label lblPulse;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox textDiastolic;
        private System.Windows.Forms.TextBox textSystolic;
        private System.Windows.Forms.TextBox textMeasured;
        private System.Windows.Forms.TextBox textPulse;
    }
}

