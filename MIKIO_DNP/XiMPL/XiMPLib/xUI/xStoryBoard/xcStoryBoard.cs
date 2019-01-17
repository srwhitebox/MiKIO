using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace XiMPLib.xUI.xStoryBoard {
    public class xcStoryBoard : Storyboard{
        public enum ANIMATION_EFFECT {
            FADE_IN,
            FADE_OUT
        }

        public DoubleAnimation Animation {
            get;
            set;
        }

        public xcStoryBoard(ANIMATION_EFFECT effect) {
            setAnimationEffect(effect);
        }

        public void setTarget(DependencyObject dependencyObject) {
            Storyboard.SetTarget(Animation, dependencyObject);
        }

        public void setTargetProperty(object objectProperty) {
            Storyboard.SetTargetProperty(Animation, new PropertyPath(objectProperty));
        }

        public void setAnimationEffect(ANIMATION_EFFECT effect){
            switch(effect){
                case ANIMATION_EFFECT.FADE_IN:
                    Animation = xcAnimation.FadeIn();
                    break;
                case ANIMATION_EFFECT.FADE_OUT:
                    Animation = xcAnimation.FadeOut();
                    break;
            }
            this.Children.Add(Animation);
        }
    }
}
