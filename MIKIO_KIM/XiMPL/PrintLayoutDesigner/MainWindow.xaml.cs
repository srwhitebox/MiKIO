using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XiMPLib.xBinding;
using XiMPLib.xIO;
using XiMPLib.xType;

namespace PrintLayoutDesigner {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        private string[] recordTypes = {"Fat Record", "Blood Pressure", "Fat Composition Record", "Registration Info"};
        private List<string> LayoutFiles = new List<string>();
        private AppProperties AppProperties;
        private Dictionary<string, object> dataDictionary = new Dictionary<string,object>();

        private string LayoutRoot {
            get {
                return System.IO.Path.Combine(xcPath.DocumentPath, @"print");
            }
        }

        public MainWindow() {
            InitializeComponent();

            initializeDataSources();
        }

        private void initializeDataSources() {
            comboRecordType.SelectionChanged += comboRecordType_SelectionChanged;
            comboRecordType.ItemsSource = recordTypes;
            comboRecordType.SelectedItem = recordTypes[0];
            listupLayouts();
            comboLayotFile.ItemsSource = LayoutFiles;
            comboLayotFile.SelectedIndex = 0;
            fillFatRecord();
            dataGridDatas.ItemsSource = dataDictionary;
        }

        void comboRecordType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            switch ((string)comboRecordType.SelectedItem) {
                case "Fat Record":
                    fillFatRecord();
                    break;
                case "Blood Pressure":
                    fillBpRecord();
                    break;
                case "Fat Composition Record":
                    fillFatCompositionRecord();
                    break;
                case "Registration Info":
                    fillRegInfoRecord();
                    break;
                default:
                    dataDictionary.Clear();
                    break;
            }
            dataGridDatas.Items.Refresh();
            
        }

        private void listupLayouts() {
            LayoutFiles.Clear();
            string[] files = System.IO.Directory.GetFiles(LayoutRoot, "*.xpl");
            if (files != null) {
                foreach(string file in files){
                    LayoutFiles.Add(file);
                }
            }
        }

        private void fillFatRecord() {
            dataDictionary.Clear();
            dataDictionary.Add("measured_date", DateTime.Now);
            dataDictionary.Add("height", 1678.34);
            dataDictionary.Add("weight", 64.7);
            dataDictionary.Add("bmi", 23.2);
        }

        private void fillBpRecord() {
            dataDictionary.Clear();
            dataDictionary.Add("measured_date", DateTime.Now);
            dataDictionary.Add("systolic_pressure", 126);
            dataDictionary.Add("diastolic_pressure", 79);
            dataDictionary.Add("pulse", 80);
        }

        private void fillFatCompositionRecord() {
            dataDictionary.Clear();

            dataDictionary.Add("measured_date", DateTime.Now);
            dataDictionary.Add("weight", 68.9);
            dataDictionary.Add("height", 168);
            dataDictionary.Add("body_fat_percentage", 19.8);
            dataDictionary.Add("fat_mass", 13.6);
            dataDictionary.Add("fat_free_mass", 55.3);
            dataDictionary.Add("body_water_mass", 56.3);
            dataDictionary.Add("body_age", 29);
            dataDictionary.Add("bmi", 24.4);
            dataDictionary.Add("bmr", 6523);
            dataDictionary.Add("bmr_by_kj", 6523);
            dataDictionary.Add("bmr_by_kcal", 1559);

            dataDictionary.Add("body_type", "S");
            dataDictionary.Add("gender", 'M');

            dataDictionary.Add("impedance_whole_body", 551);
            dataDictionary.Add("impedance_right_leg", 212);
            dataDictionary.Add("impedance_left_leg", 214);
            dataDictionary.Add("impedance_right_arm", 292);
            dataDictionary.Add("impedance_left_arm", 309);

            dataDictionary.Add("right_leg_body_fat_percentage", 8.9);
            dataDictionary.Add("right__leg_fat_mass", 1.1);
            dataDictionary.Add("right_leg_fat_free_mass", 11.5);
            dataDictionary.Add("right_legm_uscle_mass", 10.9);

            dataDictionary.Add("left_leg_body_fat_percentage", 10.1);
            dataDictionary.Add("left_leg_fat_mass", 1.2);
            dataDictionary.Add("left_leg_fat_free_mass", 11.0);
            dataDictionary.Add("left_leg_muscle_mass", 10.4);

            dataDictionary.Add("right_arm_body_fat_percentage", 14.0);
            dataDictionary.Add("right_arm_fat_mass", 0.6);
            dataDictionary.Add("right_arm_fat_free_mass", 3.6);
            dataDictionary.Add("right_arm_muscle_mass", 3.4);

            dataDictionary.Add("left_arm_body_fat_percentage", 15.4);
            dataDictionary.Add("left_arm_fat_mass", 0.6);
            dataDictionary.Add("left_arm_fat_free_mass", 3.5);
            dataDictionary.Add("left_arm_muscle_mass", 3.3);

            dataDictionary.Add("trunk_body_fat_percentage", 15.0);
            dataDictionary.Add("trunk_fat_mass", 6.0);
            dataDictionary.Add("trunk_fat_free_mass", 34.1);
            dataDictionary.Add("trunk_muscle_mass", 34.1);

            dataDictionary.Add("visceral_fat_raiting", 12);

            dataDictionary.Add("desirible_body_fat_percentage_range", new FloatRange(11.0f, 21.9f));
            dataDictionary.Add("desirible_fat_mass_range", new FloatRange(6.8f, 15.5f));

            dataDictionary.Add("comment", "正常");

        }

        private void fillRegInfoRecord() {
            dataDictionary.Clear();

            dataDictionary.Add("patient_name", "朴鍾恩");
            dataDictionary.Add("patient_id", "AC12345678");
            dataDictionary.Add("opd_date", DateTime.Now);
            dataDictionary.Add("reg_date", DateTime.Now);
            dataDictionary.Add("opd_time_name", "下午診");
            dataDictionary.Add("dept_name", "家醫科");
            dataDictionary.Add("room_id", "01");
            dataDictionary.Add("room_name", "三診");
            dataDictionary.Add("doctor_name", "蕭敦仁");
            dataDictionary.Add("reg_number", 5);
            dataDictionary.Add("memo", "Memo...");
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            XiMPLib.xDocument.xcPrintDocument pd = new XiMPLib.xDocument.xcPrintDocument(new Uri((string)comboLayotFile.SelectedItem));
            System.Globalization.CultureInfo ciTaiwan = System.Globalization.CultureInfo.CreateSpecificCulture("zh-TW");

            //dataDictionary.Add("measured_date", "量測日期:" + ((DateTime)dataDictionary["measured_date"]).ToString(ciTaiwan.DateTimeFormat));
            foreach (string key in dataDictionary.Keys) {
                //if (!key.Equals("measured_date"))
                    pd.addModelAttribute(key, dataDictionary[key]);
            }
            
            pd.print();
        }
    }
}
