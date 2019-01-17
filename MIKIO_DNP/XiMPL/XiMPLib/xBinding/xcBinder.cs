using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using XiMPLib.xSystem;
using XiMPLib.xDevice.xBpMeter;
using XiMPLib.xDevice.xFatMeter;
using XiMPLib.xDevice;
using System.IO.Ports;
using MitacHis;
using MitacHis.Database;
using System.Timers;
using System.Windows.Controls;
using XiMPLib.xDevice.xCardReader;
using XiMPLib.MiKIO;
using XiMPLib.xDevice.xBpMeter.Fora;
using XiMPLib.MiPHS;
using XiMPLib.xType;
using System.Windows;
using XiMPLib.xUI;
using System.Windows.Threading;
using XiMPLib.xDevice.xFatAnalyser;
using WXiMPLib.xChart;
using XiMPLib.MiHIS;
using XiMPLib.xDocument;
using XiMPLib.MiCampus;
using XiMPLib.MiHealth;
using XiMPLib.xNetwork;
using Newtonsoft.Json.Linq;

namespace XiMPLib.xBinding {
    public static class xcBinder {
        public static AppProperties AppProperties = new AppProperties();
        public static xcPhs MiPhs = new xcPhs();
        public static xcHis MiHIS = new xcHis();
        public static xcMiHealth MiHealth = new xcMiHealth();
        public static MiHealthClient mihealthClient = new MiHealthClient();
        public static xcMiHealthBoard miHealthBoard = new xcMiHealthBoard(); 
        public static MiHealthEyeTest MiHealthEyetest = new MiHealthEyeTest();
        public static MiHealthCareInfo MiHealthCareInfo = new MiHealthCareInfo();
        public static Mikio Mikio = new Mikio();
        public static int batchIndex = -1;
        public static Boolean IsBatchMode = false;
        private static OpdProgress OpdProgress = null;
        private static CalledInfoList CalledInfoList = null;

        public static Patient Patient = new Patient();
        public static xcBpMeter BpReader = null;

        private static List<SerialPort> SerialPorts;
        public static Dictionary<string, Panel> PanelDictionary = new Dictionary<string, Panel>();
        public static Dictionary<string, System.Windows.UIElement> ControlDictionary = new Dictionary<string, System.Windows.UIElement>();

        public static xcDeviceList ConnectedDeviceList = null;
        public static xcFatRecord FatRecord = null;
        public static xcBpRecord BpRecord = null;
        public static xcFatCompositionRecord FatCompositionRecord = null;
        public static xcNHICardInfo CardInfo = new xcNHICardInfo();

        public static xcSmartCardReader SmartCardReader = null;
        public static PhsAnnouncementList PhsAnnouncementList = null;

        public static PhsAnnouncementRecord AnnouncementRecord = new PhsAnnouncementRecord();
        public static xcPhsFatCompositionSheet PhsCompositionSheet = null;

        public static RegInfoList RegInfoList = new RegInfoList();
        public static RegInfo RegInfo = new RegInfo();

        public static OpdScheduleList OpdScheduleList = new OpdScheduleList();
        public static OpdSchedule OpdSchedule = new OpdSchedule();

        public static xcProgress Progress = new xcProgress();

        public static xcDictionary LangDictionary = null;

        public static xcNetwork Network = new xcNetwork();


        public static object getBinder(string dataSource) {
            return getBinder(new Uri(dataSource));
        }

        public static string getStringRes(string key, string defaultValue){
            if (LangDictionary == null && !string.IsNullOrEmpty(AppProperties.LangUrl)) {
                LangDictionary = new xcDictionary();
                LangDictionary.addXmlSource(new Uri(AppProperties.LangUrl));
            }

            return LangDictionary == null || !LangDictionary.ContainsKey(key) ? defaultValue : (string.IsNullOrEmpty(LangDictionary[key]) ? defaultValue : LangDictionary[key]);
        }

        public static object getBinder(Uri uri) {
            if (!uri.Scheme.Equals("xmpl"))
                return null;
            if (uri.Segments.Length < 3)
                return null;

            string host = uri.Host.Replace("/", "").ToLower();
            string block = uri.Segments[1].Replace("/", "").ToLower();
            string binder = uri.Segments[2].Replace("/", "").ToLower();
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);            
            switch (block) {
                case "system":
                    return getSystemBinder(host, binder, queryParameters);
                case "network":
                    return getNetworkBinder(host, binder, queryParameters);
                case "device":
                    return getDeviceBinder(host, binder, queryParameters);
                case "mitachis":
                case "mihis":
                    return getHisBinder(host, binder, queryParameters);
                case "miphs":
                    return getPhsBinder(host, binder, queryParameters);
                case "mihealth":
                    return getMiHealthBinder(host, binder, queryParameters);
                case "progress":
                    return Progress;
                case "mikio":
                    return getMikioBinder(host, binder, queryParameters);
                default:
                    return null;
            }
        }

        public static void saveToPHS(xcBpRecord bpRecord) {
            if (!CardInfo.HasCard)
                return;
            xcPhsMeasurementRecord record = new xcPhsMeasurementRecord(bpRecord);
            MiPhs.addMeasurementRecord(record, AddPhsRecordCallBack);
        }

        public static void saveToHIS(xcBpRecord bpRecord){
            if (!CardInfo.HasCard)
                return;
            MiHIS.registMeasuredData(bpRecord);
        }

        private static void AddPhsRecordCallBack(bool added) {
            if (added) {

            } else {

            }
        }

        public static void print(RegInfo regInfo) {
            if (regInfo == null)
                return;
            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            Uri uri = AppProperties.RegInfoPrintLayout;
            if (uri == null)
                return;
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);
            
            pd.addModelAttribute("reg_type", 1);
            pd.addModelAttribute("patient_name", Patient.Name); //.HolderName);
            pd.addModelAttribute("patient_id", Patient.ID); //CardInfo.HolderIDN);
            pd.addModelAttribute("case_no", regInfo.CaseNo);
            pd.addModelAttribute("opd_date", regInfo.OpdDate);
            pd.addModelAttribute("reg_date", regInfo.RegDate);
            pd.addModelAttribute("opd_time_id", regInfo.OpdTimeID);
            pd.addModelAttribute("opd_time_name", regInfo.OpdTimeName);
            pd.addModelAttribute("dept_id", regInfo.DeptID);
            pd.addModelAttribute("dept_name", regInfo.DeptName);
            pd.addModelAttribute("room_id", regInfo.RoomID);
            pd.addModelAttribute("room_name", regInfo.RoomName);
            pd.addModelAttribute("doctor_id", regInfo.DoctorID);
            pd.addModelAttribute("doctor_name", regInfo.DoctorName);
            pd.addModelAttribute("sub_doctor_id", regInfo.SubDoctorID);
            pd.addModelAttribute("sub_doctor_name", regInfo.SubDoctorName);
            pd.addModelAttribute("reg_number", regInfo.RegNumber);
            pd.addModelAttribute("memo", regInfo);

            pd.print();
        }

        public static void print(OpdSchedule schedule, string regNumber, TimeSpan estTime) {
            if (schedule == null)
                return;
            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            Uri uri = AppProperties.RegInfoPrintLayout;
            if (uri == null)
                return;
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("reg_type", schedule.OpdDate.Equals(DateTime.Today) ? 2 : 3);

            pd.addModelAttribute("patient_name", Patient.Name); //CardInfo.HolderName);
            pd.addModelAttribute("patient_id", Patient.ID);
            pd.addModelAttribute("opd_date", schedule.OpdDate);
            pd.addModelAttribute("reg_date", DateTime.Now);
            pd.addModelAttribute("opd_time_id", schedule.OpdTimeID);
            pd.addModelAttribute("opd_time_name", schedule.OpdTimeName);
            pd.addModelAttribute("dept_id", schedule.DeptID);
            pd.addModelAttribute("dept_name", schedule.DeptName);
            pd.addModelAttribute("room_id", schedule.RoomID);
            pd.addModelAttribute("room_name", schedule.RoomName);
            pd.addModelAttribute("doctor_id", schedule.DoctorID);
            pd.addModelAttribute("doctor_name", schedule.DoctorName);
            pd.addModelAttribute("sub_doctor_id", schedule.SubDoctorID);
            pd.addModelAttribute("sub_doctor_name", schedule.SubDoctorName);
            pd.addModelAttribute("reg_number", regNumber);
            pd.addModelAttribute("memo", "");

            try {
                pd.print();
            } catch {

            }
        }

        public static void print(xcBpRecord bpRecord) {
            if (bpRecord == null || bpRecord.Systolic == 0)
                return;

            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            Uri uri = AppProperties.BpInfoPrintLayout;
            if (uri == null)
                return;

            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", bpRecord.MeasuredAt);
            pd.addModelAttribute("systolic_pressure", bpRecord.Systolic);
            pd.addModelAttribute("diastolic_pressure", bpRecord.Diastolic);
            pd.addModelAttribute("pulse", bpRecord.Pulse);
            pd.addModelAttribute("comment", bpRecord.Comment);
            pd.print();
        }

        public static void save(xcFatRecord fatRecord)
        {
            saveToPHS(fatRecord);
            saveToHIS(fatRecord);
            saveToMiHealth(fatRecord);
        }

        public static void save(xcBpRecord bpRecord) {
            saveToPHS(bpRecord);
            saveToHIS(bpRecord);
            saveToMiHealth(bpRecord);
        }

        public static void saveToPHS(xcFatRecord fatRecord) {
            if (String.IsNullOrEmpty(AppProperties.PhsDomain))
                return;
            if (!CardInfo.HasCard || string.IsNullOrEmpty(Patient.ID))
                return;

            xcPhsMeasurementRecord record = new xcPhsMeasurementRecord(fatRecord);
            MiPhs.addMeasurementRecord(record, AddPhsRecordCallBack);
        }

        public static void saveToHIS(xcFatRecord fatRecord) {
            if (String.IsNullOrEmpty(AppProperties.HisDomain))
                return;
            if (!CardInfo.HasCard || string.IsNullOrEmpty(Patient.ID))
                return;
            MiHIS.registMeasuredData(fatRecord);
        }

        public static void saveToMiHealth(xcFatRecord fatRecord)
        {
            MiHealth.addMeasurementRecord(fatRecord);
        }

        public static void saveToMiHealth(xcBpRecord bpRecord) {
            MiHealth.addMeasurementRecord(bpRecord);
        }

        public static void saveToMiHealth(String leftEye, String rightEye) {
            MiHealth.addMeasurementRecord(leftEye, rightEye);
        }

        public static void saveToPHS(xcFatCompositionRecord fatCompositionRecord) {
            if (!CardInfo.HasCard)
                return;

            xcPhsMeasurementRecord record = new xcPhsMeasurementRecord(fatCompositionRecord);
            MiPhs.addMeasurementRecord(record, AddPhsRecordCallBack);
        }

        public static void printInternalDisease() {
            if (Patient.Comment == null)
                Patient.Comment = "";

            printInternalDisease(Patient.InternalDiseases, Patient.InternalDiseaseTreatments, Patient.Comment);
        }

        public static void printInternalDisease(String disease, String treatment, String comment) {
            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            Uri uri = AppProperties.InternalDiseasePrintLayout;
            if (uri == null)
                return;

            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("care_date", DateTime.Now);
            pd.addModelAttribute("name", Patient.Name);
            pd.addModelAttribute("id", Patient.ID);
            pd.addModelAttribute("department", "內科");
            pd.addModelAttribute("disease", disease);
            pd.addModelAttribute("treatment", treatment);
            pd.addModelAttribute("comment", comment);

            pd.print();
        }

        public static void printSurgery() {
            if (Patient.Comment == null)
                Patient.Comment = "";

            printSurgery(Patient.InjuredPlaces, Patient.InjuredPartName, Patient.Wounds, Patient.SurgeryDiseaseTreatments, Patient.Comment);
        }

        public static void printSurgery(String injuredPlaces, String injuredPartName, String wounds, String treatment, String comment) {
            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            Uri uri = AppProperties.SurgeryPrintLayout;
            if (uri == null)
                return;

            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("care_date", DateTime.Now);
            pd.addModelAttribute("name", Patient.Name);
            pd.addModelAttribute("id", Patient.ID);
            pd.addModelAttribute("department", "外科");
            pd.addModelAttribute("injured_place", injuredPlaces);
            pd.addModelAttribute("injured_part", injuredPartName);
            pd.addModelAttribute("wound", wounds);
            pd.addModelAttribute("treatment", treatment);
            pd.addModelAttribute("comment", comment);

            pd.print();
        }


        public static void printEyes(String leftEye, String rightEye) {
            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            Uri uri = AppProperties.EyesPrintLayout;
            if (uri == null)
                return;

            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", DateTime.Now);
            //pd.addModelAttribute("name", Patient.Name);
            //pd.addModelAttribute("id", Patient.ID);
            pd.addModelAttribute("leftEye", leftEye);
            pd.addModelAttribute("rightEye", rightEye);

            pd.print();
        }

        public static void print(xcFatRecord whRecord) {
            if (whRecord == null || whRecord.Height == 0)
                return;

            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");
            Uri uri = AppProperties.FatInfoPrintLayout;
            if (uri == null)
                return;

            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", DateTime.Now);
            pd.addModelAttribute("weight", whRecord.Weight);
            pd.addModelAttribute("height", whRecord.Height);
            pd.addModelAttribute("bmi", whRecord.BMI);
            pd.addModelAttribute("comment", whRecord.Comment);
            pd.print();
        }

        public static void print(xcFatCompositionRecord fcRecord) {
            if (fcRecord == null || fcRecord.Height == 0)
                return;

            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");
            Uri uri = AppProperties.FatCompositionPrintLayout;
            if (uri == null)
                return;
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(uri);

            pd.addModelAttribute("measured_date", DateTime.Now);
            pd.addModelAttribute("weight", fcRecord.Weight);
            pd.addModelAttribute("height", fcRecord.Height);
            pd.addModelAttribute("bodyfatpercentage", fcRecord.BodyFatPercentage);
            pd.addModelAttribute("body_fat_percentage", fcRecord.BodyFatPercentage);
            pd.addModelAttribute("fatmass", fcRecord.FatMass);
            pd.addModelAttribute("fat_mass", fcRecord.FatMass);
            pd.addModelAttribute("fatfreemass", fcRecord.FatFreeMass);
            pd.addModelAttribute("fat_free_mass", fcRecord.FatFreeMass);
            pd.addModelAttribute("bodywatermass", fcRecord.BodyWaterMass);
            pd.addModelAttribute("body_water_mass", fcRecord.BodyWaterMass);
            pd.addModelAttribute("bodyage", fcRecord.BodyAge);
            pd.addModelAttribute("body_age", fcRecord.BodyAge);
            pd.addModelAttribute("bmi", fcRecord.BMI);
            pd.addModelAttribute("bmr", fcRecord.BMR);
            pd.addModelAttribute("bmr_by_kj", fcRecord.BmrByKJ);
            pd.addModelAttribute("bmr_by_kcal", fcRecord.BmrByKCal);

            pd.addModelAttribute("body_type", fcRecord.BodyType);
            pd.addModelAttribute("gender", fcRecord.Gender);

            pd.addModelAttribute("impedance_whole_body", fcRecord.ImpedanceWholeBody);
            pd.addModelAttribute("impedance_right_leg", fcRecord.ImpedanceRightLeg);
            pd.addModelAttribute("impedance_left_leg", fcRecord.ImpedanceLeftLeg);
            pd.addModelAttribute("impedance_right_arm", fcRecord.ImpedanceRightArm);
            pd.addModelAttribute("impedance_left_arm", fcRecord.ImpedanceLeftArm);

            pd.addModelAttribute("right_leg_body_fat_percentage", fcRecord.RightLegBodyFatPercentage);
            pd.addModelAttribute("right__leg_fat_mass", fcRecord.RightLegFatMass);
            pd.addModelAttribute("right_leg_fat_free_mass", fcRecord.RightLegFatFreeMass);
            pd.addModelAttribute("right_legm_uscle_mass", fcRecord.RightLegMuscleMass);

            pd.addModelAttribute("left_leg_body_fat_percentage", fcRecord.LeftLegBodyFatPercentage);
            pd.addModelAttribute("left_leg_fat_mass", fcRecord.LeftLegFatMass);
            pd.addModelAttribute("left_leg_fat_free_mass", fcRecord.LeftLegFatFreeMass);
            pd.addModelAttribute("left_leg_muscle_mass", fcRecord.LeftLegMuscleMass);

            pd.addModelAttribute("right_arm_body_fat_percentage", fcRecord.RightArmBodyFatPercentage);
            pd.addModelAttribute("right_arm_fat_mass", fcRecord.RightArmFatMass);
            pd.addModelAttribute("right_arm_fat_free_mass", fcRecord.RightArmFatFreeMass);
            pd.addModelAttribute("right_arm_muscle_mass", fcRecord.RightArmMuscleMass);

            pd.addModelAttribute("left_arm_body_fat_percentage", fcRecord.LeftArmBodyFatPercentage);
            pd.addModelAttribute("left_arm_fat_mass", fcRecord.LeftArmFatMass);
            pd.addModelAttribute("left_arm_fat_free_mass", fcRecord.LeftArmFatFreeMass);
            pd.addModelAttribute("left_arm_muscle_mass", fcRecord.LeftArmMuscleMass);

            pd.addModelAttribute("trunk_body_fat_percentage", fcRecord.TrunkBodyFatPercentage);
            pd.addModelAttribute("trunk_fat_mass", fcRecord.TrunkFatMass);
            pd.addModelAttribute("trunk_fat_free_mass", fcRecord.TrunkFatFreeMass);
            pd.addModelAttribute("trunk_muscle_mass", fcRecord.TrunkMuscleMass);

            pd.addModelAttribute("visceral_fat_raiting", fcRecord.VisceralFatRaiting);

            pd.addModelAttribute("desirible_body_fat_percentage_range", fcRecord.DesirableBodyFatPercentageRange);
            pd.addModelAttribute("desirible_fat_mass_range", fcRecord.DesirableFatMassRange);

            pd.addModelAttribute("comment", fcRecord.Comment);
            pd.print();
        }


        public static object getSystemBinder(string host, string binder, NameValueCollection queryParameters) {
            switch (binder) {
                case "clock":
                    xcClock clock = new xcClock();
                    string format = queryParameters["format"];
                    if (!string.IsNullOrEmpty(format))
                        clock.Format = format;
                    return clock;
                default:
                    return null;
            }
        }

        public static object getNetworkBinder(string host, string binder, NameValueCollection queryParameters) {
            switch (binder) {
                case "status":
                    return Network;
                default:
                    return null;
            }
        }

        public static void onNetworkChanged(Boolean isAvailable) {
            if (isAvailable) {
                doAction(AppProperties.OnNetworConnectedAction);
            }
            else {
                doAction(AppProperties.OnNetworDisonnectedAction);
            }
        }

        private static void openDevices() {
            ConnectedDeviceList = AppProperties.SerialDeviceList;
            if (ConnectedDeviceList != null) {
                foreach (xcDevice deviceItem in ConnectedDeviceList) {
                    xcSerialDevice device = (xcSerialDevice)deviceItem;
                    if (device.Type == xcDevice.DEVICE_TYPE.BP_METER) {
                        device.OnNewRecord += OnNewBpRecord;
                    } else if (device.Type == xcDevice.DEVICE_TYPE.FAT_METER) {
                        device.OnNewRecord += OnNewFatRecord;
                    } else if (device.Type == xcDevice.DEVICE_TYPE.FAT_ANALYSER) {
                        device.OnNewRecord += OnNewFatCompositionRecord;
                    }
                    device.Open();
                }
            }

            if (BpReader == null) {
                D40 d40 = new D40();
                d40.startMonitor(OnD40StatusChanged);
            }

            if (SmartCardReader == null) {
                SmartCardReader = new xcSmartCardReader();
                SmartCardReader.OnStateChanged += OnSmartCardStateChange;
                SmartCardReader.Open();
            }

            //SmartCardReader.startCardReaderMonitor();
        }

        private static void OnSmartCardStateChange(object sender, EventArgs e) {
            XiMPLib.xDevice.xCardReader.xcSmartCardReader.CardReaderEventArgs args = (XiMPLib.xDevice.xCardReader.xcSmartCardReader.CardReaderEventArgs)e;
            switch (args.ReaderState) {
                case xcSmartCardReader.ReaderState.CONNECTED:
                    break;
                case xcSmartCardReader.ReaderState.DISCONNECTED:
                    break;
                case xcSmartCardReader.ReaderState.CARD_INSERTED:
                    if (args.CardUid == null && args.NhiCardInfo == null)
                        return;

                    if (args.NhiCardInfo != null) {
                        CardInfo.InfoBytes = args.NhiCardInfo.InfoBytes;
                        onNhiCardRead();
                    } else if (args.CardUid != null) {
                        onRfidCardRead(BitConverter.ToString(args.CardUid).Replace("-", string.Empty));
                    }
                    break;
                case xcSmartCardReader.ReaderState.CARD_REMOVED:
                    onSmartCardRemoved();
                    //RegInfoList.Clear();
                    break;
                case xcSmartCardReader.ReaderState.CARD_UNKNOWN:
                    break;
                default:
                    break;
            }
        }

        public static void clearPatientInfo()
        {
            if (MiHIS == null)
                MiHIS = new xcHis();

            if (Patient != null)
                Patient.clear();
            if (RegInfoList != null)
                RegInfoList.Clear();
            if (BpRecord != null)
            {
                BpRecord.clear();
            }
            if (FatRecord != null)
            {
                FatRecord.clear();
            }
            if (FatCompositionRecord != null)
            {
                FatCompositionRecord.clear();
            }
        }

        public static void onNhiCardRead()
        {
            clearPatientInfo();

            Patient.set(CardInfo);
            CardInfo.notifyChanged();
            if (AppProperties.MiPhsElement.HasAttributes)
            {
                MiPhs.login(phsLoginCallBack);
                RegInfoList.loads();
            }
            if (AppProperties.MiHealthElement.HasAttributes)
            {
                MiHealth.login(MiHealthLoginCallBack);
            }
        }

        public static void onRfidCardRead(String rfid)
        {
            clearPatientInfo();

            Patient.EasyCardId = rfid;
            CardInfo.notifyChanged();
            if (AppProperties.MiHealthElement.HasAttributes)
            {
                MiHealth.login(MiHealthLoginCallBack);
            }
        }

        public static void onSmartCardRemoved() {
            CardInfo.InfoBytes = null;
            CardInfo.notifyChanged();
            MiPhs.logout();
            Patient.clear();
            string action = AppProperties.OnCardRemovedAction;
            if (!string.IsNullOrEmpty(action)) {
                doAction(action);
            }
        }

        private static void MiHealthLoginCallBack(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
            }
            else
            {
                Patient.clear();
                Patient.notifyChanged();
                Progress.setMessage(getStringRes("login_failed", "Login failed."));
            }
        }

        public static void phsLoginCallBack(bool isLoggedIn) {
            if (isLoggedIn) {
            } else {
                Patient.Password = "";
                Patient.notifyChanged();
                Progress.setMessage(getStringRes("login_failed", "Login failed."));
            }
        }

        public static void OnNewFatRecord(object sender, object record) {
            FatRecord.copyFrom((xcFatRecord)record);
            string action = IsBatchMode ? AppProperties.OnNewFatRecordBatchAction : AppProperties.OnNewFatRecordAction;
            if (!string.IsNullOrEmpty(action)) {
                doAction(action);
            }
            if (AppProperties.AutoPrint)
                print(FatRecord);
            if (AppProperties.AutoSave) {
                saveToPHS(FatRecord);
                saveToHIS(FatRecord);
                saveToMiHealth(FatRecord);
            }

            if (AppProperties.SaveCampus)
            {
                xcCampusData campusData = new xcCampusData();
                campusData.setFatRecord(FatRecord.Height, FatRecord.Weight, FatRecord.BMI);
                campusData.send();
            }
        }

        public static void OnNewFatCompositionRecord(object sender, object record) {
            if (FatCompositionRecord == null)
                FatCompositionRecord = new xcFatCompositionRecord();
            FatCompositionRecord.copyFrom((xcFatCompositionRecord)record);
            string action = AppProperties.OnNewFatCompositionRecordAction;
            if (!string.IsNullOrEmpty(action)) {
                doAction(action);
            }
            if (AppProperties.AutoPrint)
                print(FatCompositionRecord);
            if (AppProperties.AutoSave) {

                saveToPHS(FatCompositionRecord);
                //saveToHIS(FatRecord);
            }

            try {
                if (!string.IsNullOrEmpty(Patient.ID) && AppProperties.SaveToLocal) {
                    if (PhsCompositionSheet == null) {
                        PhsCompositionSheet = new xcPhsFatCompositionSheet(AppProperties.PhsFatCompositionDataPath);
                    }
                    PhsCompositionSheet.addRow(Patient.ID, AppProperties.HospitalID, FatCompositionRecord);
                }
            }
            catch
            {

            }
        }

        private static void OnNewBpRecord(object sender, object record) {
            BpRecord.copyFrom((xcBpRecord)record);
            string action = AppProperties.OnNewBpRecordAction;
            if (!string.IsNullOrEmpty(action)) {
                doAction(action);
            }
            if (AppProperties.AutoPrint)
                print(BpRecord);
            if (AppProperties.AutoSave) {
                saveToPHS(BpRecord);
                saveToHIS(BpRecord);
            }

            if (AppProperties.SaveCampus)
            {
                xcCampusData campusData = new xcCampusData();
                campusData.setBloodPressureRecord(BpRecord.Systolic, BpRecord.Diastolic, BpRecord.Pulse);
                campusData.send();
            }
        }

        private static void OnD40StatusChanged(string readerName, uint readerStatus, xcBpRecord bpRecord) {
            if (!string.IsNullOrEmpty(readerName) && bpRecord != null) {
                OnNewBpRecord(readerName, bpRecord);
                //BpRecord.copyFrom(bpRecord);
                //if (AppProperties.AutoSave)
                //    saveToPHS(BpRecord);
                //if (AppProperties.AutoPrint)
                //    print(BpRecord);
            }
        }

        public static object getDeviceBinder(string host, string binder, NameValueCollection queryParameters) {
            if (ConnectedDeviceList == null)
                openDevices();

            switch (binder) {
                case "smartcardreader":
                case "cardreader":
                case "healthycard":
                case "msr":
                    return CardInfo;    
                case "bloodpressure":
                case "bpmeter":
                case "bpr":
                case "bpm":
                    if (BpRecord == null)
                        BpRecord = new xcBpRecord();
                    return BpRecord;
                case "fatmeter":
                case "heightweight":
                case "weightheight":
                    if (FatRecord == null)
                        FatRecord = new xcFatRecord();
                    return FatRecord;
                case "fatanalyser":
                    if (FatCompositionRecord == null)
                        FatCompositionRecord = new xcFatCompositionRecord();
                    return FatCompositionRecord;
                default:
                    return null;
            }
        }

        public static object getHisBinder(string host, string binder, NameValueCollection queryParameters) {
            if (MiHIS == null)
                MiHIS = new xcHis();
            switch (binder) {
                case "opdprogress":
                    if (OpdProgress == null)
                        initOpdProgress();

                    if (OpdProgress.DisplaySource.Count == 0)
                        return null;
                    string indexValue = queryParameters["index"];
                    if (!string.IsNullOrEmpty(indexValue)) {
                        int index = int.Parse(indexValue);
                        return OpdProgress.DisplaySource[index];
                    }
                    return null;
                case "calledinfolist":
                    if (CalledInfoList == null)
                        CalledInfoList = new CalledInfoList();

                    indexValue = queryParameters["index"];
                    if (!string.IsNullOrEmpty(indexValue)) {
                        int index = int.Parse(indexValue);
                        if (CalledInfoList.Count > index) {
                            return CalledInfoList[index];
                        }
                    }
                    return null;
                case "reginfolist":
                    return RegInfoList;
                case "opdschedule":
                    return OpdSchedule;
                case "opdschedulelist":
                    return OpdScheduleList;
                case "reginfo":
                    return RegInfo;
                default:
                    return null;
            }
        }

        public static object getPhsBinder(string host, string binder, NameValueCollection queryParameters) {
            switch (binder) {
                case "announcement":
                case "anmt":
                    if (PhsAnnouncementList == null) {
                        PhsAnnouncementList = new PhsAnnouncementList();
                        MiPhs.getAnouncements();
                    }
                    string target = queryParameters["target"];
                    if (!string.IsNullOrEmpty(target)) {
                        switch (target.ToLower()) {
                            case "item":
                            case "record":
                                return AnnouncementRecord;
                            case "list":
                                return AnnouncementRecord;
                        }
                        return AnnouncementRecord;
                    }else
                        return PhsAnnouncementList;
                case "patient":
                case "user":
                    return Patient;
                default:
                    return null;
            }
        }

        public static object getMiHealthBinder(string host, string binder, NameValueCollection queryParameters)
        {
            String code = queryParameters["code"];
            String text = queryParameters["text"];
            CheckBoxItem checkBoxItem = null;
            switch (binder)
            {
                case "patient":
                case "user":
                    return Patient;
                case "internaldiseaselist":
                    checkBoxItem = Patient.getCheckBoxItem(Patient.InternalDiseaseList, code, text);
                    if (checkBoxItem != null) {
                        if (!String.IsNullOrEmpty(text)) {
                            checkBoxItem.Text = text;
                        }
                        return checkBoxItem;
                    }
                    return null;
                case "internaldiseasetreatmentlist":
                    checkBoxItem = Patient.getCheckBoxItem(Patient.InternalDiseaseTreatmentList, code, text);
                    if (checkBoxItem != null) {
                        if (!String.IsNullOrEmpty(text)) {
                            checkBoxItem.Text = text;
                        }
                        return checkBoxItem;
                    }
                    return null;
                case "surgerydiseaselist":
                    checkBoxItem = Patient.getCheckBoxItem(Patient.SurgeryDiseaseList, code, text);
                    if (checkBoxItem != null) {
                        if (!String.IsNullOrEmpty(text)) {
                            checkBoxItem.Text = text;
                        }
                        return checkBoxItem;
                    }
                    return null;
                case "surgerydiseasetreatmentlist":
                    checkBoxItem = Patient.getCheckBoxItem(Patient.SurgeryDiseaseTreatmentList, code, text);
                    if (checkBoxItem != null) {
                        if (!String.IsNullOrEmpty(text)) {
                            checkBoxItem.Text = text;
                        }
                        return checkBoxItem;
                    }
                    return null;
                case "surgerypartlist":
                    checkBoxItem = Patient.getCheckBoxItem(Patient.InjuredPartList, code, text);
                    if (checkBoxItem != null) {
                        if (!String.IsNullOrEmpty(text)) {
                            checkBoxItem.Text = text;
                        }
                        return checkBoxItem;
                    }
                    return null;
                case "surgeryplacelist":
                    checkBoxItem = Patient.getCheckBoxItem(Patient.InjuredPlaceList, code, text);
                    if (checkBoxItem != null) {
                        if (!String.IsNullOrEmpty(text)) {
                            checkBoxItem.Text = text;
                        }
                        return checkBoxItem;
                    }
                    return null;
                case "carerecords":
                    return MiHealth.CareRecords;
                case "visiongrade":
                case "eyetest":
                    return MiHealthEyetest;
                case "nurse":
                    return MiHealth.Nurse;
                default:
                    return null;
            }
        }

        public static object getMikioBinder(string host, string binder, NameValueCollection queryParameters)
        {
            switch (binder.ToLower())
            {
                case "properties":
                    return Mikio;
                default:
                    return null;
            }
        }

        private static Timer OpdProgressTimer = null;

        internal static MiHealthClient MihealthClient { get => mihealthClient; set => mihealthClient = value; }

        private static void initOpdProgress() {            
            OpdProgress = MiHIS.OpdProgress;
        }

        private static void OpdProgressTimer_Elapsed(object sender, ElapsedEventArgs e) {
            OpdProgressTimer.Enabled = false;
            RoomInfo firstRoom = new RoomInfo(OpdProgress[0].ID);
            firstRoom.update(OpdProgress[0]);
            for (int i = 0; i < OpdProgress.Count-1; i++) {
                OpdProgress[i].update(OpdProgress[i+1]);
            }
            OpdProgress[OpdProgress.Count - 1].update(firstRoom);

            OpdProgress[0].notifyChanged();
            OpdProgress[1].notifyChanged();
            OpdProgress[2].notifyChanged();

            OpdProgressTimer.Enabled = true;
        }

        public static void doAction(string uriAction) {
            if (!String.IsNullOrEmpty(uriAction))
                doAction(new Uri(uriAction));
        }

        public static void doAction(Uri uriAction) {
            if (uriAction == null)
                return;

            string host = uriAction.Host.Replace("/", "").ToLower();

            string block = uriAction.Segments[1].Replace("/", "").ToLower();
            string binder = uriAction.Segments[2].Replace("/", "").ToLower();
            string item = uriAction.Segments.Length > 3 ? uriAction.Segments[3].Replace("/", "").ToLower() : null;
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uriAction.Query);
            string batchMode = queryParameters["batchMode"];
            if (!String.IsNullOrEmpty(batchMode))
            {
                IsBatchMode = Boolean.Parse(batchMode);
                string measureTarget = queryParameters["measure"];
                if (measureTarget == "eye")
                    MiHealthEyetest.setMode(MiHealthEyeTest.MODE.AUTO);
            }
            
            switch (block) {
                case "action":
                    if (binder.Equals("register_surgery_wounds")) {
                        String injuredPart = queryParameters.Get("injuredPart");
                        Patient.InjuredPartCode = injuredPart;
                    }
                    doAction(AppProperties.getAction(binder));
                    break;
                case "batch":
                    if (IsBatchMode && binder.Equals("move"))
                    {
                        String direction = queryParameters.Get("to");
                        switch (direction.ToLower())
                        {
                            case "first":
                                batchIndex = 0;
                                break;
                            case "next":
                                batchIndex++;
                                break;
                            case "prev":
                                batchIndex--;
                                break;
                            case "last":
                                batchIndex = Mikio.Students.Count - 1;
                                break;
                            default:
                                batchIndex = Int32.Parse(direction);
                                break;
                        }

                        if (batchIndex >= Mikio.Students.Count)
                        {
                            IsBatchMode = false;
                            String onFinishedAction = queryParameters["onFinished"];
                            batchIndex = -1;
                            doAction(AppProperties.getAction(onFinishedAction));
                            xcBinder.clearPatientInfo();
                        }
                        else
                        {
                            IsBatchMode = true;
                            Patient.setMihealthStudent((JObject)Mikio.Students[batchIndex]);
                        }
                    }
                    break;
                case "app":
                    switch(binder){
                        case "event":
                            switch (queryParameters["event"].ToLower()) {
                                case "onclose":
                                    bool showDialog = bool.Parse(queryParameters["ShowDialog"]);
                                    if (showDialog) {
                                        string title = queryParameters["title"];
                                        if (String.IsNullOrEmpty(title))
                                        {
                                            title = "Exit MiKIO " + AppProperties.AppVersion;
                                        }
                                        else
                                        {
                                            title = title.Replace("${AppVersion}", AppProperties.AppVersion);
                                        }
                                        string message = queryParameters["message"];
                                        if (String.IsNullOrEmpty(message))
                                        {
                                            message = "Do you really want to exit?";
                                        }                                        
                                        var response = xcMessageBox.Show(message, title, xcMessageBox.Buttons.YesNo, xcMessageBox.Icon.Exclamation);
                                        if (response == System.Windows.Forms.DialogResult.Yes) {
                                            //Application.Current.Shutdown();
                                            System.Environment.Exit(0);
                                        }
                                    } else {
                                        System.Environment.Exit(0);
                                    }
                                    break;
                                case "logout":
                                    MiHealth.logout();
                                    String afterAction = queryParameters["action"];
                                    doAction(AppProperties.getAction(afterAction));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "layout":
                    switch (binder) {
                        case "visibility":
                            foreach (string key in queryParameters.Keys) {
                                if (string.IsNullOrEmpty(key))
                                    continue;
                                try {
                                    var panel = PanelDictionary[key];
                                    if (panel != null) {
                                        var visiblity = xcUiProperty.toVisibility(queryParameters[key]);
                                        string name = "???";
                                        if (!panel.Visibility.Equals(visiblity)) {
                                            changeVisibility(panel, visiblity);
                                            if (panel != null && panel.GetType().Equals(typeof(xcCanvas))) {
                                                var canvas = (xcCanvas)panel;
                                                canvas.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { name = canvas.Name; }));
                                                Console.WriteLine("DoAction : " + name + " - " + panel.Visibility + " - " + DateTime.Now.Ticks);
                                            }
                                        }
                                        //panel.Visibility = xcUiProperty.toVisibility(queryParameters[key]);
                                    }
                                } catch (Exception e) {

                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "control":
                    switch (binder) {
                        case "phschart":
                            string controlName = queryParameters["name"];
                            var phsChart = (xcPhsChart)ControlDictionary[controlName];
                            if (phsChart != null) {
                                string target = queryParameters["target"];
                                if (!string.IsNullOrEmpty(target))
                                    phsChart.setTarget(target);
                                string period = queryParameters["period"];
                                if (!string.IsNullOrEmpty(period))
                                    phsChart.setPeriod(period);
                            }
                            break;
                        case "scroll":
                            foreach (string key in queryParameters.Keys) {
                                var control = ControlDictionary[key];
                                if (control != null) {
                                    switch (control.GetType().Name) {
                                        case "xcDataGrid":
                                            var dataGrid = (xcDataGrid)control;
                                            dataGrid.Scroll(queryParameters[key]);
                                            break;
                                        case "xcWebView2":
                                            var webView = (xcWebView2)control;
                                            webView.Scroll(queryParameters[key]);
                                            break;
                                    }
                                }
                            }
                            break;
                        case "command":
                            foreach (string key in queryParameters.Keys) {
                                var control = ControlDictionary[key];
                                if (control != null) {
                                    switch (control.GetType().Name) {
                                        case "xcWebView":
                                            var webView = (xcWebView)control;
                                            webView.command(queryParameters[key]);
                                            break;
                                        case "xcWebView2":
                                            var webView2 = (xcWebView2)control;
                                            webView2.Scroll(queryParameters[key]);
                                            break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "device":
                    String command = uriAction.Segments.Length > 3 ? uriAction.Segments[3].Replace("/", "") : null;
                    if (String.IsNullOrEmpty(command))
                        break;
                    switch (binder.ToLower()) {
                        case "fatmeter":
                            switch (command)
                            {
                                case "save":
                                    save(xcBinder.FatRecord);
                                    break;
                                case "print":
                                    xcBinder.print(xcBinder.FatRecord);
                                    break;
                                case "saveprint":
                                case "printsave":
                                    xcBinder.print(xcBinder.FatRecord);
                                    save(xcBinder.FatRecord);
                                    break;
                                case "reginfo":
                                    break;
                                default:
                                    xcBinder.print(xcBinder.FatRecord);
                                    xcBinder.saveToPHS(xcBinder.FatRecord);
                                    xcBinder.saveToHIS(xcBinder.FatRecord);
                                    break;
                            }
                            break;
                        case "bloodpressure":
                            switch (command) {
                                case "save":
                                    save(xcBinder.BpRecord);
                                    break;
                                case "print":
                                    xcBinder.print(xcBinder.BpRecord);
                                    break;
                                case "saveprint":
                                case "printsave":
                                    xcBinder.print(xcBinder.BpRecord);
                                    save(xcBinder.BpRecord);
                                    break;
                                case "reginfo":
                                    break;
                                default:
                                    xcBinder.print(xcBinder.BpRecord);
                                    xcBinder.saveToPHS(xcBinder.BpRecord);
                                    xcBinder.saveToHIS(xcBinder.BpRecord);
                                    break;
                            }
                            break;
                        case "fatanalyser":
                            xcBinder.print(xcBinder.FatCompositionRecord);
                            xcBinder.saveToPHS(xcBinder.FatCompositionRecord);
                            //xcBinder.saveToHIS(xcBinder.FatRecord);
                            break;
                        case "bpr":
                            xcBinder.print(xcBinder.BpRecord);
                            xcBinder.saveToPHS(xcBinder.BpRecord);
                            xcBinder.saveToHIS(xcBinder.BpRecord);
                            break;
                    }
                    break;
                case "print":
                    switch (binder) {
                        case "fatmeter":
                            xcBinder.print(xcBinder.FatRecord);
                            break;
                        case "bpr":
                            xcBinder.print(xcBinder.BpRecord);
                            break;
                        case "fatanalyser":
                            xcBinder.print(xcBinder.FatCompositionRecord);
                            break;
                        case "reginfo":
                            Console.WriteLine("Print Registration Info");                            
                            break;
                        default:
                            break;
                    }
                    break;
                case "fatmeter":
                case "weightheight":
                    switch (binder)
                    {
                        case "save":
                            save(xcBinder.FatRecord);
                            break;
                        case "print":
                            xcBinder.print(xcBinder.FatRecord);
                            break;
                        case "saveprint":
                        case "printsave":
                            xcBinder.print(xcBinder.FatRecord);
                            save(xcBinder.FatRecord);
                            break;
                        case "reginfo":
                            break;
                        default:
                            break;
                    }
                    break;
                case "bpr":
                case "bloodpresure":
                    switch (binder)
                    {
                        case "save":
                            break;
                        case "print":
                            xcBinder.print(xcBinder.BpRecord);
                            break;
                        case "saveprint":
                        case "printsave":
                            break;
                        case "reginfo":
                            break;
                        default:
                            break;
                    }
                    break;
                case "fatanalyser":
                    switch (binder)
                    {
                        case "save":
                            break;
                        case "print":
                            xcBinder.print(xcBinder.FatCompositionRecord);
                            break;
                        case "saveprint":
                        case "printsave":
                            break;
                        case "reginfo":
                            break;
                        default:
                            break;
                    }
                    break;
                case "reginfo":
                    Progress.setMessage(getStringRes("processing", "處理中"));
                    switch (binder) {
                        case "cancel":                            
                            RegInfo.cancel();
                            break;
                        case "checkin":
                            RegInfo.checkIn();
                            print(RegInfo);
                            break;
                        case "print":
                            Progress.setMessage(getStringRes("reginfo_printing", "正在列印中,請稍候..."));
                            print(RegInfo);
                            Progress.setMessage(getStringRes("reginfo_remove_card", "已經健保卡拿出自動登出"));
                            break;
                        default:
                            break;
                    }
                    break;
                case "opdschedule":
                    switch (binder){
                        case "register":
                            Progress.setMessage(getStringRes("processing", "處理中"));
                            if (OpdSchedule.OpdDate.Equals(DateTime.Today)) {
                                RegInfo.copyFrom(OpdSchedule);
                                MiHIS.ArrivedCheckIn(OpdSchedule.OpdDate, OpdSchedule.OpdTimeID, OpdSchedule.RoomID, arrivalCheckinCallback);
                            } else {
                                RegInfo.copyFrom(OpdSchedule);
                                MiHIS.DoReg(OpdSchedule.OpdDate, OpdSchedule.OpdTimeID, OpdSchedule.DeptID, OpdSchedule.DoctorID, "", doRegCallback);
                            }
                            break;
                    }
                    break;
                case "mihis":
                    switch (binder) {
                        case "opdschedulelist":
                            var deptID = queryParameters["deptID"];
                            OpdScheduleList.loads(deptID);
                            break;
                        default:
                            break;
                    }
                    break;
                case "miphs":
                    switch (binder) {
                        case "login":
                            MiPhs.login(phsLoginCallBack);
                            RegInfoList.loads();
                            break;
                        default:
                            break;
                    }
                    break;
                case "mihealth":
                    switch (binder)
                    {
                        case "login":
                            MiHealth.login(MiHealthLoginCallBack);
                            break;
                        case "update_care_records":
                            MiHealth.CareRecords.Clear();
                            MiHealth.updateCareRecords();
                            break;                            
                        case "disease_record":
                            switch (item) {
                                case "print":
                                    if (MiHealthCareInfo.DeptId.Equals("internal")) {
                                        printInternalDisease(MiHealthCareInfo.Disease, MiHealthCareInfo.Treatments, MiHealthCareInfo.Comment);
                                    }
                                    else if (MiHealthCareInfo.DeptId.Equals("surgery")) {
                                        printSurgery(MiHealthCareInfo.InjuredPlace, MiHealthCareInfo.InjuredPart, MiHealthCareInfo.Disease, MiHealthCareInfo.Treatments, MiHealthCareInfo.Comment);
                                    }
                                    break;
                            }
                            break;
                        case "internaldisease":
                            switch (item) {
                                case "save":
                                    MiHealth.addCareRecord("internal");
                                    break;
                                case "print":
                                    printInternalDisease();
                                    break;
                                case "saveprint":
                                    printInternalDisease();
                                    MiHealth.addCareRecord("internal");
                                    break;
                                case "update_care_id":
                                case "update_reg_no":
                                    MiHealth.Nurse.setDept("internal");
                                    Patient.updateCareID();
                                    break;
                            }
                            break;
                        case "surgery":
                            switch (item) {
                                case "save":
                                    MiHealth.addCareRecord("surgery");
                                    break;
                                case "print":
                                    printSurgery();
                                    break;
                                case "saveprint":
                                    printSurgery();
                                    MiHealth.addCareRecord("surgery");
                                    break;
                                case "update_care_id":
                                case "update_reg_no":
                                    MiHealth.Nurse.setDept("surgery");
                                    Patient.updateCareID();
                                    break;
                            }
                            break;
                        case "eyes_test":
                            switch (item) {
                                case "auto":
                                    MiHealthEyetest.setMode(MiHealthEyeTest.MODE.AUTO);
                                    break;
                                case "manual":
                                    MiHealthEyetest.setMode(MiHealthEyeTest.MODE.MANUAL);
                                    break;
                                case "result":
                                    MiHealthEyetest.setMode(MiHealthEyeTest.MODE.RESULT);
                                    break;
                                case "next":
                                    MiHealthEyetest.doNext();
                                    break;
                                case "saveprint":
                                    break;
                            }
                            break;
                        case "notice":
                            switch (item) {
                                case "refresh":
                                    
                                    // MiHealth.updateNotice();
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public static bool HasCondition(Uri conditionUri) {
            if (conditionUri == null)
                return false;
            if (conditionUri.Scheme.Equals("xmpl") && conditionUri.Segments.Length >= 4) {
                string block = conditionUri.Segments[1].Replace("/", "").Trim().ToLower();
                string module = conditionUri.Segments[2].Replace("/", "").Trim().ToLower();
                string item = conditionUri.Segments[3].Replace("/", "").Trim().ToLower();
                NameValueCollection queryParameters = HttpUtility.ParseQueryString(conditionUri.Query);
                switch (block) {
                    case "device":
                        switch (module) {
                            case "cardreader":
                                switch (item) {
                                    case "hascard":
                                        var cardInfo = (xcNHICardInfo)xcBinder.getBinder(conditionUri);
                                        return cardInfo.HasCard;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "mihis":
                        switch (module) {
                            case "checkin":
                                switch (item) {
                                    case "isbusinesshour":
                                        foreach (OpdTime opdTime in AppProperties.OpdTimeList){
                                            if (opdTime.canCheckin())
                                                return true;
                                        }
                                        return false;
                                }
                                break;
                        }
                        break;
                    case "miphs":
                        switch (module) {
                            case "user":
                                switch (item) {
                                    case "isloggedin":
                                        return MiPhs.isLoggedIn;
                                    default:
                                        break;
                                }
                                break;
                        }
                        break;
                    case "mihealth":
                        switch (module)
                        {
                            case "patient":
                                switch (item)
                                {
                                    case "isloggedin":
                                        return Patient.IsLoggedIn;
                                    default:
                                        break;
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            return false;
        }

        private static void arrivalCheckinCallback(string caseNo) {
            if (caseNo.StartsWith("Y")) {
                string regNumber = caseNo.Substring(caseNo.Length - 3);
                RegInfo.RegNumber = int.Parse(regNumber);
                RegInfo.notifyChanged();
                xcBinder.Progress.setMessage(getStringRes("checkin_succeed", "掛號已完成,請量血壓後至診間報到.\n請您收取健保卡與表單,謝謝您."));
                RegInfoList.loads();
                print(OpdSchedule, int.Parse(regNumber).ToString(), new TimeSpan());
            } else {
                string error = caseNo.Substring(2);
                Progress.setMessage(getStringRes("checkin_failed", error));
                //MessageBox.Show(error, "MiKIO Error");
            }
        }

        private static void doRegCallback(string regNo, TimeSpan estTime,string error) {
            // get registered information...
            if (string.IsNullOrEmpty(error)) {
                RegInfo.RegNumber =  int.Parse(regNo);
                RegInfo.notifyChanged();
                xcBinder.Progress.setMessage(getStringRes("register_succeed", "掛號已完成,請量血壓後至診間報到.\n請您收取健保卡與表單,謝謝您."));
                RegInfoList.loads();
                print(OpdSchedule, regNo, estTime);
            } else {
                Progress.setMessage(getStringRes("register_failed", error));
                //MessageBox.Show(error, "MiKIO Error");
            }
        }

        private delegate void ChangeVisibilitDelegate(Panel panel, Visibility visibility);
        private static void changeVisibility(Panel panel, Visibility visibility){
            if (panel.Dispatcher.CheckAccess()) {
                panel.Visibility = visibility;
            } else {
                panel.Dispatcher.BeginInvoke(new ChangeVisibilitDelegate(changeVisibility), panel, visibility);
            }
        }
    }
}
