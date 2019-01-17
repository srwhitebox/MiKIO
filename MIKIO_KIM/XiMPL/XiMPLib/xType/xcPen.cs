using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {
    public class xcPen {

        public static Pen toPen(string penValue) {
            string[] tokens = penValue.Split(',');
            return new Pen(new xcDrawingColor(tokens[0]).Color, (float)new xcLength(tokens[1]).Pixel);
        }

        public static Pen toPagePen(string penValue) {
            string[] tokens = penValue.Split(',');
            return new Pen(new xcDrawingColor(tokens[0]).Color, new xcLength(tokens[1]).PageLength);
        }

    }
}
