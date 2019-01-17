using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using XiMPLib.xDocument;
using XiMPLib.xUI.Effects;

namespace XiMPLib.xUI {
    public class xcEffect : xcJObject  {
        public Effect Effect {
            get;
            set;
        }

        public xcEffect(xcJObject jProperty)
            : base(jProperty) {
            generate();
        }

        private void generate() {
            string type = getString("type").ToLower();
            switch (type) {
                case "dropshadow":
                    Effect = (Effect)new xcDropShadowEffect(this);
                    break;
            }
        }
    }
}
