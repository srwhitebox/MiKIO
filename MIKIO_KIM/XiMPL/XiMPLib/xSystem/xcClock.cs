using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using XiMPLib.xBinding;

namespace XiMPLib.xSystem {
    public class xcClock : INotifyPropertyChanged {
        public string Text {
            get;
            set;
        }



        public string Format {
            get;
            set;
        }

        private CultureInfo CultureInfo
        {
            get
            {
                return xcBinder.AppProperties.CultureInfo;
            }
        }
        private Timer Timer = new Timer(1000);
        public event PropertyChangedEventHandler PropertyChanged;

        public xcClock() {
            Format = "yyyy年 M月 d日 dddd tthh:mm";
            setTimeText();
            Timer.Elapsed += Timer_Elapsed;
            start();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            setTimeText();
        }

        private void setTimeText() {
            Text = DateTime.Now.ToString(Format, CultureInfo);
            OnPropertyChanged("Text");
        }

        public void start(){
            Timer.Enabled = true;
        }
        
        public void stop(){
            Timer.Enabled = false;
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
