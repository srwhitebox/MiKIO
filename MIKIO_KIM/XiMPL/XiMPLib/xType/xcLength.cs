using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    /// <summary>
    /// Length Class
    /// </summary>
    public class xcLength : xcNumber {
        /// <summary>
        /// Default DPI
        /// </summary>
        private const int DEFAULT_DPI = 180;

        /// <summary>
        /// Unit Enumeration
        /// </summary>
        public enum UNIT{ 
            PIXEL,
            POINT,
            PICA,
            DP,
            SP,
            MM,
            CM,
            M,
            KM,
            FEET,
            INCH,
            LEAGUE,
            MILE,
            YARD,
            UNKOWN,
        }

        /// <summary>
        /// Retur Inch value
        /// </summary>
        public double Inch{
            get{
                const double cmToInch = 1 / 2.54;
                const double mToInch = cmToInch * 100;
                const double kmToInch = mToInch * 1000;
                const double mmToInch = cmToInch / 10;

                switch(Unit){
                    case UNIT.INCH:
                        return Double;
                    case UNIT.KM:
                        return Double * kmToInch;
                    case UNIT.M:
                        return Double * mToInch;
                    case UNIT.CM:
                        return Double * cmToInch;
                    case UNIT.MM:
                        return Double * mmToInch;
                    case UNIT.FEET:
                        return Double * 12;
                    case UNIT.LEAGUE:
                        return Double * 190080.38189;
                    case UNIT.MILE:
                        return Double * 63360;
                    case UNIT.YARD:
                        return Double * 36;
                    case UNIT.POINT:
                        return Double / 72;     // 1 point = 1/72 inches
                    case UNIT.PICA:
                        return Double * 12 / 72; // 1 pc = 12 points
                    case UNIT.DP:
                        return Double / 160;     // 1 DP = 1/160 inch.
                    case UNIT.SP:
                        return Double * 10;
                    case UNIT.PIXEL:
                        return Double / DPI;
                    default :
                        return 0;
                }
            }
        }

        public int PageLength {
            get {
                return (int)Math.Round(Inch * 100);
            }
        }

        /// <summary>
        /// Get / Set unit
        /// </summary>
        public UNIT Unit {
            get;
            set;
        }

        /// <summary>
        /// Get / Set DPI
        /// </summary>
        public double DPI {
            get;
            set;
        }

        /// <summary>
        /// Get Pixels
        /// </summary>
        public int Pixel {
            get {
                return (int)Math.Round(Inch * DPI);
            }
        }

        public double Point {
            get {
                return Math.Round(Inch * 72);
            }
        }

        public double MiliMeter
        {
            get
            {
                switch (Unit) {
                    case UNIT.M:
                        return this.Double * 1000;
                    case UNIT.MM:
                        return this.Double;
                    case UNIT.CM:
                        return this.Double * 10;
                    case UNIT.KM:
                        return this.Double * 1000 * 1000;
                    case UNIT.INCH:
                        return this.Double * 25.4;
                    case UNIT.FEET:
                        return this.Double * 304.8;
                    case UNIT.MILE:
                        return this.Double * 1609340;
                    case UNIT.YARD:
                        return this.Double * 914.4;
                }
                return this.Inch * 25.4;
            }

        }

        public xcLength(String length, UNIT unit=UNIT.INCH, double dpi = DEFAULT_DPI)
            : base(length) {
                if (string.IsNullOrEmpty(UnitText))
                    Unit = unit;
                else
                    Unit = getUnit(UnitText);
            DPI = dpi;
        }

        public bool hasLength() {
            return Double > 0;
        }

        /// <summary>
        /// Get Unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private UNIT getUnit(String unit) {
            switch (unit.ToLower()) {
                case "km":
                case "kilometer":
                    return UNIT.KM;
                case "m":
                case "meter":
                    return UNIT.M;
                case "cm":
                case "centimeter":
                    return UNIT.CM;
                case "mm":
                case "milimeter":
                    return UNIT.MM;
                case "ft":
                case "\'":
                case "feet":
                    return UNIT.FEET;
                case "league":
                    return UNIT.LEAGUE;
                case "mi":
                case "mile":
                    return UNIT.MILE;
                case "yd":
                case "yard":
                    return UNIT.YARD;
                case "in":
                case "\"":
                case "inch":
                    return UNIT.INCH;
                case "pt":
                case "point":
                    return UNIT.POINT;
                case "pc":
                case "pica":
                    return UNIT.PICA;
                case "dp":
                case "dip":
                    return UNIT.DP;
                case "sp":
                case "sip":
                    return UNIT.SP;
                case "pixel":
                case "px":
                    return UNIT.PIXEL;
                default:
                    return UnitText == string.Empty ? UNIT.PIXEL : UNIT.UNKOWN;
            }
        }
    }
}
