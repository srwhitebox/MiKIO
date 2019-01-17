using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace XiMPLib.xUI
{
    public class xcMessageBox : Form
    {
        private const int CS_DROPSHADOW = 0x00020000;
        private static xcMessageBox _xcMessageBox;
        private Panel _plHeader = new Panel();
        private Panel _plFooter = new Panel();
        private Panel _plIcon = new Panel();
        private PictureBox _picIcon = new PictureBox();
        private FlowLayoutPanel _flpButtons = new FlowLayoutPanel();
        private Label _lblTitle;
        private Label _lblMessage;
        private List<System.Windows.Forms.Button> _buttonCollection = new List<System.Windows.Forms.Button>();
        private static DialogResult _buttonResult = new DialogResult();
        private static Timer _timer;
        private static Point lastMousePos;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        private xcMessageBox()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.FromArgb(0, 50, 90);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Width = 600;

            _lblTitle = new Label();
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Font = new System.Drawing.Font("Segoe UI", 18);
            _lblTitle.Dock = DockStyle.Top;
            _lblTitle.Height = 50;

            _lblMessage = new Label();
            _lblMessage.ForeColor = Color.White;
            _lblMessage.Font = new System.Drawing.Font("Segoe UI", 10);
            _lblMessage.Dock = DockStyle.Fill;

            _flpButtons.FlowDirection = FlowDirection.RightToLeft;
            _flpButtons.Dock = DockStyle.Fill;

            _plHeader.Dock = DockStyle.Fill;
            _plHeader.Padding = new Padding(20);
            _plHeader.Controls.Add(_lblMessage);
            _plHeader.Controls.Add(_lblTitle);

            _plFooter.Dock = DockStyle.Bottom;
            _plFooter.Padding = new Padding(20);
            _plFooter.BackColor = Color.FromArgb(0, 0, 90);
            _plFooter.Height = 80;
            _plFooter.Controls.Add(_flpButtons);

            _picIcon.Width = 32;
            _picIcon.Height = 32;
            _picIcon.Location = new Point(30, 50);

            _plIcon.Dock = DockStyle.Left;
            _plIcon.Padding = new Padding(20);
            _plIcon.Width = 70;
            _plIcon.Controls.Add(_picIcon);

            List<Control> controlCollection = new List<Control>();
            controlCollection.Add(this);
            controlCollection.Add(_lblTitle);
            controlCollection.Add(_lblMessage);
            controlCollection.Add(_flpButtons);
            controlCollection.Add(_plHeader);
            controlCollection.Add(_plFooter);
            controlCollection.Add(_plIcon);
            controlCollection.Add(_picIcon);

            foreach (Control control in controlCollection)
            {
                control.MouseDown += xcMessageBox_MouseDown;
                control.MouseMove += xcMessageBox_MouseMove;
            }

            this.Controls.Add(_plHeader);
            this.Controls.Add(_plIcon);
            this.Controls.Add(_plFooter);
        }

        private static void xcMessageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePos = new Point(e.X, e.Y);
            }
        }


        private static void xcMessageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _xcMessageBox.Left += e.X - lastMousePos.X;
                _xcMessageBox.Top += e.Y - lastMousePos.Y;
            }
        }

        public static DialogResult Show(string message)
        {
            _xcMessageBox = new xcMessageBox();
            _xcMessageBox._lblMessage.Text = message;

            xcMessageBox.InitButtons(Buttons.OK);

            _xcMessageBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title)
        {
            _xcMessageBox = new xcMessageBox();
            _xcMessageBox._lblMessage.Text = message;
            _xcMessageBox._lblTitle.Text = title;
            _xcMessageBox.Size = xcMessageBox.MessageSize(message);

            xcMessageBox.InitButtons(Buttons.OK);

            _xcMessageBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons)
        {
            _xcMessageBox = new xcMessageBox();
            _xcMessageBox._lblMessage.Text = message;
            _xcMessageBox._lblTitle.Text = title;
            _xcMessageBox._plIcon.Hide();

            xcMessageBox.InitButtons(buttons);

            _xcMessageBox.Size = xcMessageBox.MessageSize(message);
            _xcMessageBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icon icon)
        {
            _xcMessageBox = new xcMessageBox();
            _xcMessageBox._lblMessage.Text = message;
            _xcMessageBox._lblTitle.Text = title;

            xcMessageBox.InitButtons(buttons);
            xcMessageBox.InitIcon(icon);

            //_xcMessageBox.Size = xcMessageBox.MessageSize(message);
            _xcMessageBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icon icon, AnimateStyle style)
        {
            _xcMessageBox = new xcMessageBox();
            _xcMessageBox._lblMessage.Text = message;
            _xcMessageBox._lblTitle.Text = title;
            _xcMessageBox.Height = 0;

            xcMessageBox.InitButtons(buttons);
            xcMessageBox.InitIcon(icon);

            _timer = new Timer();
            Size formSize = xcMessageBox.MessageSize(message);

            switch (style)
            {
                case xcMessageBox.AnimateStyle.SlideDown:
                    _xcMessageBox.Size = new Size(formSize.Width, 0);
                    _timer.Interval = 1;
                    _timer.Tag = new AnimatexcMessageBox(formSize, style);
                    break;

                case xcMessageBox.AnimateStyle.FadeIn:
                    _xcMessageBox.Size = formSize;
                    _xcMessageBox.Opacity = 0;
                    _timer.Interval = 20;
                    _timer.Tag = new AnimatexcMessageBox(formSize, style);
                    break;

                case xcMessageBox.AnimateStyle.ZoomIn:
                    _xcMessageBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    _timer.Tag = new AnimatexcMessageBox(formSize, style);
                    _timer.Interval = 1;
                    break;
            }

            _timer.Tick += timer_Tick;
            _timer.Start();

            _xcMessageBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            AnimatexcMessageBox animate = (AnimatexcMessageBox)timer.Tag;

            switch (animate.Style)
            {
                case xcMessageBox.AnimateStyle.SlideDown:
                    if (_xcMessageBox.Height < animate.FormSize.Height)
                    {
                        _xcMessageBox.Height += 17;
                        _xcMessageBox.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case xcMessageBox.AnimateStyle.FadeIn:
                    if (_xcMessageBox.Opacity < 1)
                    {
                        _xcMessageBox.Opacity += 0.1;
                        _xcMessageBox.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case xcMessageBox.AnimateStyle.ZoomIn:
                    if (_xcMessageBox.Width > animate.FormSize.Width)
                    {
                        _xcMessageBox.Width -= 17;
                        _xcMessageBox.Invalidate();
                    }
                    if (_xcMessageBox.Height > animate.FormSize.Height)
                    {
                        _xcMessageBox.Height -= 17;
                        _xcMessageBox.Invalidate();
                    }
                    break;
            }
        }

        private static void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case xcMessageBox.Buttons.AbortRetryIgnore:
                    _xcMessageBox.InitAbortRetryIgnoreButtons();
                    break;

                case xcMessageBox.Buttons.OK:
                    _xcMessageBox.InitOKButton();
                    break;

                case xcMessageBox.Buttons.OKCancel:
                    _xcMessageBox.InitOKCancelButtons();
                    break;

                case xcMessageBox.Buttons.RetryCancel:
                    _xcMessageBox.InitRetryCancelButtons();
                    break;

                case xcMessageBox.Buttons.YesNo:
                    _xcMessageBox.InitYesNoButtons();
                    break;

                case xcMessageBox.Buttons.YesNoCancel:
                    _xcMessageBox.InitYesNoCancelButtons();
                    break;
            }

            foreach (System.Windows.Forms.Button btn in _xcMessageBox._buttonCollection)
            {
                btn.ForeColor = Color.FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 8);
                btn.Padding = new Padding(3);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Height = 30;
                btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                _xcMessageBox._flpButtons.Controls.Add(btn);
            }
        }

        private static void InitIcon(Icon icon)
        {
            switch (icon)
            {
                case xcMessageBox.Icon.Application:
                    _xcMessageBox._picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case xcMessageBox.Icon.Exclamation:
                    _xcMessageBox._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case xcMessageBox.Icon.Error:
                    _xcMessageBox._picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case xcMessageBox.Icon.Info:
                    _xcMessageBox._picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case xcMessageBox.Icon.Question:
                    _xcMessageBox._picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case xcMessageBox.Icon.Shield:
                    _xcMessageBox._picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case xcMessageBox.Icon.Warning:
                    _xcMessageBox._picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        private void InitAbortRetryIgnoreButtons()
        {
            System.Windows.Forms.Button btnAbort = new System.Windows.Forms.Button();
            btnAbort.Text = "Abort";
            btnAbort.Click += ButtonClick;

            System.Windows.Forms.Button btnRetry = new System.Windows.Forms.Button();
            btnRetry.Text = "Retry";
            btnRetry.Click += ButtonClick;

            System.Windows.Forms.Button btnIgnore = new System.Windows.Forms.Button();
            btnIgnore.Text = "Ignore";
            btnIgnore.Click += ButtonClick;

            this._buttonCollection.Add(btnAbort);
            this._buttonCollection.Add(btnRetry);
            this._buttonCollection.Add(btnIgnore);
        }

        private void InitOKButton()
        {
            System.Windows.Forms.Button btnOK = new System.Windows.Forms.Button();
            btnOK.Text = "OK";
            btnOK.Click += ButtonClick;

            this._buttonCollection.Add(btnOK);
        }

        private void InitOKCancelButtons()
        {
            System.Windows.Forms.Button btnOK = new System.Windows.Forms.Button();
            btnOK.Text = "OK";
            btnOK.Click += ButtonClick;

            System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;


            this._buttonCollection.Add(btnOK);
            this._buttonCollection.Add(btnCancel);
        }

        private void InitRetryCancelButtons()
        {
            System.Windows.Forms.Button btnRetry = new System.Windows.Forms.Button();
            btnRetry.Text = "OK";
            btnRetry.Click += ButtonClick;

            System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;


            this._buttonCollection.Add(btnRetry);
            this._buttonCollection.Add(btnCancel);
        }

        private void InitYesNoButtons()
        {
            System.Windows.Forms.Button btnYes = new System.Windows.Forms.Button();
            btnYes.Text = "Yes";
            btnYes.Click += ButtonClick;

            System.Windows.Forms.Button btnNo = new System.Windows.Forms.Button();
            btnNo.Text = "No";
            btnNo.Click += ButtonClick;


            this._buttonCollection.Add(btnYes);
            this._buttonCollection.Add(btnNo);
        }

        private void InitYesNoCancelButtons()
        {
            System.Windows.Forms.Button btnYes = new System.Windows.Forms.Button();
            btnYes.Text = "Abort";
            btnYes.Click += ButtonClick;

            System.Windows.Forms.Button btnNo = new System.Windows.Forms.Button();
            btnNo.Text = "Retry";
            btnNo.Click += ButtonClick;

            System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;

            this._buttonCollection.Add(btnYes);
            this._buttonCollection.Add(btnNo);
            this._buttonCollection.Add(btnCancel);
        }

        private static void ButtonClick(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;

            switch (btn.Text)
            {
                case "Abort":
                    _buttonResult = DialogResult.Abort;
                    break;

                case "Retry":
                    _buttonResult = DialogResult.Retry;
                    break;

                case "Ignore":
                    _buttonResult = DialogResult.Ignore;
                    break;

                case "OK":
                    _buttonResult = DialogResult.OK;
                    break;

                case "Cancel":
                    _buttonResult = DialogResult.Cancel;
                    break;

                case "Yes":
                    _buttonResult = DialogResult.Yes;
                    break;

                case "No":
                    _buttonResult = DialogResult.No;
                    break;
            }

            _xcMessageBox.Dispose();
        }

        private static Size MessageSize(string message)
        {
            Graphics g = _xcMessageBox.CreateGraphics();
            int width = 350;
            int height = 230;

            SizeF size = g.MeasureString(message, new System.Drawing.Font("Segoe UI", 10));

            if (message.Length < 150)
            {
                if ((int)size.Width > 350)
                {
                    width = (int)size.Width;
                }
            }
            else
            {
                string[] groups = (from Match m in System.Text.RegularExpressions.Regex.Matches(message, ".{1,180}") select m.Value).ToArray();
                int lines = groups.Length + 1;
                width = 700;
                height += (int)(size.Height + 10) * lines;
            }
            return new Size(width, height);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1));
            Pen pen = new Pen(Color.FromArgb(0, 151, 251));

            g.DrawRectangle(pen, rect);
        }

        public enum Buttons
        {
            AbortRetryIgnore = 1,
            OK = 2,
            OKCancel = 3,
            RetryCancel = 4,
            YesNo = 5,
            YesNoCancel = 6
        }

        public enum Icon
        {
            Application = 1,
            Exclamation = 2,
            Error = 3,
            Warning = 4,
            Info = 5,
            Question = 6,
            Shield = 7,
            Search = 8
        }

        public enum AnimateStyle
        {
            SlideDown = 1,
            FadeIn = 2,
            ZoomIn = 3
        }

    }

    class AnimatexcMessageBox
    {
        public Size FormSize;
        public xcMessageBox.AnimateStyle Style;

        public AnimatexcMessageBox(Size formSize, xcMessageBox.AnimateStyle style)
        {
            this.FormSize = formSize;
            this.Style = style;
        }
    }

}
