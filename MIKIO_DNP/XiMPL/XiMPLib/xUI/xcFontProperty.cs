using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XiMPLib.xUI {
    /// <summary>
    /// Font Property class
    /// </summary>
    public class xcFontProperty {

        /// <summary>
        /// Font Family Property
        /// </summary>
        public System.Windows.Media.FontFamily FontFamily {
            get;
            set;
        }

        /// <summary>
        /// Font size property
        /// </summary>
        public double FontSize {
            get;
            set;
        }

        /// <summary>
        /// Font Weight property
        /// </summary>
        public FontWeight FontWeight {
            get;
            set;
        }

        /// <summary>
        /// Font style property
        /// </summary>
        public FontStyle FontStyle {
            get;
            set;
        }

        /// <summary>
        /// Font Stretch property
        /// </summary>
        public FontStretch FontStretch {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property"></param>
        public xcFontProperty(string property) {
            dispatch(property);
        }

        /// <summary>
        /// Determine whether this is font
        /// </summary>
        /// <returns></returns>
        public bool isFont() {
            return FontFamily != null;
        }

        /// <summary>
        /// Dispatch property string
        /// </summary>
        /// <param name="property"></param>
        private void dispatch(string property) {
            if (string.IsNullOrWhiteSpace(property))
                return;

            string[] properties = property.Split(',');
            if (properties.Length>0)
                FontFamily = new System.Windows.Media.FontFamily(properties[0]);
            if (properties.Length > 1) {
                XiMPLib.xType.xcLength length = new XiMPLib.xType.xcLength(properties[1], XiMPLib.xType.xcLength.UNIT.POINT, xcUiProperty.getDpiY());
                FontSize = length.Point;
            }
            if (properties.Length > 2)
                FontWeight = getFontWeight(properties[2]);
            if (properties.Length > 3)
                FontStyle = getFontStyle(properties[3]);
            if (properties.Length > 4)
                FontStretch = getFontStretch(properties[4]);
        }

        /// <summary>
        /// Convert to Font Style
        /// </summary>
        /// <param name="styleValue"></param>
        /// <returns></returns>
        public static FontStyle getFontStyle(string styleValue) {
            if (string.IsNullOrWhiteSpace(styleValue))
                return FontStyles.Normal;
            switch (styleValue.ToLower().Trim()) {
                case "italic":
                    return FontStyles.Italic;
                case "normal":
                    return FontStyles.Normal;
                case "oblique":
                    return FontStyles.Oblique;
                default:
                    return FontStyles.Normal;

            }
        }

        /// <summary>
        /// Convert to Font weight
        /// </summary>
        /// <param name="weightValue"></param>
        /// <returns></returns>
        public static FontWeight getFontWeight(String weightValue) {
            if (string.IsNullOrWhiteSpace(weightValue))
                return FontWeights.Normal;

            switch (weightValue.ToLower().Trim()) {
                case "black":
                    return FontWeights.Black;
                case "700":
                case "bold":
                    return FontWeights.Bold;
                case "600":
                case "demibold":
                    return FontWeights.DemiBold;
                case "extrablack":
                    return FontWeights.ExtraBlack;
                case "800":
                case "extrabold":
                    return FontWeights.ExtraBold;
                case "200":
                case "extralight":
                    return FontWeights.ExtraLight;
                case "900":
                case "heavy":
                    return FontWeights.Heavy;
                case "300":
                case "light":
                    return FontWeights.Light;
                case "500":
                case "medium":
                    return FontWeights.Medium;
                case "400":
                case "normal":
                    return FontWeights.Normal;
                case "regular":
                    return FontWeights.Regular;
                case "semibold":
                    return FontWeights.SemiBold;
                case "100":
                case "thin":
                    return FontWeights.Thin;
                case "950":
                case "ultrablack":
                    return FontWeights.UltraBlack;
                case "ultrabold":
                    return FontWeights.UltraBold;
                case "ultralight":
                    return FontWeights.UltraLight;
                default:
                    return FontWeights.Normal;
            }
        }

        /// <summary>
        /// Convert to font stretch
        /// </summary>
        /// <param name="stretchValue"></param>
        /// <returns></returns>
        public static FontStretch getFontStretch(string stretchValue) {
            if (string.IsNullOrWhiteSpace(stretchValue))
                return FontStretches.Normal;
            switch (stretchValue.ToLower().Trim()) {
                case "1":
                case "ultracondensed":
                    return FontStretches.UltraCondensed;
                case "2":
                case "extracondensed":
                    return FontStretches.ExtraCondensed;
                case "3":
                case "condensed":
                    return FontStretches.Condensed;
                case "4":
                case "semicondensed":
                    return FontStretches.SemiCondensed;
                case "5":
                case "medium":
                    return FontStretches.Medium;
                case "6":
                case "semiexpanded":
                    return FontStretches.SemiExpanded;
                case "7":
                case "expanded":
                    return FontStretches.Expanded;
                case "8":
                case "extraexpanded":
                    return FontStretches.ExtraExpanded;
                case "9":
                case "ultraexpanded":
                    return FontStretches.UltraExpanded;
                default:
                    return FontStretches.Normal;
            }
        }

    }
}
