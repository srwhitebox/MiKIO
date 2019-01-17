using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;

namespace XiMPLib.xType {
    public class xcPaperSize : PaperSize{

        public xcPaperSize(string size) {
            string[] tokens = size.Split(',');
            Width = new xcLength(tokens[0]).PageLength;
            Height = (tokens.Length > 1) ? new xcLength(tokens[1]).PageLength : Width;
        }
    }
}
