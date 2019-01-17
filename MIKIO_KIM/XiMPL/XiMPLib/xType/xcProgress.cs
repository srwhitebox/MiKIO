using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcProgress : INotifyPropertyChanged {
        public int MessageID {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }

        public string Message {
            get;
            set;
        }

        public int Maximum {
            get;
            set;
        }

        public int Minimum {
            get;
            set;
        }

        public int Progress {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public xcProgress() {
            setRange(0, 100);
            this.Progress = 0;
        }

        public void setRange(int min, int max) {
            this.Minimum = min;
            this.Maximum = max;
            notifyChanged();
        }

        public void setProgress(int progress) {
            this.Progress = progress;
            notifyChanged();
        }

        public void setMessage(string message, string title = "") {
            this.Message = message;
            this.Title = title;
            notifyChanged();
        }

        public void clearMessage() {
            setMessage("");
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
