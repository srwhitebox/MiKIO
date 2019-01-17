using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XiMPLib.xUI {
    public class xcProgressWindow : Window{
        public const string NAME_TITLE = "Title";
        public const string NAME_MESSAGE = "Message";

        public string Title {
            get;
            set;
        }

        public string Message {
            get;
            set;
        }

        public TextBlock TextTitle {
            get {
                return getTextBlock(NAME_TITLE);
            }
        }

        public TextBlock TextMessage {
            get {
                return getTextBlock(NAME_MESSAGE);
            }
        }

        public xcProgressWindow(string title=null, string message=null) {
            this.Title = title;
            this.Message = message;
        }

        public TextBlock getTextBlock(string name) {
            foreach (TextBlock tb in FindVisualChildren<TextBlock>(this)) {
                if (tb.Name.Equals(name))
                    return tb;
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
