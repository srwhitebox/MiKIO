namespace RPGate {
    partial class FrmMain {
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.textBoxLogs = new System.Windows.Forms.TextBox();
            this.buttonBPRecord = new System.Windows.Forms.Button();
            this.buttonConfigureClock = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxLogs
            // 
            this.textBoxLogs.Location = new System.Drawing.Point(8, 12);
            this.textBoxLogs.Multiline = true;
            this.textBoxLogs.Name = "textBoxLogs";
            this.textBoxLogs.Size = new System.Drawing.Size(956, 509);
            this.textBoxLogs.TabIndex = 2;
            // 
            // buttonBPRecord
            // 
            this.buttonBPRecord.Location = new System.Drawing.Point(724, 527);
            this.buttonBPRecord.Name = "buttonBPRecord";
            this.buttonBPRecord.Size = new System.Drawing.Size(240, 59);
            this.buttonBPRecord.TabIndex = 3;
            this.buttonBPRecord.Text = "Reset Logs";
            this.buttonBPRecord.UseVisualStyleBackColor = true;
            this.buttonBPRecord.Click += new System.EventHandler(this.buttonBPRecord_Click);
            // 
            // buttonConfigureClock
            // 
            this.buttonConfigureClock.Location = new System.Drawing.Point(8, 527);
            this.buttonConfigureClock.Name = "buttonConfigureClock";
            this.buttonConfigureClock.Size = new System.Drawing.Size(240, 59);
            this.buttonConfigureClock.TabIndex = 4;
            this.buttonConfigureClock.Text = "Configure clock";
            this.buttonConfigureClock.UseVisualStyleBackColor = true;
            this.buttonConfigureClock.Click += new System.EventHandler(this.buttonConfigureClock_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 591);
            this.Controls.Add(this.buttonConfigureClock);
            this.Controls.Add(this.buttonBPRecord);
            this.Controls.Add(this.textBoxLogs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "RP-Gate Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLogs;
        private System.Windows.Forms.Button buttonBPRecord;
        private System.Windows.Forms.Button buttonConfigureClock;


    }
}

