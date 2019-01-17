using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcLandolf {
        public static double TangentSpace = Math.Tan(1f / 120f) ;
        public static double ArcMin = 2.90888 / 10000;

        public double Distance
        {
            get; set;
        }

        public xcLandolf() {
        }

        public void setDistanceByMeter(double meter) {
            this.Distance = meter * 1000;
        }

        public void setDistanceByCm(double centiMeter) {
            this.Distance = centiMeter * 100;
        }

        public void setDistanceByMiliMeter(double miliMeter) {
            this.Distance = miliMeter;
        }

        public double getHeight(double visionValue) {
            return ArcMin * visionValue * this.Distance * 5;
        }

        public double getHeightByPixel(double visionValue, double ppi) {
            double ppm = ppi / 25.4;

            return getHeight(visionValue) * ppm;
        }

    }
}
