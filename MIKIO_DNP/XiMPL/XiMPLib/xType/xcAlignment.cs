using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {
    class xcAlignment {
        public static StringAlignment toAlignment(string alignment) {
            switch (xcString.removeWhiteSpace(alignment).ToLower()) {
                case "near":
                case "left":
                case "top":
                    return StringAlignment.Near;
                case "center":
                    return StringAlignment.Center;
                case "far":
                case "right":
                case "bottom":
                    return StringAlignment.Far;
                default:
                    return StringAlignment.Center;
            }
        }

        
    }
}
