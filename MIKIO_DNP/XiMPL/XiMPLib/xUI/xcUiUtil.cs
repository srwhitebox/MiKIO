using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace XiMPLib.xUI {
    public class xcUiUtil {
        public static ScaleTransform getScaleTransform(Visual visual) {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new ScaleTransform(1 / m.M11, 1 / m.M22);
        }
    }
}
