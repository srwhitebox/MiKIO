using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace XiMPLib.xDevice {
    public class LR8R : xcRemoteKeyboard {

        public LR8R() {
            init();
        }


        private DispatcherTimer ClickTimer;
        private int clickCount;

        public LR8R(DependencyObject dependencyObject) {
            base.DependencyObject = dependencyObject;
            init();
        }

        private void init() {
            ClickTimer = new DispatcherTimer();
            ClickTimer.Tick += ClickTimer_Tick;
            ClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
        }

        private void ClickTimer_Tick(object sender, EventArgs e) {
            ClickTimer.Stop();
            switch (clickCount) {
                case 1:
                    doAction(ACTION.CONFIRM);
                    break;
                case 2:
                    doAction(ACTION.SAVE);
                    break;
                case 3:
                    doAction(ACTION.CANCEL);
                    break;
            }
            clickCount = 0;
        }


        override public ACTION getAction(MouseEventArgs e) {
            const int delay = 120;
            var action = ACTION.NONE;
            var position = e.GetPosition(InputWindow);

            if (startPosition == null) {
                resetCursorPosition();
                this.mouseStartTime = e.Timestamp;
                return ACTION.NONE;
            }

            if (e.Timestamp - this.mouseStartTime > 400) {
                resetCursorPosition();
                this.mouseStartTime = e.Timestamp;

                return ACTION.NONE;
            }

            Decimal xMove = (Decimal)(position.X - startPosition.X);
            Decimal yMove = (Decimal)(position.Y - startPosition.Y);


            if (yMove == 0) {
                if (e.Timestamp - this.mouseStartTime > delay) {
                    // action = xMove > 0 ? ACTION.EYE_RIGHT : ACTION.EYE_LEFT;
                    action = xMove > 0 ? ACTION.EYE_SWITCH : ACTION.GRADE_RENEW;
                    this.mouseStartTime = e.Timestamp;
                    resetCursorPosition();
                }
            }else if (xMove == 0) {
                if (e.Timestamp - this.mouseStartTime > delay) {
                    action = yMove > 0 ? ACTION.GRADE_DOWN : ACTION.GRADE_UP;
                    resetCursorPosition();
                    this.mouseStartTime = e.Timestamp;
                }
            }
            else {
                this.mouseStartTime = e.Timestamp;
            }

            return action;
        }
        
        override public void validateMouseButtonAction(MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                if (clickCount == 0)
                    ClickTimer.Start();
                clickCount = e.ClickCount;
            }
        }

        public override ACTION getAction(Key key, bool isAltKey, bool isCtrlKey, bool isShiftKey) {
            switch (key) {
                case Key.F5:
                case Key.Escape:
                    if (isCtrlKey || isShiftKey)
                        return ACTION.CANCEL;
                    return ACTION.LEFT;
                case Key.B:
                    return ACTION.RIGHT;
                case Key.PageUp:
                    return ACTION.UP;
                case Key.PageDown:
                    return ACTION.DOWN;
                case Key.Enter:
                    if (isCtrlKey || isShiftKey)
                        return ACTION.SAVE;
                    return ACTION.NEXT;
                default:
                    return base.getAction(key, isAltKey, isCtrlKey, isShiftKey);
            }
        }

    }
}
