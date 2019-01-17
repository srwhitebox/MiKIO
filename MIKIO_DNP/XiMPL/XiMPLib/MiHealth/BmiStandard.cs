using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiHealth
{
    public class BmiLevel
    {
        public Double Normal { get; set; }
        public Double Over { get; set; }
        public Double Obesity { get; set; }
        public Double Fat { get; set; }
        public Double OverFat { get; set; }

        public BmiLevel(double normal, double over, double obesity, double fat = 0, double overfat = 0)
        {
            this.Normal = normal;
            this.Over = over;
            this.Obesity = obesity;
            this.Fat = fat;
            this.OverFat = overfat;
        }

        public BmiLevel(JObject jLevels)
        {
            this.Normal = jLevels["normal"].ToObject<Double>();
            this.Over = jLevels["over"].ToObject<Double>();
            this.Obesity = jLevels["obesity"].ToObject<Double>();
            if (jLevels["obese"] != null)
                this.Fat = jLevels["obese"].ToObject<Double>();
            else
                this.Fat = 0;
        }

        public int getLevel(Double bmi)
        {
            if (bmi < Normal)
                return -1;
            else if (bmi >= Normal && bmi < Over)
                return 0;
            else if (bmi >= Over && bmi < Obesity)
                return 1;
            else if (Fat == 0 || (bmi >= Obesity && bmi < Fat))
                return 2;
            else
                return 3;
        }
    }

    public class BmiStandard
    {
        public Double Age { get; set; }

 
        public BmiLevel Male{ get; set; }
        public BmiLevel Female { get; set; }

        public int getLevel(String gender, Double bmi)
        {
            if (gender.StartsWith("M"))
                return Male.getLevel(bmi);
            else
                return Female.getLevel(bmi);
        }

        public BmiStandard(JObject jStandard)
        {
            this.Age = Double.Parse(((String)jStandard["age"]).Replace("+", ""));
            this.Male = new BmiLevel((JObject)jStandard["male"]);
            this.Female = new BmiLevel((JObject)jStandard["female"]);
        }
    }
}
