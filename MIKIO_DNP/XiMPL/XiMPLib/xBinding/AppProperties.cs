using MitacHis.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml;
using XiMPLib.xIO;
using XiMPLib.MiHIS;
using XiMPLib.MiKIO;
using XiMPLib.xType;
using XiMPLib.xDevice;
using XiMPLib.xDevice.xBpMeter.AMPAll;
using XiMPLib.xDevice.xBpMeter.ParamaTech;
using XiMPLib.xDevice.xCardReader;
using XiMPLib.xDevice.xFatMeter.GTech;
using XiMPLib.xDevice.xFatMeter.SuperView;
using XiMPLib.xDevice.xFatAnalyser.Tanita;
using XiMPLib.MiHIS.Database;
using XiMPLib.xDevice.xBpMeter.Omron;
using XiMPLib.xUI;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualBasic.ApplicationServices;

namespace XiMPLib.xBinding {
    /// <summary>
    /// Application Properties Class
    /// </summary>
    public class AppProperties : XmlDocument {
        #region Constant Values
        private const string PROPERTY_FILE_NAME = "settings.properties";
        private const string ELEMENT_ROOT = "xmpl";
        private const string ELEMENT_HOME = "home";
        private const string ELEMENT_KIOSK = "kiosk";
        private const string ELEMENT_DEVICE = "device";
        private const string ELEMENT_VISION_TEST = "vision_test";
        private const string ELEMENT_MIPHS = "miphs";
        private const string ELEMENT_MIHIS = "mihis";
        private const string ELEMENT_MIHEALTH = "mihealth";
        private const string ELEMENT_CLINIC = "clinic";
        private const string ELEMENT_HOSPITAL = "hospital";
        private const string ELEMENT_SCHOOL = "school";
        private const string ELEMENT_DEPARTMENTS = "departments";
        private const string ELEMENT_ROOMS = "rooms";
        private const string ELEMENT_OPD_TIMES = "opd_times";
        private const string ELEMENT_ADV_LIST = "adv_list";
        private const string ELEMENT_SERIALS = "serial";
        private const string ELEMENT_DISPLAY = "display";
        private const string ELEMENT_KEYBOARD = "keyboard";
        private const string ELEMENT_PRINT_LAYTOUT = "print_layout";
        private const string ELEMENT_EVENT = "event";
        private const string ELEMENT_ACTIONS = "actions";
        private const string ELEMENT_MESSAGES = "messages";
        private const string ELEMENT_FAT_COMMENT = "fat_comment";


        private const string NAME_URL = "url";
        private const string NAME_LANG_URL = "lang_url";
        private const string NAME_APP_URL = "app_url";
        private const string NAME_LANGUAGE = "language";
        private const string NAME_ID = "id";
        private const string NAME_NAME = "name";
        private const string NAME_PASSWORD = "password";
        private const string NAME_AUTHKEY = "auth_key";
        private const string NAME_TITLE = "title";
        private const string NAME_VALUE = "value";
        private const string NAME_CHECKIN_RANGE = "checkin_range";
        private const string NAME_OPD_TIME_RANGE = "range";
        private const string NAME_LENGTH = "length";
        private const string NAME_START_TIME = "start_time";
        private const string NAME_END_TIME = "end_time";
        private const string NAME_VENDOR = "vendor";
        private const string NAME_AUTO_SAVE = "auto_save";
        private const string NAME_SAVE_CAMPUS = "save_campus";
        private const string NAME_AUTO_PRINT = "auto_print";
        private const string NAME_SAVE_TO_LOCAL = "save_to_local";
        private const string NAME_SAVE_WITHOUT_CARD = "save_without_card";
        private const string NAME_TYPE = "type";
        private const string NAME_TARGET = "target";
        private const string NAME_ACTION = "action";
        private const string NAME_BATCH_ACTION = "batchAction";
        private const string NAME_PORT = "port";
        private const string NAME_BAUD_RATE = "baud_rate";
        private const string NAME_PARITY = "parity";
        private const string NAME_DATA_BITS = "data_bits";
        private const string NAME_STOP_BITS= "stop_bits";
        private const string NAME_DELAY = "delay";

        private const string NAME_BP_INFO_PRINT_LAYOUT = "bp_record";
        private const string NAME_FAT_INFO_PRINT_LAYOUT = "fat_record";
        private const string NAME_FAT_COMPOSITION_PRINT_LAYOUT = "fat_composition_record";
        private const string NAME_REG_INFO_PRINT_LAYOUT = "reg_info";
        private const string NAME_CHECKIN_INFO_PRINT_LAYOUT = "checkin_info";
        private const string NAME_INTERNAL_DISEASE_PRINT_LAYOUT = "internal_disease";
        private const string NAME_SURGERY_PRINT_LAYOUT = "surgery";
        private const string NAME_EYES_PRINT_LAYOUT = "eyes_record";

        private const string NAME_DISTANCE = "distance";
        private const string NAME_MINUTES = "minutes";
        private const string NAME_NOTATION = "notation";
        private const string NAME_TYPEFACE = "typeface";
        private const string NAME_TEST_GRADES = "test_grades";

        private const string DEFAULT_PHS_DOMAIN =     "http://220.228.160.36/mehc-miphs/s/miphs";
        private const string DEFAULT_PHS_APP_DOMAIN = "http://220.228.160.36/mehc-mitac/s/miphs";
        private const string DEFAULT_HIS_DOMAIN = "https://dev.mihis.com.tw/THESEHisSRV/STheseHis.asmx";
        //private const string DEFAULT_HIS_DOMAIN = "https://thcloud.mihis.com.tw/THESEHisSRV/STheseHis.asmx";

        private const string DEFAULT_FAT_COMPOSITION_DATA_PATH = "data/PhsFatComposition.xls";

        #endregion

        #region Properties

        public String AppVersion
        {
            get
            {
                return new ApplicationBase().Info.Version.ToString();
            }
        }

        /// <summary>
        /// Get/set File Path
        /// </summary>
        public string FilePath {
            get;
            set;
        }

        /// <summary>
        /// Get Root XML Element
        /// </summary>
        private XmlElement RootElement {
            get {
                return getElement(ELEMENT_ROOT);
            }
        }

        /// <summary>
        /// Get Home XML Element
        /// </summary>
        private XmlElement HomeElement {
            get {
                return getElement(RootElement, ELEMENT_HOME);
            }
        }

        /// <summary>
        /// Get Kiosk XML Element
        /// </summary>
        private XmlElement KioskElement {
            get {
                return getElement(RootElement, ELEMENT_KIOSK);
            }
        }

        private XmlElement DeviceElement {
            get {
                return getElement(RootElement, ELEMENT_DEVICE);
            }
        }

        /// <summary>
        /// Get HIS XML Element
        /// </summary>
        public XmlElement MiHisElement {
            get {
                return getElement(RootElement, ELEMENT_MIHIS);
            }
        }

        /// <summary>
        /// Get PHS XML Element
        /// </summary>
        public XmlElement MiPhsElement {
            get {
                return getElement(RootElement, ELEMENT_MIPHS);
            }
        }

        public XmlElement MiHealthElement
        {
            get
            {
                return getElement(RootElement, ELEMENT_MIHEALTH);
            }
        }

        /// <summary>
        /// Get Hospital XML Element
        /// </summary>
        private XmlElement ClinicElement
        {
            get {
                var element = getElement(RootElement, ELEMENT_CLINIC);
                
                if (!element.HasAttributes)
                {
                    element = getElement(RootElement, ELEMENT_HOSPITAL);
                }
                if (!element.HasAttributes)
                {
                    element = getElement(RootElement, ELEMENT_SCHOOL);
                }

                return element;
            }
        }

        public XmlElement HospitalElement
        {
            get
            {
                return ClinicElement;
            }            
        }

        public XmlElement SchoolElement
        {
            get
            {
                return ClinicElement;
            }
        }


        private XmlElement ActionsElement
        {
            get
            {
                return getElement(RootElement, ELEMENT_ACTIONS);
            }
        }

        private XmlElement EventElement {
            get {
                return getElement(RootElement, ELEMENT_EVENT);
            }
        }

        private XmlElement MessagesElement
        {
            get
            {
                return getElement(RootElement, ELEMENT_MESSAGES);
            }
        }

        private XmlElement FatCommentElement
        {
            get
            {
                return getElement(MessagesElement, ELEMENT_FAT_COMMENT);
            }
        }

        private XmlElement PrintLayoutElement {
            get {
                return getElement(RootElement, ELEMENT_PRINT_LAYTOUT);
            }
        }

        public Uri BpInfoPrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_BP_INFO_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        public Uri FatInfoPrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_FAT_INFO_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }
        public Uri FatCompositionPrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_FAT_COMPOSITION_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        public Uri RegInfoPrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_REG_INFO_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        public Uri CheckinInfoPrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_CHECKIN_INFO_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        public Uri InternalDiseasePrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_INTERNAL_DISEASE_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        public Uri SurgeryPrintLayout {
            get {
                string url = PrintLayoutElement.GetAttribute(NAME_SURGERY_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        public Uri EyesPrintLayout
        {
            get
            {
                string url = PrintLayoutElement.GetAttribute(NAME_EYES_PRINT_LAYOUT).ToString();
                return string.IsNullOrEmpty(url) ? null : new System.Uri(new Uri(FilePath), url);
            }
        }

        /// <summary>
        /// Get/set Home URL Property
        /// </summary>
        public string HomeUrl {
            get {
                string url = HomeElement.GetAttribute(NAME_URL);
                return new System.Uri(new Uri(FilePath), url).AbsolutePath;
            }
            set {
                HomeElement.SetAttribute(NAME_URL, value);
                save();
            }
        }

        public string LangUrl {
            get {
                string url = HomeElement.GetAttribute(NAME_LANG_URL);
                return new System.Uri(new Uri(FilePath), url).AbsolutePath;
            }
            set {
                HomeElement.SetAttribute(NAME_LANG_URL, value);
                save();
            }
        }

        /// <summary>
        /// Get/set Domain & Root path of HIS WebService Property
        /// </summary>
        public string HisDomain {
            get {
                string domain = MiHisElement.GetAttribute(NAME_URL);
                //if (string.IsNullOrEmpty(domain)) {
                //    HisDomain = domain = DEFAULT_HIS_DOMAIN;
                //}
                return domain;
            }
            set {
                MiHisElement.SetAttribute(NAME_URL, value);
                save();
            }
        }

        /// <summary>
        /// Get/set Domain & Root path of PHS Web Service Property
        /// </summary>
        public string PhsDomain {
            get {
                string domain = MiPhsElement.GetAttribute(NAME_URL);
                //if (string.IsNullOrEmpty(domain)) {
                //    PhsAppDomain = domain = DEFAULT_PHS_DOMAIN;
                //}
                return domain;
            }
            set {
                MiPhsElement.SetAttribute(NAME_URL, value);
                save();
            }
        }

        /// <summary>
        /// Get/set PHS App Domain property
        /// </summary>
        public string PhsAppDomain {
            get {
                return MiPhsElement.GetAttribute(NAME_APP_URL);
            }
            set {
                MiPhsElement.SetAttribute(NAME_APP_URL, value);
            }
        }

        public string MiHealthDomain
        {
            get
            {
                return MiHealthElement.GetAttribute(NAME_URL);
            }
            set
            {
                MiHealthElement.SetAttribute(NAME_URL, value);
            }
        }


        /// <summary>
        /// Get/set Language property
        /// </summary>
        public string Language {
            get {
                return HomeElement.GetAttribute(NAME_LANGUAGE);
            }
            set {
                HomeElement.SetAttribute(NAME_LANGUAGE, value);
            }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                return String.IsNullOrEmpty(Language) ? CultureInfo.DefaultThreadCurrentCulture : new CultureInfo(Language);
            }
        }

        /// <summary>
        /// Get/set Kiosk ID property
        /// </summary>
        public string KioskID {
            get {
                return KioskElement.GetAttribute(NAME_ID);
            }
            set {
                KioskElement.SetAttribute(NAME_ID, value);
            }
        }

        /// <summary>
        /// Get/set Kiosk password property
        /// </summary>
        public string KioskPassword {
            get {
                return KioskElement.GetAttribute(NAME_PASSWORD);
            }
            set {
                KioskElement.SetAttribute(NAME_PASSWORD, value);
            }
        }

        public bool AutoSave {
            get {
                var value = KioskElement.GetAttribute(NAME_AUTO_SAVE);
                return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
            }
            set {
                KioskElement.SetAttribute(NAME_AUTO_SAVE, value.ToString());
            }
        }
        public bool SaveCampus
        {
            get
            {
                var value = KioskElement.GetAttribute(NAME_SAVE_CAMPUS);
                return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
            }
            set
            {
                KioskElement.SetAttribute(NAME_SAVE_CAMPUS, value.ToString());
            }
        }


        public bool AutoPrint {
            get {
                var value = KioskElement.GetAttribute(NAME_AUTO_PRINT);
                return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
            }
            set {
                KioskElement.SetAttribute(NAME_AUTO_PRINT, value.ToString());
            }
        }

        public bool SaveToLocal {
            get {
                var value = KioskElement.GetAttribute(NAME_SAVE_TO_LOCAL);
                return string.IsNullOrEmpty(value) ? true : bool.Parse(value);
            }
            set {
                KioskElement.SetAttribute(NAME_SAVE_TO_LOCAL, value.ToString());
            }
        }

        public bool SaveWithoutCard {
            get {
                var value = KioskElement.GetAttribute(NAME_SAVE_WITHOUT_CARD);
                return string.IsNullOrEmpty(value) ? true : bool.Parse(value);
            }
            set {
                KioskElement.SetAttribute(NAME_SAVE_WITHOUT_CARD, value.ToString());
            }
            
        }

        public string PhsFatCompositionDataPath {
            get {
                return new System.Uri(new Uri(FilePath), DEFAULT_FAT_COMPOSITION_DATA_PATH).AbsolutePath;
            }
        }

        /// <summary>
        /// Get/set Hospital ID property
        /// </summary>
        public string HospitalID {
            get {
                return HospitalElement.GetAttribute(NAME_ID);
            }
            set {
                HospitalElement.SetAttribute(NAME_ID, value);
            }
        }

        /// <summary>
        /// Get/set Hospital Authorization Key property
        /// </summary>
        public string HospitalAuthKey {
            get {
                return HospitalElement.GetAttribute(NAME_AUTHKEY);
            }
            set {
                HospitalElement.SetAttribute(NAME_AUTHKEY, value);
            }
        }

        /// <summary>
        /// Get Rooms XML Element
        /// </summary>
        private XmlElement RoomsElement {
            get {
                return getElement(HospitalElement, ELEMENT_ROOMS);
            }
        }

        private XmlElement DepartmentsElement
        {
            get
            {
                return getElement(HospitalElement, ELEMENT_DEPARTMENTS);
            }
        }

        public String getDepartmentName(String id) {
            if (DepartmentsElement.IsEmpty)
                return id.StartsWith("internal") ? "內科" : "外科";

            foreach (XmlElement element in DepartmentsElement) {
                if (element.GetAttribute(NAME_ID) == id)
                    return element.GetAttribute(NAME_NAME);
            }

            return id.StartsWith("internal") ? "內科" : "外科";
        }

        /// <summary>
        /// Get Room List
        /// </summary>
        public RoomList RoomList {
            get {
                if (RoomsElement.IsEmpty)
                    return null;

                RoomList list = new RoomList();
                foreach (XmlElement element in RoomsElement) {
                    list.Add(new RoomInfo(element.GetAttribute(NAME_ID), string.Empty, element.GetAttribute(NAME_TITLE), OpdTimeList));
                }
                return list;
            }
        }

        public string OnNewBpRecordAction {
            get {
                return getEventAction("onNewRecord", "bp_record");
            }
        }

        public string OnNewFatRecordAction {
            get {
                return getEventAction("onNewRecord", "fat_record");
            }
        }

        public string OnNewFatRecordBatchAction
        {
            get
            {
                return getEventAction("onNewRecord", "fat_record", true);
            }
        }

        public string OnNewFatCompositionRecordAction {
            get {
                return getEventAction("onNewRecord", "fat_composition_record");
            }
        }

        public string OnCardInserted {
            get {
                return getEventAction("onCardInserted", "card_info");
            }
        }

        public string OnCardRemovedAction {
            get {
                return getEventAction("onCardRemoved", "card_info");
            }
        }

        public string OnNetworConnectedAction
        {
            get
            {
                return getEventAction("onNetworkConnected", "network_info");
            }
        }

        public string OnNetworDisonnectedAction
        {
            get
            {
                return getEventAction("onNetworkDisconnected", "network_info");
            }
        }

        private List<xcActionInfo> actionList = null;

        public List<xcActionInfo> ActionList
        {
            get
            {
                if (actionList != null || ActionsElement.IsEmpty)
                    return actionList;

                actionList = new List<xcActionInfo>();
                foreach (XmlElement element in ActionsElement)
                {
                    actionList.Add(new xcActionInfo(element.GetAttribute(NAME_NAME), element.GetAttribute(NAME_ACTION)));
                }
                return actionList;
            }
        }

        public List<xcEventInfo> EventList {
            get {
                if (EventElement.IsEmpty)
                    return null;

                List<xcEventInfo> list = new List<xcEventInfo>();
                foreach (XmlElement element in EventElement) {
                    list.Add(new xcEventInfo(element.Name, element.GetAttribute(NAME_TARGET), element.GetAttribute(NAME_ACTION), element.GetAttribute(NAME_BATCH_ACTION)));
                }
                return list;
            }
        }

        public GL150 Gl150;
        public BP868F Bp868;
        public HBP9020 Hbp9020;
        public BC418 Bc418;

        public xcDeviceList SerialDeviceList {
            get {
                if (DeviceElement.IsEmpty)
                    return null;

                xcDeviceList list = new xcDeviceList();
                foreach (XmlElement element in DeviceElement) {
                    string vendor = element.GetAttribute(NAME_VENDOR);
                    string name = element.GetAttribute(NAME_NAME);
                    if (string.IsNullOrEmpty(name))
                        continue;
                    //var deviceType = (xcDevice.DEVICE_TYPE)Enum.Parse(typeof(xcDevice.DEVICE_TYPE), element.GetAttribute(NAME_TYPE), true);
                    if (!element.Name.ToLower().Equals("serial"))
                        continue;
                    xcSerialDevice device = null;
                    string portName = element.GetAttribute(NAME_PORT);
                    switch (name) {
                        case "FT201":
                            device = new FT201(portName);
                            break;
                        case "BP868F":
                            device = Bp868 = new BP868F(portName);
                            break;
                        case "HBP-9020":
                        case "HBP9020":
                            device = Hbp9020 = new HBP9020(portName);
                            break;
                        case "GL150":
                            device = Gl150 = new GL150(portName);
                            break;
                        case "HW3030":
                            device = new HW3030(portName);
                            break;
                        case "BC418":
                            device = Bc418 = new BC418(portName);
                            break;
                        default:
                            break;
                    }
                    //xcSerialDevice device = new xcSerialDevice(vendor, name, deviceType);
                    device.Interface = xcDevice.DEVICE_INTERFACE.SERIAL;
                    //device.PortName = element.GetAttribute(NAME_PORT);
                    device.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), element.GetAttribute(NAME_PARITY));
                    device.BaudRate = int.Parse(element.GetAttribute(NAME_BAUD_RATE));
                    device.DataBits = int.Parse(element.GetAttribute(NAME_DATA_BITS));
                    device.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), element.GetAttribute(NAME_STOP_BITS));
                    if (element.HasAttribute(NAME_DELAY))
                        device.Delay = int.Parse(element.GetAttribute(NAME_DELAY));
                    list.Add(device);
                }
                return list;
            }
            set {
                DeviceElement.RemoveAll();
                foreach (xcDevice device in value) {
                    if (device.Interface != xcDevice.DEVICE_INTERFACE.SERIAL)
                        continue;
                    XmlElement nodeChild = CreateElement(ELEMENT_SERIALS);
                    nodeChild.SetAttribute(NAME_VENDOR, device.Vendor);
                    nodeChild.SetAttribute(NAME_NAME, device.Name);
                    nodeChild.SetAttribute(NAME_TYPE, device.Type.ToString());
                    nodeChild.SetAttribute(NAME_PORT, device.PortName);
                    nodeChild.SetAttribute(NAME_BAUD_RATE, device.BaudRate.ToString());
                    nodeChild.SetAttribute(NAME_PARITY, device.Parity.ToString());
                    nodeChild.SetAttribute(NAME_DATA_BITS, device.DataBits.ToString());
                    nodeChild.SetAttribute(NAME_STOP_BITS, device.StopBits.ToString());
                    DeviceElement.AppendChild(nodeChild);
                }

                save();
            }
        }

        public XmlElement DisplayElement
        {
            get
            {

                return getElement(DeviceElement, ELEMENT_DISPLAY);
            }
        }

        public Double DisplayPPI
        {
            get
            {
                String ppi = DisplayElement.GetAttribute("ppi");
                return String.IsNullOrEmpty(ppi) ? xcScreen.DpiX : (double)xcDecimal.Parse(ppi);
            }
        }

        public Double DisplayDistance
        {
            get
            {
                const double defaultDistance = 5000;
                if (DisplayElement == null)
                    return defaultDistance;

                String distance = DisplayElement.GetAttribute("distance");
                return String.IsNullOrEmpty(distance) ? defaultDistance : new xcLength(distance).MiliMeter;
            }
        }

        public XmlElement VisionTestElement
        {
            get
            {

                return getElement(RootElement, ELEMENT_VISION_TEST);
            }
        }

        public String VisionTestTypeface
        {
            get{
                return VisionTestElement == null ? "E" : VisionTestElement.GetAttribute(NAME_TYPEFACE);
            }
        }

        String[] testGrades = null;
        public String[] TestGrades
        {
            get
            {
                if (testGrades != null)
                    return testGrades;

                const String defaultGrades = "0.03, 0.04, 0.05, 0.063, 0.08, 0.1, 0.125, 0.16, 0.2, 0.25, 0.32, 0.4, 0.5, 0.63, 0.8, 1.0, 1.6, 2.0";

                String grades = VisionTestElement != null && VisionTestElement.HasAttribute(NAME_TEST_GRADES) ? VisionTestElement.GetAttribute(NAME_TEST_GRADES) : defaultGrades;
                if (String.IsNullOrEmpty(grades))
                    grades = defaultGrades;

                testGrades = Array.ConvertAll(grades.Split(','), p => p.Trim());

                return testGrades;
            }
        }

        public List<xcVisualAcuity> VisualAcuities = new List<xcVisualAcuity>();

        public List<xcVisionGrade> VisionGrades = new List<xcVisionGrade>() {

            //new xcVisionGrade("0.03", "3.5", "1.5", 19.953f, 2),
            //new xcVisionGrade("0.04", "3.6", "1.4", 19.953f, 2),
            new xcVisionGrade("0.05", "3.7", "1.3", 19.953f, 2),
            new xcVisionGrade("0.06", "3.8", "1.2", 15.849f, 2),
            new xcVisionGrade("0.08", "3.9", "1.1", 12.589f, 2),
            new xcVisionGrade("0.1", "4.0", "1.0", 10f, 2),

            new xcVisionGrade("0.125", "4.1", "0.9", 7.943f, 3),
            new xcVisionGrade("0.15", "4.2", "0.8", 6.31f, 3),
            new xcVisionGrade("0.2", "4.3", "0.7", 5.012f, 3),

            new xcVisionGrade("0.25", "4.4", "0.6", 3.981f, 5),
            new xcVisionGrade("0.32", "4.5", "0.5", 3.162f, 5),
            new xcVisionGrade("0.4", "4.6", "0.4", 2.512f, 5),
            new xcVisionGrade("0.5", "4.7", "0.3", 1.995f, 5),
            new xcVisionGrade("0.6", "4.8", "0.2", 1.585f, 5),
            new xcVisionGrade("0.8", "4.9", "0.1", 1.259f, 5),
            new xcVisionGrade("1.0", "5.0", "0.0", 1, 5),
            new xcVisionGrade("1.25", "5.1", "-0.1", 0.794f, 5),
            new xcVisionGrade("1.5", "5.2", "-0.2", 0.631f, 5),

            new xcVisionGrade("2.0", "5.3", "-0.3", 0.501f, 5),
        };

        /// <summary>
        /// Return Visual Acuity Notation
        /// </summary>
        private xcVisualAcuity.NOTATION visualAcuityNotation = xcVisualAcuity.NOTATION.UNKNOWN;
        public xcVisualAcuity.NOTATION VisionTestNotation
        {
            get
            {
                if (visualAcuityNotation != xcVisualAcuity.NOTATION.UNKNOWN)
                {
                    return visualAcuityNotation;
                }

                if (VisionTestElement == null)
                {
                    visualAcuityNotation = xcVisualAcuity.NOTATION.DECIMAL;
                }
                else
                {
                    String unit = VisionTestElement.GetAttribute(NAME_NOTATION);
                    if (String.IsNullOrEmpty(unit))
                    {
                        unit = VisionTestElement.GetAttribute(NAME_NOTATION);
                    }
                    switch (unit)
                    {
                        case "decimal":
                        case "d":
                        case "2":
                            visualAcuityNotation = xcVisualAcuity.NOTATION.DECIMAL;
                            break;
                        case "arc5min":
                        case "5min":
                        case "min5":
                        case "5":
                            visualAcuityNotation = xcVisualAcuity.NOTATION.MIN5;
                            break;
                        case "logmar":
                        case "l":
                            visualAcuityNotation = xcVisualAcuity.NOTATION.LOG_MAR;
                            break;
                        case "snellen4m":
                        case "4m":
                        case "4":
                            visualAcuityNotation = xcVisualAcuity.NOTATION.SNELLEN_4M;
                            break;
                        case "senellen6m":
                        case "6m":
                        case "6":
                            visualAcuityNotation = xcVisualAcuity.NOTATION.SNELLEN_4M;
                            break;
                        case "snellen20ft":
                        case "20ft":
                        case "20":
                            visualAcuityNotation = xcVisualAcuity.NOTATION.SNELLEN_20FT;
;
                            break;
                        default:
                            visualAcuityNotation = xcVisualAcuity.NOTATION.DECIMAL;
                            break;
                    }
                }

                return visualAcuityNotation;
            }
        }


        public Double VisionTestDistance
        {
            get
            {
                if (VisionTestElement == null)
                    return DisplayDistance;
               
                String distance = VisionTestElement.GetAttribute("distance");

                return String.IsNullOrEmpty(distance) ? DisplayDistance : new xcLength(distance).MiliMeter;
            }
        }



        public XmlElement KeyboardElement
        {
            get
            {

                return getElement(DeviceElement, ELEMENT_KEYBOARD);
            }
        }

        public xcRemoteKeyboard RemoteKeyboard {
            get
            {
                if (KeyboardElement == null) {
                    return new xcRemoteKeyboard();
                }
                else {
                    String model = KeyboardElement.GetAttribute(NAME_NAME);
                    switch (model) {
                        case "LR8R":
                            return new LR8R();
                        default:
                            return new xcRemoteKeyboard();
                    }
                }
            }
        }

        /// <summary>
        /// Get OPD Times XML Element
        /// </summary>
        private XmlElement OpdTimesElement {
            get {
                return getElement(HospitalElement, ELEMENT_OPD_TIMES);
            }
        }

        /// <summary>
        /// Get OPD Time list
        /// </summary>
        public OpdTimeList OpdTimeList {
            get {
                if (RoomsElement.IsEmpty)
                    return null;

                OpdTimeList list = new OpdTimeList();
                foreach (XmlElement element in OpdTimesElement) {
                    string id = element.GetAttribute(NAME_ID);
                    string name = element.GetAttribute(NAME_NAME);
                    string checkinRange = element.GetAttribute(NAME_CHECKIN_RANGE);
                    string range = element.GetAttribute(NAME_OPD_TIME_RANGE);
                    OpdTime opdTime = new OpdTime(int.Parse(id), name, range, checkinRange);
                    list.Add(opdTime);
                }
                return list;
            }
        }

        Dictionary<String, String> fatComents = new Dictionary<string, string>();
        public Dictionary<String, String> FatComments
        {
            get
            {
                if (fatComents.Count > 1)
                    return fatComents;

                if (FatCommentElement.IsEmpty) {
                    fatComents.Add("underweight", "體重過輕");
                    fatComents.Add("healthy", "正常");
                    fatComents.Add("overweight", "過重");
                    fatComents.Add("obese", "輕度肥胖");
                    fatComents.Add("fat", "中度肥胖");
                    fatComents.Add("overfat", "重度肥胖");
                }
                else {
                    foreach (XmlElement element in FatCommentElement) {
                        string id = element.GetAttribute(NAME_ID);
                        string value = element.GetAttribute(NAME_VALUE);
                        fatComents.Add(id, value);
                    }
                }
                return fatComents;
            }
        }

        /// <summary>
        /// Get Advertisement List XML Element
        /// </summary>
        private XmlElement AdvListElement {
            get {
                return getElement(HospitalElement, ELEMENT_ADV_LIST);
            }
        }
        
        /// <summary>
        /// Get Advertisement List
        /// </summary>
        public List<xcPlayItem> AdvList {
            get {
                if (AdvListElement.IsEmpty)
                    return null;

                List<xcPlayItem> list = new List<xcPlayItem>();
                //var nameAttr = AdvListElement.Attributes["Name"];
                //string Name = nameAttr == null ? String.Empty : nameAttr.Value;
                foreach (XmlElement element in AdvListElement) {
                    string id = element.GetAttribute(NAME_ID);
                    string url = element.GetAttribute(NAME_URL);
                    if (!string.IsNullOrEmpty(url)) {
                        url = new System.Uri(new Uri(FilePath), url).AbsolutePath;
                        string length = element.GetAttribute(NAME_LENGTH);
                        string startTime = element.GetAttribute(NAME_START_TIME);
                        string endTime = element.GetAttribute(NAME_END_TIME);
                        list.Add(new xcPlayItem(id, url, length, startTime, endTime));
                    }
                }
                return list;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AppProperties() {
            this.FilePath = System.IO.Path.Combine(xcPath.DocumentPath, PROPERTY_FILE_NAME);
            load();

            //SmartCardReader.startCardReaderMonitor();
            
            //List<xcSerialDevice> deviceList = SerialDeviceList;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath"></param>
        public AppProperties(string filePath) {
            this.FilePath = filePath;
            load();
        }
        #endregion

        #region Room Item Handler
        /// <summary>
        /// Get Room Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private XmlElement getRoom(string id) {
            foreach (XmlElement element in RoomsElement) {
                if (element.GetAttribute(NAME_ID).Equals(id))
                    return element;
            }
            return null;
        }

        /// <summary>
        /// Add Room Item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        public void addRoom(string id, string title) {
            XmlElement element = getRoom(id);
            if (element == null) {
                element = CreateElement("room");
                element.SetAttribute(NAME_ID, id);
                RoomsElement.AppendChild(element);
            }

            element.SetAttribute(NAME_TITLE, title);
            
        }

        /// <summary>
        /// Add Room Item
        /// </summary>
        /// <param name="info"></param>
        public void addRoom(RoomInfo info) {
            addRoom(info.ID, info.Title);
        }
        #endregion

        #region OPD Time item handler
        /// <summary>
        /// Get OPD Time item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private XmlElement getOpdTime(int id) {
            foreach (XmlElement element in OpdTimesElement) {
                if (element.GetAttribute(NAME_ID).Equals(id.ToString()))
                    return element;
            }
            return null;
        }

        /// <summary>
        /// Add OPD Time item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="range"></param>
        public void addOpdTime(int id, string name, string range, string checkinRange=null) {
            XmlElement element = getOpdTime(id);
            if (element == null) {
                element = CreateElement("opd_time");
                element.SetAttribute(NAME_ID, id.ToString());
                OpdTimesElement.AppendChild(element);
            }

            element.SetAttribute(NAME_NAME, name);
            if (!string.IsNullOrEmpty(checkinRange))
                element.SetAttribute(NAME_CHECKIN_RANGE, checkinRange);
            element.SetAttribute(NAME_OPD_TIME_RANGE, range);

        }

        /// <summary>
        /// Add OPD Time item
        /// </summary>
        /// <param name="opdTime"></param>
        public void addOpdTime(OpdTime opdTime) {
            addOpdTime(opdTime.Id, opdTime.Name, opdTime.RangeString);
        }
        #endregion

        #region Advertisement Item Handler
        /// <summary>
        /// Get Advertisement item with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private XmlElement getAdv(int id) {
            foreach (XmlElement element in AdvListElement) {
                if (element.GetAttribute(NAME_ID).Equals(id.ToString()))
                    return element;
            }
            return null;
        }

        /// <summary>
        /// Add Advertisement item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="length"></param>
        public void addAdv(int id, string url, int length) {
            XmlElement element = getAdv(id);
            if (element == null) {
                element = CreateElement("media");
                element.SetAttribute(NAME_ID, id.ToString());
                AdvListElement.AppendChild(element);
            }

            element.SetAttribute(NAME_URL, url);
            element.SetAttribute(NAME_LENGTH, length.ToString());
        }

        /// <summary>
        /// Add Advertisement item
        /// </summary>
        /// <param name="adv"></param>
        public void addAdv(xcPlayItem adv) {
            addAdv(adv.ID, adv.Url, adv.Duration);
        }
        #endregion

        /// <summary>
        /// Get Element with the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private XmlElement getElement(string name) {
            if (this[name] == null)
                this.AppendChild(CreateElement(name));
            return this[name];
        }

        /// <summary>
        /// Get XML Element from the parent element
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private XmlElement getElement(XmlElement parent, string name) {
            if (parent[name] == null)
                parent.AppendChild(CreateElement(name));
            return parent[name];
        }


        /// <summary>
        /// Load settings.properties
        /// </summary>
        private void load() {
            xcFile file = new xcFile(FilePath);
            if (file.exists()) {
                try {
                    Load(FilePath);
                } catch {
                    initiailize();
                }
            } else {
                initiailize();
            }
            initVisionGrades();
        }

        public void initVisionGrades() {
            bool isDefault = VisionTestElement == null || !VisionTestElement.HasAttribute(NAME_TEST_GRADES);
            xcVisualAcuity.NOTATION notation = isDefault ? xcVisualAcuity.NOTATION.DECIMAL : VisionTestNotation;
            foreach (String grade in TestGrades) {
                xcVisualAcuity acuity = null;
                switch (notation)
                {
                    case xcVisualAcuity.NOTATION.DECIMAL:
                    case xcVisualAcuity.NOTATION.MIN5:
                    case xcVisualAcuity.NOTATION.LOG_MAR:
                        acuity = new xcVisualAcuity(notation, Double.Parse(grade));
                        break;
                    case xcVisualAcuity.NOTATION.SNELLEN_4M:
                    case xcVisualAcuity.NOTATION.SNELLEN_6M:
                    case xcVisualAcuity.NOTATION.SNELLEN_20FT:
                        acuity = new xcVisualAcuity(notation, grade);
                        break;
                          
                }
                if (acuity != null && acuity.Decimal >= 0.03 && acuity.Decimal <= 2.0)
                    VisualAcuities.Add(acuity);
            }
        }

        /// <summary>
        /// Save properties
        /// </summary>
        public void save() {
            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    ",
                NewLineChars = "\r\n",
                NewLineOnAttributes = true,
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Replace
            };

            try {
                using (XmlWriter writer = XmlWriter.Create(FilePath, settings)) {
                    Save(writer);
                }
            } catch {

            }
        }

        /// <summary>
        /// Edit properties
        /// Run Notepad to edit properties file directly
        /// </summary>
        public void editProperties() {
            MessageBox.Show("Home url is not defined yet.\nPlease edif the file : " + FilePath);

            System.Diagnostics.Process.Start(FilePath);
        }

        public string getAction(string actionName)
        {
            if (EventList == null)
                return null;
            foreach (xcActionInfo actionInfo in ActionList)
            {
                if (!string.IsNullOrEmpty(actionName) && actionName.Equals(actionInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return actionInfo.Action;
                }
            }
            return null;
        }

        private string getEventAction(string eventName, string target, Boolean isBatchMode = false) {
            if (EventList == null)
                return null;
            foreach (xcEventInfo eventInfo in EventList) {
                if (!string.IsNullOrEmpty(eventName) && eventName.Equals(eventInfo.Name)) {
                    if (!string.IsNullOrEmpty(target) && target.Equals(eventInfo.Target))
                        return isBatchMode ? eventInfo.BatchAction : eventInfo.Action;
                    else if (string.IsNullOrEmpty(target) && string.IsNullOrEmpty(eventInfo.Target))
                        return isBatchMode ? eventInfo.BatchAction : eventInfo.Action;
                }
            }
            return null;
        }

        /// <summary>
        /// Initialize the values
        /// </summary>
        private void initiailize() {
            HomeUrl = @"";
            Language = "zh-TW";

            PhsDomain = DEFAULT_PHS_DOMAIN; // "http://220.228.160.36/mehc-miphs/s/miphs";
            PhsAppDomain = DEFAULT_PHS_APP_DOMAIN; //  "http://220.228.160.36/mehc-mitac/s/miphs";
            HisDomain = DEFAULT_HIS_DOMAIN; // "https://test.mihis.com.tw/THESEHisSRV/STheseHis.asmx";

            KioskID = "";
            KioskPassword = "";

            HospitalID = "salesdemo";
            HospitalAuthKey = "123456";

            addRoom("01", "一診");
            addRoom("02", "二診");
            addRoom("03", "三診");
            addRoom("04", "四診");

            addOpdTime(1, "上午診", "0800-1220");
            addOpdTime(2, "下午診", "1330-1720");
            addOpdTime(3, "夜間診", "1730-2120");

            addAdv(1, "", 5000);
            addAdv(2, "", 5000);
            addAdv(3, "", 5000);
            addAdv(4, "", 5000);

            save();
        }
    }
}
