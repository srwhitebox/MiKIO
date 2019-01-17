using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xBinding;

namespace XiMPLib.xType
{
    /// <summary>
    /// Visual Acuity
    /// </summary>
    public class xcVisualAcuity
    {
        /// <summary>
        /// Notation Types
        /// </summary>
        public enum NOTATION
        {
            DECIMAL,
            LOG_MAR,
            MIN5,
            SNELLEN_4M,
            SNELLEN_6M,
            SNELLEN_20FT,

            UNKNOWN,
        }

        /// <summary>
        /// Get/set logMAR
        /// </summary>
        public Double LogMar
        {
            get;set;
        }

        /// <summary>
        /// Get decimal grade
        /// </summary>
        public Double Decimal
        {
            get
            {
                return ToDecimal(NOTATION.LOG_MAR, LogMar);
            }
        }

        /// <summary>
        /// Get Arc 5 Minutes grade
        /// </summary>
        public Double Min5
        {
            get
            {
                return ToMin5(NOTATION.LOG_MAR, LogMar);
            }
        }

        /// <summary>
        /// Get Metre snellen grade
        /// </summary>
        public String Snellen4M
        {
            get
            {
                return ToSnellen4M(NOTATION.LOG_MAR, LogMar);
            }
        }

        /// <summary>
        /// Get Metre snellen grade
        /// </summary>
        public String Snellen6M
        {
            get
            {
                return ToSnellen6M(NOTATION.LOG_MAR, LogMar);
            }
        }

        /// <summary>
        /// Get Foot snellen grade
        /// </summary>
        public String Snellen20ft
        {
            get
            {
                return ToSnellen20ft(NOTATION.LOG_MAR, LogMar);
            }
        }

        public int Width
        {
            get
            {
                double distanceByinches = 1 / 25.4 * xcBinder.AppProperties.VisionTestDistance;
                double widthByInches = GetWidth(distanceByinches); // 2 * distanceByinches * Math.Tan(Math.PI / 180 * (1 / (Decimal * 120))) * 5;

                return Convert.ToInt32(Math.Round(widthByInches * xcBinder.AppProperties.DisplayPPI));
            }
        }

        public String String
        {
            get
            {
                switch (xcBinder.AppProperties.VisionTestNotation)
                {
                    case NOTATION.DECIMAL:
                        return DecimalString;
                    case NOTATION.MIN5:
                    case NOTATION.LOG_MAR:
                        return Min5.ToString("F1");
                    case NOTATION.SNELLEN_20FT:
                        return Snellen20ft;
                    case NOTATION.SNELLEN_4M:
                        return Snellen4M;
                    case NOTATION.SNELLEN_6M:
                        return Snellen6M;
                    default:
                        return "";
                }
            }
        }

        public String DecimalString
        {
            get
            {
                return Decimal.ToString("F3");
            }
        }

        /// <summary>
        /// Constructor with logMAR grade
        /// </summary>
        /// <param name="logMarGrade"></param>
        public xcVisualAcuity(double logMarGrade)
        {
            LogMar = logMarGrade;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        public xcVisualAcuity(NOTATION notation, double grade)
        {
            LogMar = ToLogMar(notation, grade);
        }

        /// <summary>
        /// Constructor with snellen grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        public xcVisualAcuity(NOTATION notation, String grade)
        {
            LogMar = ToLogMar(notation, grade);
        }

        public double GetWidth(double distance)
        {
            return 2 * distance * Math.Tan(Math.PI / 180 * (1 / (Decimal * 120))) * 5;
        }

        /// <summary>
        /// Convert to logMAR
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double ToLogMar(NOTATION notation, double grade)
        {
            switch (notation)
            {
                case NOTATION.DECIMAL:
                case NOTATION.SNELLEN_4M:
                case NOTATION.SNELLEN_6M:
                case NOTATION.SNELLEN_20FT:
                    return -Math.Log10(grade);
                case NOTATION.LOG_MAR:
                    return grade;
                case NOTATION.MIN5:
                    return 5 - grade;

            }

            return -1;            
        }

        /// <summary>
        /// Convert to logMAR
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double ToLogMar(NOTATION notation, String grade)
        {
            Double decimalGrade = -1;
            String[] tokens = grade.Split('/');
            if (tokens.Length == 1)
            {
                if (!Double.TryParse(grade, out decimalGrade))
                {
                    return -1;
                }
                if (decimalGrade != -1)
                    return -Math.Log10(decimalGrade); 
            }

            if (tokens.Length != 2)
                return -1;

            decimalGrade = Double.Parse(tokens[0]) / Double.Parse(tokens[1]);
            switch (notation)
            {
                case NOTATION.SNELLEN_4M:
                case NOTATION.SNELLEN_6M:
                case NOTATION.SNELLEN_20FT:
                    return -Math.Log10(decimalGrade);
            }

            return -1;
        }

        /// <summary>
        /// Convert to Arc 5 minutes 
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double ToMin5(NOTATION notation, double grade)
        {
            return 5 - ToLogMar(notation, grade);
        }

        /// <summary>
        /// Convert to Arc 5 minutes 
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double ToMin5(NOTATION notation, String grade)
        {
            return 5 - ToLogMar(notation, grade);
        }

        /// <summary>
        /// To Decimal grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double ToDecimal(NOTATION notation, double grade)
        {
            return Math.Pow(10, -(ToLogMar(notation, grade)));
        }

        /// <summary>
        /// To Decimal grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double ToDeciamlGrade(NOTATION notation, String grade)
        {
            return Math.Pow(10, -(ToLogMar(notation, grade)));
        }

        /// <summary>
        /// To metre grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static String ToSnellen4M(NOTATION notation, double grade)
        {
            return "4/" + Denominator(4, notation, grade);
        }

        /// <summary>
        /// To metre grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static String ToSnellen4M(NOTATION notation, String grade)
        {
            return "4/" + Denominator(4, notation, grade);
        }

        /// <summary>
        /// To metre grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static String ToSnellen6M(NOTATION notation, double grade)
        {
            return "6/" + Denominator(6, notation, grade);
        }

        /// <summary>
        /// To metre grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static String ToSnellen6M(NOTATION notation, String grade)
        {
            return "6/" + Denominator(6, notation, grade);
        }

        /// <summary>
        /// To foot grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static String ToSnellen20ft(NOTATION notation, double grade)
        {
            return "20/" + Denominator(20, notation, grade); ;
        }

        /// <summary>
        /// To foot grade
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static String To20Feet(NOTATION notation, String grade)
        {
            return "20/" + Denominator(20, notation, grade); ;
        }

        /// <summary>
        /// Conver to snellon grade
        /// </summary>
        /// <param name="nominator"></param>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        private static String Denominator(int nominator, NOTATION notation, double grade)
        {
            double decimalGrade = ToDecimal(notation, grade);
            return Denominator(nominator, decimalGrade);
        }

        /// <summary>
        /// Conver to snellon grade
        /// </summary>
        /// <param name="nominator"></param>
        /// <param name="notation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        private static String Denominator(int nominator, NOTATION notation, String grade)
        {
            double decimalGrade = ToDeciamlGrade(notation, grade);
            return Denominator(nominator, decimalGrade);
        }

        /// <summary>
        /// Conver to snellon grade
        /// </summary>
        /// <param name="nominator"></param>
        /// <param name="decimalGrade"></param>
        /// <returns></returns>
        private static String Denominator(int nominator, double decimalGrade)
        {
            double denominator = nominator / decimalGrade;
            String denominatorStr = denominator.ToString("F1");
            return denominatorStr.EndsWith(".0") ? denominatorStr.Substring(0, denominatorStr.Length - 2) : denominatorStr;
        }
    }
}
