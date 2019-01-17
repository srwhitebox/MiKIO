using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;

namespace XiMPLib.xType {
    public class xcPaperMargins : Margins {
        /// <summary>
        /// Consrutuctor
        /// </summary>
        /// <param name="margins"></param>
        public xcPaperMargins(string margins) {
            string[] tokens = margins.Split(',');
            if (tokens.Length == 1) {         // All margin is same
                Left = Right = Top = Bottom = new xcLength(tokens[0]).PageLength;
            } else if (tokens.Length == 2) {    // First parameter is left, right margins, Second parameter is Top, Bottom margins
                Left = Right = new xcLength(tokens[0]).PageLength;
                Top = Bottom = new xcLength(tokens[1]).PageLength;
            } else if (tokens.Length == 3) {    // Over 3 tokesn,, left, right, top, bottom margins.
                Left = new xcLength(tokens[0]).PageLength;
                Right = new xcLength(tokens[1]).PageLength;
                Top = Bottom = new xcLength(tokens[2]).PageLength;
            } else if (tokens.Length == 4) {
                Left = new xcLength(tokens[0]).PageLength;
                Right = new xcLength(tokens[1]).PageLength;
                Top = new xcLength(tokens[2]).PageLength;
                Bottom = new xcLength(tokens[3]).PageLength;
            }
        }
    }
}
