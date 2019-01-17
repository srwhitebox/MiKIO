using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {
    class xcDrawingFont {
        public String FontFamily {
            get;
            set;
        }

        public float Size {
            get;
            set;
        }

        public FontStyle FontStyle {
            get;
            set;
        }

        public Font Font {
            get {
                return new Font(FontFamily, Size, FontStyle);
            }
        }

        public xcDrawingFont(string font) {
            if (string.IsNullOrWhiteSpace(font))
                return;
            string[] token = font.Split(',');
            FontFamily = token[0];
            Size = string.IsNullOrWhiteSpace(token[1]) ? SystemFonts.DefaultFont.Size : (float)new xcLength(token[1]).Inch;
            FontStyle = getFontStyle(token[2]);
        }

        public bool isFont() {
            return !string.IsNullOrWhiteSpace(FontFamily);
        }

        private FontStyle getFontStyle(string styleValue) {
            if (string.IsNullOrWhiteSpace(styleValue))
                return SystemFonts.DefaultFont.Style;

            string[] styles = styleValue.Split('|');
            FontStyle fontStyle = new FontStyle();    
            foreach(string styleName in styles){
                switch (xcString.removeWhiteSpace(styleName.ToLower())) {
                    case "normal":
                    case "regular":
                        this.FontStyle |= FontStyle.Regular;
                        break;
                    case "bold":
                        this.FontStyle |= FontStyle.Bold;
                        break;
                    case "italic":
                        this.FontStyle |= FontStyle.Italic;
                        break;
                    case "strikeout":
                        this.FontStyle |= FontStyle.Strikeout;
                        break;
                }
            }
            return fontStyle;
        }
    }
}
