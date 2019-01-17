using System;
using System.Collections.Generic;
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

namespace TouchMonitor {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            textLogs.TextChanged += textLogs_TextChanged;
        }

        void textLogs_TextChanged(object sender, TextChangedEventArgs e) {
            textLogs.ScrollToEnd();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            addMouseState("Mouse Down", e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            addMouseState("Mouse Up", e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            base.OnMouseDoubleClick(e);
            addMouseState("Mouse Double Click", e);
        }

        protected override void OnStateChanged(EventArgs e) {
            base.OnStateChanged(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);

            textLogs.Width = gridMain.ActualWidth / 2;
            textLogs.Height = gridMain.ActualHeight - textLogs.Margin.Top - textLogs.Margin.Bottom;
        }

        private void addMouseState(string state, MouseButtonEventArgs e) {
            string mouseState = string.Format("{0:HH:mm.fff} : {1} : ({2}, {3}) : Left - {4}, Right -{5}\n", DateTime.Now, state, GetMousePosition().X, GetMousePosition().Y, e.LeftButton, e.RightButton);
            textLogs.Text += mouseState;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition() {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
