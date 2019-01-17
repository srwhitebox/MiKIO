using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using XiMPLib.xType;

namespace XiMPLib.xDevice.xFatAnalyser {
    public class xcFatCompositionRecord : INotifyPropertyChanged {
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
        public const string KEY_RIGHT_LEG_MUSCLE_MASS = "right_leg_predicted_muscle_mass";
        public const string KEY_LEFT_LEG_BODY_FAT_PERCENTAGE = "left_leg_body_fat_percentage";
        public const string KEY_LEFT_LEG_FAT_MASS = "left_leg_fat_mass";
        public const string KEY_LEFT_LEG_FAT_FREE_MASS = "left_leg_fat_free_mass";
        public const string KEY_LEFT_LEG_MUSCLE_MASS = "left_leg_predicted_muscle_mass";
        public const string KEY_RIGHT_ARM_BODY_FAT_PERCENTAGE = "right_arm_body_fat_percentage";
        public const string KEY_RIGHT_ARM_FAT_MASS = "right_arm_fat_mass";
        public const string KEY_RIGHT_ARM_FAT_FREE_MASS = "right_arm_fat_free_mass";
        public const string KEY_RIGHT_ARM_MUSCLE_MASS = "right_arm_predicted_muscle_mass";
        public const string KEY_LEFT_ARM_BODY_FAT_PERCENTAGE = "left_arm_body_fat_percentage";
        public const string KEY_LEFT_ARM_FAT_MASS = "left_arm_fat_mass";
        public const string KEY_LEFT_ARM_FAT_FREE_MASS = "left_arm_fat_free_mass";
        public const string KEY_LEFT_ARM_MUSCLE_MASS = "left_arm_predicted_muscle_mass";
        public const string KEY_TRUNK_BODY_FAT_PERCENTAGE = "trunk_body_fat_percentage";
        public const string KEY_TRUNK_FAT_MASS = "trunk_fat_mass";
        public const string KEY_TRUNK_FAT_FREE_MASS = "trunk_fat_free_mass";
        public const string KEY_TRUNK_MUSCLE_MASS = "trunk_predicted_muscle_mass";
        public const string KEY_VISCERAL_FAT_RATING = "visceral_fat_rating";

        public IntRange[] AgeRangeTable = new IntRange[]{
            new IntRange(18, 39),
            new IntRange(40, 59),
            new IntRange(60, int.MaxValue)
        };

        public FloatRange[] DesirableMaleBodyFatPercentageTable = new FloatRange[] {
            new FloatRange(11f, 21.9f),
            new FloatRange(12f, 22.9f),
            new FloatRange(14f, 24.9f)
        };

        public FloatRange[] DesirableFemaleBodyFatPercentageTable = new FloatRange[] {
            new FloatRange(21f, 34.9f),
            new FloatRange(22f, 35.9f),
            new FloatRange(23f, 36.9f)
        };

        public DateTime MeasuredAt {
            get;
            set;
        }

        public char BodyType {
            get;
            set;
        }

        public char Gender {
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

        public float BodyFatPercentage {
            get;
            set;
        }

        public float FatMass {
            get;
            set;
        }

        public float FatFreeMass {
            get;
            set;
        }

        public float BodyWaterMass {
            get;
            set;
        }

        public int Age {
            get {
                return BodyAge;
            }
        }

        public int BodyAge {
            get;
            set;
        }

        public float BMI {
            get;
            set;
        }

        public float BMR {
            get;
            set;
        }

        public int BmrByKJ {
            get {
                return (int)BMR;
            }
        }

        public float BmrByKCal {
            get {
                return (int)(BMR / 4.184f);
            }
        }

        public float ImpedanceWholeBody {
            get;
            set;
        }

        public float ImpedanceRightLeg {
            get;
            set;
        }

        public float ImpedanceLeftLeg {
            get;
            set;
        }

        public float ImpedanceRightArm {
            get;
            set;
        }

        public float ImpedanceLeftArm {
            get;
            set;
        }

        public float RightLegBodyFatPercentage {
            get;
            set;
        }

        public float RightLegFatMass {
            get;
            set;
        }

        public float RightLegFatFreeMass {
            get;
            set;

        }
        public float RightLegMuscleMass {
            get;
            set;
        }

        public float LeftLegBodyFatPercentage {
            get;
            set;
        }

        public float LeftLegFatMass {
            get;
            set;
        }

        public float LeftLegFatFreeMass {
            get;
            set;

        }
        public float LeftLegMuscleMass {
            get;
            set;
        }

        public float RightArmBodyFatPercentage {
            get;
            set;
        }

        public float RightArmFatMass {
            get;
            set;
        }

        public float RightArmFatFreeMass {
            get;
            set;

        }
        public float RightArmMuscleMass {
            get;
            set;
        }

        public float LeftArmBodyFatPercentage {
            get;
            set;
        }

        public float LeftArmFatMass {
            get;
            set;
        }

        public float LeftArmFatFreeMass {
            get;
            set;

        }
        public float LeftArmMuscleMass {
            get;
            set;
        }

        public float TrunkBodyFatPercentage {
            get;
            set;
        }

        public float TrunkFatMass {
            get;
            set;
        }

        public float TrunkFatFreeMass {
            get;
            set;

        }
        public float TrunkMuscleMass {
            get;
            set;
        }

        public float VisceralFatRaiting {
            get;
            set;
        }

        private int AgeRangeIndex {
            get {
                for (int i = 0; i < AgeRangeTable.Length; i++) {
                    if (AgeRangeTable[i].contains(this.Age))
                        return i;
                }
                return -1;
            }
        }

        public FloatRange DesirableBodyFatPercentageRange {
            get {
                int index = AgeRangeIndex;
                if (index == -1)
                    return new FloatRange(-1, -1);
                else
                    return Gender == '1' ? DesirableMaleBodyFatPercentageTable[index] : DesirableFemaleBodyFatPercentageTable[index];
            }
        }

        public FloatRange DesirableFatMassRange {
            get {
                return new FloatRange(getDesiribleWeight(DesirableBodyFatPercentageRange.Start), getDesiribleWeight(DesirableBodyFatPercentageRange.End));
            }
        }

        public string Comment {
            get {
                if (Height == 0 || Weight == 0)
                    return string.Empty;
                string[] comment = { "體重過輕", "正常", "過重", "輕度肥胖", "中度肥胖", "重度肥胖" };
                int index = 0;
                switch (Gender) {
                    case 'M':
                        if (BMI < 18.5)
                            index = 0;
                        else if (BMI < 23)
                            index = 1;
                        else if (BMI < 27)
                            index = 2;
                        else if (BMI < 30)
                            index = 3;
                        else if (BMI < 35)
                            index = 4;
                        else 
                            index = 5;
                        break;
                    case 'F':
                        if (BMI < 18.5)
                            index = 0;
                        else if (BMI < 24)
                            index = 1;
                        else if (BMI < 27)
                            index = 2;
                        else if (BMI < 30)
                            index = 3;
                        else if (BMI < 35)
                            index = 4;
                        else 
                            index = 5;
                        break;
                    default:
                        break;
                }
                return comment[index];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public xcFatCompositionRecord(char gender = 'M') {
            this.Gender = gender;
        }

        public xcFatCompositionRecord(float height, float weight, char gender = 'M') {
            this.MeasuredAt = DateTime.Now;
            this.Height = height;
            this.Weight = weight;
            this.Gender = gender;
        }

        public xcFatCompositionRecord(xcPacket packet) {
            // MeasuredAt = new DateTime(packet.getInt(xcSerialDevice.KEY_YEAR)+2000, packet.getInt(xcSerialDevice.KEY_MONTH), packet.getInt(xcSerialDevice.KEY_DAY));
            MeasuredAt = DateTime.Now;
            BodyType = packet.getChar(xcSerialDevice.KEY_BODY_TYPE); 
            Gender = packet.getChar(KEY_GENDER);
            Height = packet.getFloat(KEY_HEIGHT);
            Weight = packet.getFloat(KEY_WEIGHT);
            BodyFatPercentage = packet.getFloat(KEY_BODY_FAT_PERCENTAGE);
            FatMass = packet.getFloat(KEY_FAT_MASS);
            FatFreeMass = packet.getFloat(KEY_FAT_FREE_MASS);
            BodyWaterMass = packet.getFloat(KEY_BODY_WATER_MASS);
            BodyAge = packet.getInt(KEY_AGE);
            BMI = packet.getFloat(KEY_BMI);
            BMR = packet.getFloat(KEY_BMR);

            ImpedanceWholeBody = packet.getFloat(KEY_IMPEDANCE_WHOLE_BODY);
            ImpedanceRightLeg = packet.getFloat(KEY_IMPEDANCE_RIGHT_LEG);
            ImpedanceLeftLeg = packet.getFloat(KEY_IMPEDANCE_LEFT_LEG);
            ImpedanceRightArm = packet.getFloat(KEY_IMPEDANCE_RIGHT_ARM);
            ImpedanceLeftArm = packet.getFloat(KEY_IMPEDANCE_LEFT_ARM);

            RightLegBodyFatPercentage = packet.getFloat(KEY_RIGHT_LEG_BODY_FAT_PERCENTAGE);
            RightLegFatMass = packet.getFloat(KEY_RIGHT_LEG_FAT_MASS);
            RightLegFatFreeMass = packet.getFloat(KEY_RIGHT_LEG_FAT_FREE_MASS);
            RightLegMuscleMass = packet.getFloat(KEY_RIGHT_LEG_MUSCLE_MASS);

            LeftLegBodyFatPercentage = packet.getFloat(KEY_LEFT_LEG_BODY_FAT_PERCENTAGE);
            LeftLegFatMass = packet.getFloat(KEY_LEFT_LEG_FAT_MASS);
            LeftLegFatFreeMass = packet.getFloat(KEY_LEFT_LEG_FAT_FREE_MASS);
            LeftLegMuscleMass = packet.getFloat(KEY_LEFT_LEG_MUSCLE_MASS);

            RightArmBodyFatPercentage = packet.getFloat(KEY_RIGHT_ARM_BODY_FAT_PERCENTAGE);
            RightArmFatMass = packet.getFloat(KEY_RIGHT_ARM_FAT_MASS);
            RightArmFatFreeMass = packet.getFloat(KEY_RIGHT_ARM_FAT_FREE_MASS);
            RightArmMuscleMass = packet.getFloat(KEY_RIGHT_ARM_MUSCLE_MASS);

            LeftArmBodyFatPercentage = packet.getFloat(KEY_LEFT_ARM_BODY_FAT_PERCENTAGE);
            LeftArmFatMass = packet.getFloat(KEY_LEFT_ARM_FAT_MASS);
            LeftArmFatFreeMass = packet.getFloat(KEY_LEFT_ARM_FAT_FREE_MASS);
            LeftArmMuscleMass = packet.getFloat(KEY_LEFT_ARM_MUSCLE_MASS);

            TrunkBodyFatPercentage = packet.getFloat(KEY_TRUNK_BODY_FAT_PERCENTAGE);
            TrunkFatMass = packet.getFloat(KEY_TRUNK_FAT_MASS);
            TrunkFatFreeMass = packet.getFloat(KEY_TRUNK_FAT_FREE_MASS);
            TrunkMuscleMass = packet.getFloat(KEY_TRUNK_MUSCLE_MASS);

            VisceralFatRaiting = packet.getInt(KEY_VISCERAL_FAT_RATING);
        }

        public void copyFrom(xcFatCompositionRecord record) {
            if (record == null)
                this.clear();
            else
                xcObject.copyProperties(this, record);

            notifyChanged();
        }

        public void clear() {
            this.Height = 0;
            this.Weight = 0;
            BodyFatPercentage = 0;
            FatMass = 0;
            FatFreeMass = 0;
            BodyWaterMass = 0;
            BodyAge = 0;
            BMI = 0;
            BMR = 0;

            notifyChanged();
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
        }

        public override string ToString() {
            JObject jBpInfo = new JObject();
            jBpInfo.Add("height", Height);
            jBpInfo.Add("weight", Weight);
            jBpInfo.Add("body_fat_percentage", BodyFatPercentage);
            jBpInfo.Add("fat_mass", FatMass);
            jBpInfo.Add("fat_free_mass", FatFreeMass);
            jBpInfo.Add("body_water_mass", BodyWaterMass);
            jBpInfo.Add("body_age", BodyAge);
            jBpInfo.Add("bmi", BMI);
            jBpInfo.Add("bmr", BMR);
            jBpInfo.Add("comment", Comment);

            return jBpInfo.ToString();
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        private float getDesiribleWeight(float percentage) {
            var percentValue = percentage / 100;
            return (FatFreeMass * percentValue) / (1 - percentValue);
        }
    }
}
