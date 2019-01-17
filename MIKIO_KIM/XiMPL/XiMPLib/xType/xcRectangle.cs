using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XiMPLib.xType {
    public class xcRectangle{
        public xcLength Left {
            get;
            set;
        }
        public xcLength Top {
            get;
            set;
        }

        public xcLength Width {
            get;
            set;
        }

        public xcLength Height {
            get;
            set;
        }

        public Rectangle Rectangle {
            get {
                return new Rectangle(Left.Pixel, Top.Pixel, Width.Pixel, Height.Pixel);
            }
        }

        public Rectangle PageRectangle {
            get {
                return new Rectangle(Left.PageLength, Top.PageLength, Width.PageLength, Height.PageLength);
            }
        }

        public RectangleF PageRectangleF {
            get {
                return new RectangleF((float)Left.PageLength, (float)Top.PageLength, (float)Width.PageLength, (float)Height.PageLength);
            }
        }

        public xcRectangle(string rectangle) {
            string[] tokens = rectangle.Split(',');
            if (tokens.Length >= 4) {
                Left = new xcLength(tokens[0]);
                Top = new xcLength(tokens[1]);
                Width = new xcLength(tokens[2]);
                Height = new xcLength(tokens[3]);
            }
        }
    }
}
