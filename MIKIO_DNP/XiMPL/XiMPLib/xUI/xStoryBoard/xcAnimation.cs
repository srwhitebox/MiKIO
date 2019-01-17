using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using XiMPLib.xType;

namespace XiMPLib.xUI.xStoryBoard {
    public class xcAnimation {
        public static DoubleAnimation FadeIn(double fromValue = 0, double toValue = 1, string durationValue = "0:0:0.25") {
            return new DoubleAnimation(fromValue, toValue, new xcDuration(durationValue).Value);
        }

        public static DoubleAnimation FadeOut(double toValue = 1, string durationValue = "0:0:0.5") {
            return new DoubleAnimation(toValue, new xcDuration(durationValue).Value);
        }

    }
}
