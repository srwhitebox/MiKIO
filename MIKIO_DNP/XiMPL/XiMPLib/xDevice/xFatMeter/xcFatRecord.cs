using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using XiMPLib.xType;

namespace XiMPLib.xDevice.xFatMeter {
    public class xcFatRecord : INotifyPropertyChanged {
        public const string KEY_HEIGHT = "height";
        public const string KEY_WEIGHT = "weight";
        public const string KEY_BODY_FAT_PERCENTAGE = "body_fat_percentage";
        public const string KEY_FAT_MASS = "fat_mass";
        public const string KEY_FAT_FREE_MASS = "fat_free_mass";
        public const string KEY_BODY_WATER_MASS= "body_water_mass";
        public const string KEY_AGE = "age";
        public const string KEY_BMI = "bmi";
        public const string KEY_BMR = "bmr";
        public const string KEY_OBESITY = "obesity";
        public const string KEY_GENDER = "gender";

        public const string KEY_IMPEDANCE_WHOLE_BODY = "impedance_whole_body";
        public const string KEY_IMPEDANCE_RIGHT_LEG = "impedance_right_leg";
        public const string KEY_IMPEDANCE_LEFT_LEG = "impedance_left_leg";
        public const string KEY_IMPEDANCE_RIGHT_ARM = "impedance_right_arm";
        public const string KEY_IMPEDANCE_LEFT_ARM = "impedance_left_arm";
        public const string KEY_RIGHT_LEG_BODY_FAT_PERCENTAGE = "right_leg_body_fat_percentage";
        public const string KEY_RIGHT_LEG_FAT_MASS = "right_leg_fat_mass";
        public const string KEY_RIGHT_LEG_FAT_FREE_MASS = "right_leg_fat_free_mass";
        public const string KEY_RIGHT_LEG_PREDICTED_MUSCLE_MASS = "right_leg_predicted_muscle_mass";
        public const string KEY_LEFT_LEG_BODY_FAT_PERCENTAGE = "left_leg_body_fat_percentage";
        public const string KEY_LEFT_LEG_FAT_MASS = "left_leg_fat_mass";
        public const string KEY_LEFT_LEG_FAT_FREE_MASS = "left_leg_fat_free_mass";
        public const string KEY_LEFT_LEG_PREDICTED_MUSCLE_MASS = "left_leg_predicted_muscle_mass";
        public const string KEY_RIGHT_ARM_BODY_FAT_PERCENTAGE = "right_arm_body_fat_percentage";
        public const string KEY_RIGHT_ARM_FAT_MASS = "right_arm_fat_mass";
        public const string KEY_RIGHT_ARM_FAT_FREE_MASS = "right_arm_fat_free_mass";
        public const string KEY_RIGHT_ARM_PREDICTED_MUSCLE_MASS = "right_arm_predicted_muscle_mass";
        public const string KEY_LEFT_ARM_BODY_FAT_PERCENTAGE = "left_arm_body_fat_percentage";
        public const string KEY_LEFT_ARM_FAT_MASS = "left_arm_fat_mass";
        public const string KEY_LEFT_ARM_FAT_FREE_MASS = "left_arm_fat_free_mass";
        public const string KEY_LEFT_ARM_PREDICTED_MUSCLE_MASS = "left_arm_predicted_muscle_mass";
        public const string KEY_TRUNK_BODY_FAT_PERCENTAGE = "trunk_body_fat_percentage";
        public const string KEY_TRUNK_FAT_MASS = "trunk_fat_mass";
        public const string KEY_TRUNK_FAT_FREE_MASS = "trunk_fat_free_mass";
        public const string KEY_TRUNK_PREDICTED_MUSCLE_MASS = "trunk_predicted_muscle_mass";
        public const string KEY_VISCERAL_FAT_RATING = "visceral_fat_rating";
        public DateTime MeasuredAt {
            get;
            set;
        }

        public float Height {
            get;
            set;
        }

        public float Weight {
            get;
            set;
        }

        public char Gender {
            get;
            set;
        }

        public int BmiLevel
        {
            get
            {
                return xBinding.xcBinder.MiHealth.getBmiLevel(this);
            }
        }

        public string Comment {
            get {
                if (Height == 0 || Weight == 0)
                    return string.Empty;
                switch (BmiLevel)
                {
                    case -1:
                        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["underweight"];
                    case 0:
                        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["healthy"];
                    case 1:
                        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["overweight"];
                    case 2:
                        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["obese"];
                    case 3:
                        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["fat"];
                    case 4:
                        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["overfat"];
                    default:
                        return "";
                }
                //switch (Gender) {
                //    case 'M':
                //        if (BMI < 18.5)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["underweight"];
                //        else if (BMI < 23)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["healthy"];
                //        else if (BMI < 27)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["overweight"];
                //        else if (BMI < 30)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["obese"];
                //        else if (BMI < 35)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["fat"];
                //        else
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["overfat"];
                //    case 'F':
                //        if (BMI < 18.5)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["underweight"];
                //        else if (BMI < 24)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["healthy"];
                //        else if (BMI < 27)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["overweight"];
                //        else if (BMI < 30)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["obese"];
                //        else if (BMI < 35)
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["fat"];
                //        else
                //            return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["overfat"];
                //    default:
                //        return XiMPLib.xBinding.xcBinder.AppProperties.FatComments["healthy"];
                //}
            }
        }

        /// <summary>
        /// Return BMI
        /// BMI = Weight in Kg / (Height in Meter * Height in Meter)
        /// </summary>
        public float BMI {
            get {
                return Weight == 0 || Height == 0 ? 0 : (float)Math.Round(Weight / (Height/100 * Height/100), 1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public xcFatRecord(char gender = 'M') {
            this.Gender = gender;
        }

        public xcFatRecord(float height, float weight, char gender = 'M') {
            this.MeasuredAt = DateTime.Now;
            this.Height = height;
            this.Weight = weight;
            this.Gender = gender;
        }

        public void copyFrom(xcFatRecord record) {
            if (record == null)
                this.clear();
            else
                xcObject.copyProperties(this, record);
            //this.Height = record.Height;
            //this.Weight = record.Weight;
            //this.Obesity = record.Obesity;

            notifyChanged();
        }

        public void clear() {
            this.Height = 0;
            this.Weight = 0;

            notifyChanged();
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }

            //OnPropertyChanged("Height");
            //OnPropertyChanged("Weight");
            //OnPropertyChanged("BMI");
            //OnPropertyChanged("Comment");
        }

        public override string ToString() {
            JObject jBpInfo = new JObject();
            jBpInfo.Add("height", Height);
            jBpInfo.Add("weight", Weight);
            jBpInfo.Add("bmi", BMI);
            jBpInfo.Add("comment", Comment);

            return jBpInfo.ToString();
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
