using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace XiMPLib.xDevice {
    public delegate void OnActionKey(xcRemoteKeyboard.ACTION action);
    public class xcRemoteKeyboard {

        public enum ACTION {
            LEFT,
            RIGHT,
            UP,
            DOWN,
            NEXT,

            EYE_LEFT,
            EYE_RIGHT,
            EYE_SWITCH,
            EYE_GLASS,
            GRADE_UP,
            GRADE_DOWN,
            GRADE_RENEW,
            GRADE_RESET,
            CONFIRM,
            SAVE,

            CANCEL,

            NONE
        }

        DependencyObject dependencyObject;
        public DependencyObject DependencyObject {
            get
            {
                return this.dependencyObject;
            }
            set
            {
                if (value != null) {
                    this.dependencyObject = value;
                    InputWindow.MouseDown += InputWindow_MouseDown;
                    InputWindow.MouseMove += InputWindow_MouseMove;
                    InputWindow.KeyDown += InputWindow_KeyDown;
                }
                else if (this.dependencyObject != null){
                    InputWindow.MouseDown -= InputWindow_MouseDown;
                    InputWindow.MouseMove -= InputWindow_MouseMove;
                    InputWindow.KeyDown -= InputWindow_KeyDown;
                    this.dependencyObject = value;
                }
            }
        }

        private void InputWindow_KeyDown(object sender, KeyEventArgs e) {
            doAction(getAction(e));
        }

        private void InputWindow_MouseMove(object sender, MouseEventArgs e) {
            doAction(getAction(e));
        }

        private void InputWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            validateMouseButtonAction(e);
        }

        protected void doAction(ACTION action) {

            if (action != ACTION.NONE && OnActionKey != null) {
                OnActionKey(action);
            }
        }

        protected Window InputWindow {
            get {
                if (DependencyObject != null)
                    return Window.GetWindow(DependencyObject);
                return null;
            }
        }

        protected long prevKeyDownTimeStamp = 0;
        private System.Timers.Timer ClickTimer;

        public xcRemoteKeyboard() {

        }

        public xcRemoteKeyboard(DependencyObject dependencyObject) {
            this.DependencyObject = dependencyObject;
        }

        private void initClickTimer() {
            ClickTimer = new Timer(300);
            ClickTimer.Elapsed += new ElapsedEventHandler(EvaluateClicks);
        }

        private void EvaluateClicks(object sender, ElapsedEventArgs e) {
            ClickTimer.Stop();
        }

        protected Point startPosition;
        protected long mouseStartTime = -1;

        public OnActionKey OnActionKey
        {
            get; set;
        }

        virtual public ACTION getAction(KeyEventArgs e) {
            if (prevKeyDownTimeStamp == e.Timestamp)
                return ACTION.NONE;

            prevKeyDownTimeStamp = e.Timestamp;

            var IsShiftKey = Keyboard.Modifiers == ModifierKeys.Shift ? true : false;
            var IsCtrlKey = Keyboard.Modifiers == ModifierKeys.Control ? true : false;
            var IsAltKey = Keyboard.Modifiers == ModifierKeys.Alt ? true : false;

            return getAction(e.Key, IsAltKey, IsCtrlKey, IsShiftKey);
        }

        virtual public ACTION getAction(MouseEventArgs e) {
            return ACTION.NONE;
        }

        virtual public void validateMouseButtonAction(MouseButtonEventArgs e) {            
        }

        virtual public ACTION getAction(Key key, Boolean isAltKey, Boolean isCtrlKey, Boolean isShiftKey) {
            switch (key) {
                case Key.Up:
                    return ACTION.UP;
                case Key.Down:
                    return ACTION.DOWN;
                case Key.Left:
                    return ACTION.LEFT;
                case Key.Right:
                    return ACTION.RIGHT;
                case Key.Space:
                    return ACTION.NEXT;
                case Key.Home:
                    return ACTION.EYE_LEFT;
                case Key.End:
                    return ACTION.EYE_RIGHT;
                case Key.Tab:
                    return ACTION.EYE_SWITCH;
                case Key.OemBackslash:
                case Key.F10:
                case Key.G:
                    return ACTION.EYE_GLASS;
                case Key.PageUp:
                case Key.OemPlus:
                case Key.Add:
                    return ACTION.GRADE_UP;
                case Key.PageDown:
                case Key.OemMinus:
                case Key.Subtract:
                    return ACTION.GRADE_DOWN;
                case Key.F5:
                case Key.System:
                    return ACTION.GRADE_RENEW;
                case Key.F2:
                    return ACTION.GRADE_RESET;
                case Key.Enter:
                    if (isCtrlKey || isShiftKey)
                        return ACTION.SAVE;
                    return ACTION.CONFIRM;
                case Key.Escape:
                case Key.Delete:
                    return ACTION.CANCEL;

                case Key.S:
                    if (isCtrlKey)
                        return ACTION.SAVE;
                    else
                        return ACTION.NONE;
                default:
                    return ACTION.NONE;
            }
        }


        protected void resetCursorPosition() {
            startPosition = new Point(InputWindow.Width / 2, InputWindow.Height / 2);
            SetCursorPos((int)startPosition.X, (int)startPosition.Y);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

    }
}
