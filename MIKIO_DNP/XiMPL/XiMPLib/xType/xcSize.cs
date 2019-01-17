using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xUI;

namespace XiMPLib.xType {
    public class xcSize {
        public int Width {
            get;
            set;
        }
        public int Height {
            get;
            set;
        }

        public xcSize(string size, UInt16 defaultDpi = 96) {
            if (string.IsNullOrWhiteSpace(size))
                return;

            string[] tokens = size.Split(',');
            Width = xcString.toInt(tokens[0]);
            Height = (tokens.Length > 1) ? xcString.toInt(tokens[1]) : Width;
        }

        public bool hasSize() {
            return Width > 0 && Height > 0;
        }
    }
}
