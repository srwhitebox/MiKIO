using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {

    /// <summary>
    /// Color class
    /// 
    /// </summary>
    public class xcDrawingColor {
        public Color Color {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// Use the English color name or RGB value with "#xxxxxxxx"
        /// </summary>
        /// <param name="color"></param>
        public xcDrawingColor(String color){
            try {
                Color = (Color)new ColorConverter().ConvertFromString(color);
            } catch {
                Color = Color.Black;
            }
        }
    }
}
