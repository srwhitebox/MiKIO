using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using XiMPLib.xDocument;
using Newtonsoft.Json.Linq;

namespace XiMPLib.xUI.Effects {
    public class xcDropShadowEffect{
        public DropShadowEffect Effect {
            get;
            set;
        }

        public static explicit operator Effect(xcDropShadowEffect effect) {
            return effect.Effect;
        }

        public xcDropShadowEffect(xcJObject jProperties) {
            Effect = new DropShadowEffect();
            apply(jProperties);
        }

        private void apply(xcJObject jProperties) {
            foreach (var property in jProperties.Properties()) {
                if (property.Value.Type == JTokenType.Array) {
                    foreach (JToken tokenItem in property.Value) {
                        // process multiple controls/panel
                    }
                } else {
                    // process properites
                    apply(new xcJProperty(property));
                }
            }
        }

        private void apply(xcJProperty property) {
            switch (property.Key) {
                case "blurradius":
                    this.Effect.BlurRadius = property.Float;
                    break;
                case "color":
                    Effect.Color = property.Color;
                    break;
                case "direction":
                    this.Effect.Direction = property.Float; 
                    break;
                case "opacity":
                    this.Effect.Opacity = property.Float;
                    break;
                case "shadowdepth":
                    this.Effect.ShadowDepth = property.Float;
                    break;
                default:
                    break;
            }
        }
    }
}
