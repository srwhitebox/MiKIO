using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XiMPLib.xUI;

namespace XiMPLib.xType {
    public class xcBrushes : List<Brush>{
        public Brush Normal {
            get {
                return this.Count > 0 ? this[0] : null;
            }
        }
        public Brush Released {
            get {
                return Normal;
            }
        }

        public Brush Pressed {
            get {
                return this.Count > 1 ? this[1] : Normal;
            }
        }

        public Brush Disabled {
            get {
                return this.Count > 2 ? this[2] : Normal;
            }
        }
        public Brush Hover {
            get {
                return this.Count > 3 ? this[3] : Normal;
            }
        }

        public xcBrushes(string brushesDef, Uri parentUri=null) {
            dispatch(brushesDef, parentUri);
        }

        private void dispatch(string brushesDefine, Uri parentUri=null) {
            if (string.IsNullOrWhiteSpace(brushesDefine))
                return;
            if (brushesDefine.ToLower().Equals("none") || brushesDefine.ToLower().Equals("null"))
                return;
            string[] brushTokens = brushesDefine.Split('|');
            for (int i = 0; i < brushTokens.Length; i++) {
                this.Add(xcUiProperty.toBrush(brushTokens[i], parentUri));
            }
        }
    }
}
