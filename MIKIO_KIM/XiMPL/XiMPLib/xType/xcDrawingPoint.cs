using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {
    public class xcDrawingPoint {
        public static Point toPoint(string pointValue) {
            string[] tokens = pointValue.Split(',');
            return new Point(new xcLength(tokens[0]).Pixel, new xcLength(tokens[1]).Pixel);
        }

        public static Point toPagePoint(string pointValue) {
            string[] tokens = pointValue.Split(',');
            return new Point(new xcLength(tokens[0]).PageLength, new xcLength(tokens[1]).PageLength);
        }
    }
}
