using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiHealth {
    public class CheckBoxItem : INotifyPropertyChanged {
        public String Name {
            get; set;
        }

        public Boolean IsChecked {
            get; set;
        }

        public String Text {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CheckBoxItem(String name, String text, Boolean isChecked=false) {
            this.Name = name;
            this.Text = text;
            this.IsChecked = isChecked;
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
