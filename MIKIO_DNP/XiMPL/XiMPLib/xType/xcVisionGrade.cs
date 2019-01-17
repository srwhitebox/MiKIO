using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public enum VisualAcuityType
    {

    }

    public class xcVisionGrade {
        public String Degree2Min { get; set; }
        public String Degree5Min { get; set; }

        public double Degree { get; set; }

        public int PresentNumber { get; set; }

        public xcVisionGrade(String grade2, String grade5, double degree, int presentNumber) {
            this.Degree2Min = grade2;
            this.Degree5Min = grade5;
            this.Degree = degree;
            this.PresentNumber = presentNumber;
        }


        public xcVisionGrade(String grade2, String grade5, String logMar, double degree, int presentNumber)
        {
            this.Degree2Min = grade2;
            this.Degree5Min = grade5;
            this.Degree = degree;
            this.PresentNumber = presentNumber;
        }

        public static double LogMarFromDecimaGrade(double decimalGrade)
        {
            return -Math.Log10(decimalGrade);
        }

        public static double To5MinsGrade(double decimalGrade)
        {
            return 5.0 - LogMarFromDecimaGrade(decimalGrade);
        }

        public static double ToDecimalNotation(double arc5Mins)
        {
            return Math.Pow(10, -(5.0 - arc5Mins));            
        }
    }
}
