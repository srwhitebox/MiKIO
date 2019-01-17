using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XiMPLib.xType {
    public class xcDuration{
        public Duration Value {
            get;
            set;
        }
        public xcDuration(string value){
            Value = (Duration)new DurationConverter().ConvertFromString(value);            
        }
    }
}
