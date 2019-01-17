using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XiMPLib.xType {
    public class xcCornerRadius{
        private CornerRadius mCornerRadius;

        public CornerRadius CornerRadius {
            get {
                return mCornerRadius;
            }
        }

        /// <summary>
        /// Consrutuctor
        /// </summary>
        /// <param name="radius"></param>
        public xcCornerRadius(string radius) {
            string[] tokens = radius.Split(',');
            if (tokens.Length == 1) {         // All margin is same
                mCornerRadius = new CornerRadius(xcString.toFloat(tokens[0]));
                mCornerRadius.TopLeft = mCornerRadius.TopRight = mCornerRadius.BottomLeft = mCornerRadius.BottomRight = xcString.toFloat(tokens[0]);
            } else if (tokens.Length == 4) {
                mCornerRadius = new CornerRadius(xcString.toFloat(tokens[0]), xcString.toFloat(tokens[1]), xcString.toFloat(tokens[2]), xcString.toFloat(tokens[3]));
            }
        }
    }
}
