using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XiMPLib.xType {
    public class xcThickness{
        private Thickness mThickness;

        public double Left {
            get {
                return mThickness.Left;
            }
            set{
                mThickness.Left = value;
            }
        }

        public double Top {
            get {
                return mThickness.Top;
            }
            set {
                mThickness.Top = value;
            }
        }
        public double Right {
            get {
                return mThickness.Right;
            }
            set {
                mThickness.Right = value;
            }
        }

        public double Bottom {
            get {
                return mThickness.Bottom;
            }
            set {
                mThickness.Bottom = value;
            }
        }

        public Thickness Thickness {
            get {
                return mThickness;
            }
        }

        /// <summary>
        /// Consrutuctor
        /// </summary>
        /// <param name="margins"></param>
        public xcThickness(string margins) {
            mThickness = new Thickness();
            string[] tokens = margins.Split(',');
            if (tokens.Length == 1) {         // All margin is same

                mThickness.Left = mThickness.Right = mThickness.Top = mThickness.Bottom = xcString.toFloat(tokens[0]);
            } else if (tokens.Length == 2) {    // First parameter is left, right margins, Second parameter is Top, Bottom margins
                mThickness.Left = mThickness.Right = xcString.toFloat(tokens[0]);
                mThickness.Top = mThickness.Bottom = xcString.toFloat(tokens[1]);
            } else if (tokens.Length == 3) {    // Over 3 tokesn,, left, right, top, bottom margins.
                mThickness.Left = xcString.toFloat(tokens[0]);
                mThickness.Right = xcString.toFloat(tokens[1]);
                mThickness.Top = mThickness.Bottom = xcString.toFloat(tokens[2]);
            } else if (tokens.Length == 4) {
                mThickness.Left = xcString.toFloat(tokens[0]);
                mThickness.Top = xcString.toFloat(tokens[1]);
                mThickness.Right = xcString.toFloat(tokens[2]);
                mThickness.Bottom = xcString.toFloat(tokens[3]);
            }
        }
    }
}
