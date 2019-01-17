using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xDevice.xFatAnalyser.Tanita {
    
    public class BC418 : xcSerialDevice {
        public BC418(string portName = "Any")
            : base("Tanita", "BC418", DEVICE_TYPE.FAT_ANALYSER) {
            initPacket();

            this.PortName = portName;
            this.BaudRate = 4800;
            this.Parity = System.IO.Ports.Parity.None;
            this.DataBits = 8;
            this.StopBits = System.IO.Ports.StopBits.One;
        }

        private void initPacket() {
            // "04/05/15","15:52",0,1,00170,078.3,25.9,020.3,058.0,038.9,37,027.1,06837,572,225,229,304,311,23.9,003.5,011.1,010.5,24.0,003.4,010.8,010.2,19.9,000.7,003.0,002.8,20.5,000.7,002.8,002.6,28.3,012.0,030.4,028.9,12
            const string doubleQuotationMark = "double_quotation_mark";
            const string slash = "slash";

            Packet.addElement(xcPacket.KEY_START, 1);       //    "
            Packet.addElement(KEY_DAY, 2);                  //    DD
            Packet.addElement(slash, 1);                    //    /
            Packet.addElement(KEY_MONTH, 2);                //    MM
            Packet.addElement(slash, 1);                    //    /
            Packet.addElement(KEY_YEAR, 2);                 //    YY
            Packet.addElement(doubleQuotationMark, 1);      //   "

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,
            
            Packet.addElement(doubleQuotationMark, 1);      //  "
            Packet.addElement(KEY_HOUR, 2);                 //  HH
            Packet.addElement(KEY_COLON, 1);                //  :
            Packet.addElement(KEY_MINUTE, 2);               //  MM
            Packet.addElement(doubleQuotationMark, 1);      //  "

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(KEY_BODY_TYPE, 1);            //    0:standard, 2 : Athlete

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_GENDER, 1);   //    1:male, 2: female

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_HEIGHT, 5);   //    XXXXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_WEIGHT, 5);   //    XXX.X

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_BODY_FAT_PERCENTAGE, 4); //    XX.X

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_FAT_MASS, 5); //    XXX.X

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_FAT_FREE_MASS, 5); //    XXX.X

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_BODY_WATER_MASS, 5); //    XXX.X

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_AGE, 2);      //    XX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_BMI, 5);      //    XXX.X

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_BMR, 5);      //    XXXXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_IMPEDANCE_WHOLE_BODY, 3); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_IMPEDANCE_RIGHT_LEG, 3); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_IMPEDANCE_LEFT_LEG, 3); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_IMPEDANCE_RIGHT_ARM, 3); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_IMPEDANCE_LEFT_ARM, 3); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_LEG_BODY_FAT_PERCENTAGE, 4); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_LEG_FAT_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_LEG_FAT_FREE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_LEG_MUSCLE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_LEG_BODY_FAT_PERCENTAGE, 4); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_LEG_FAT_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_LEG_FAT_FREE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_LEG_MUSCLE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_ARM_BODY_FAT_PERCENTAGE, 4); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_ARM_FAT_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_ARM_FAT_FREE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_RIGHT_ARM_MUSCLE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_ARM_BODY_FAT_PERCENTAGE, 4); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_ARM_FAT_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_ARM_FAT_FREE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_LEFT_ARM_MUSCLE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_TRUNK_BODY_FAT_PERCENTAGE, 4); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_TRUNK_FAT_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_TRUNK_FAT_FREE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_TRUNK_MUSCLE_MASS, 5); //    XXX

            Packet.addElement(KEY_SEPARATOR, 1);            //    ,

            Packet.addElement(xcFatCompositionRecord.KEY_VISCERAL_FAT_RATING, 2); //    XXX

            Packet.addElement(xcSerialDevice.KEY_CR, 1);    //  0x0D
            Packet.addElement(xcPacket.KEY_END, 1);         //  0x0A

            Packet.Start = (byte)'\"';
            Packet.End = xcSerialDevice.LF;
        }

        public void simulate() {
            byte[] bufferData = { 0x22, 0x32, 0x32, 0x2F, 0x30, 0x35, 0x2F, 0x31, 0x35, 0x22, 0x2C, 0x22, 0x31, 0x36, 0x3A, 0x33, 0x34, 0x22, 0x2C, 0x30, 0x2C, 0x31, 0x2C, 0x30, 0x30, 0x31, 0x36, 0x38, 0x2C, 0x30, 0x36, 0x38, 0x2E, 0x39, 0x2C, 0x31, 0x39, 0x2E, 0x38, 0x2C, 0x30, 0x31, 0x33, 0x2E, 0x36, 0x2C, 0x30, 0x35, 0x35, 0x2E, 0x33, 0x2C, 0x30, 0x33, 0x37, 0x2E, 0x38, 0x2C, 0x32, 0x39, 0x2C, 0x30, 0x32, 0x34, 0x2E, 0x34, 0x2C, 0x30, 0x36, 0x35, 0x32, 0x33, 0x2C, 0x35, 0x36, 0x33, 0x2C, 0x32, 0x32, 0x33, 0x2C, 0x32, 0x34, 0x33, 0x2C, 0x32, 0x38, 0x37, 0x2C, 0x32, 0x38, 0x39, 0x2C, 0x31, 0x39, 0x2E, 0x38, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x36, 0x2C, 0x30, 0x31, 0x30, 0x2E, 0x34, 0x2C, 0x30, 0x30, 0x39, 0x2E, 0x38, 0x2C, 0x32, 0x31, 0x2E, 0x35, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x37, 0x2C, 0x30, 0x30, 0x39, 0x2E, 0x37, 0x2C, 0x30, 0x30, 0x39, 0x2E, 0x32, 0x2C, 0x31, 0x35, 0x2E, 0x34, 0x2C, 0x30, 0x30, 0x30, 0x2E, 0x36, 0x2C, 0x30, 0x30, 0x33, 0x2E, 0x30, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x38, 0x2C, 0x31, 0x36, 0x2E, 0x32, 0x2C, 0x30, 0x30, 0x30, 0x2E, 0x36, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x39, 0x2C, 0x30, 0x30, 0x32, 0x2E, 0x37, 0x2C, 0x31, 0x39, 0x2E, 0x39, 0x2C, 0x30, 0x30, 0x37, 0x2E, 0x33, 0x2C, 0x30, 0x32, 0x39, 0x2E, 0x33, 0x2C, 0x30, 0x32, 0x37, 0x2E, 0x39, 0x2C, 0x30, 0x38, 0x0D, 0x0A };
            Packet.setPacketData(bufferData, bufferData.Length);

            OnPacketReceived();
        }
    }
}
