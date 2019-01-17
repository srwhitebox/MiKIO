using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using XiMPLib.xBinding;
using XiMPLib.xDevice;
using XiMPLib.xType;
using XiMPLib.xUI;

namespace XiMPLib.MiHealth {
    public class MiHealthEyeTest : INotifyPropertyChanged {
        public enum STATE {
            GUIDE,
            HIDE,
            DO_TEST,
            ANSWERED,
            RESULT
        };

        public enum MODE {
            AUTO,
            MANUAL,
            RESULT
        };

        public enum EYE_SIDE {
            LEFT,
            RIGHT
        }

        public List<xcVisualAcuity> VisualAcuities
        {
            get
            {
                return xcBinder.AppProperties.VisualAcuities;
            }
        }

        public STATE state {
            get; set;
        }

        public EYE_SIDE curEye
        {
            get; set;
        }

        public MODE mode { get;set;}

        UIElement btnNext
        {
            get
            {
                return xcBinder.ControlDictionary["btnEyeTestNext"];
            }
        }
        UIElement btnEyeTestHome {
            get
            {
                return xcBinder.ControlDictionary["btnEyeTestHome"];
            }
        }

        xcTextView txtProgress {
            get
            {
                return (xcTextView)xcBinder.ControlDictionary["txtEyeTestProgress"];
            }
        }

        xUI.xcImage imgLandolt
        {
            get
            {
                return xcBinder.ControlDictionary == null ? null : (xUI.xcImage)xcBinder.ControlDictionary["imgEyeTestLandolf"];
            }
        }

        xUI.xcImage imgCorrect
        {
            get
            {
                if (xcBinder.ControlDictionary == null)
                    return null;
                return xcBinder.ControlDictionary.ContainsKey("imgEyeTestCorrect") ? (xUI.xcImage)xcBinder.ControlDictionary["imgEyeTestCorrect"] : null;
            }
        }

        xUI.xcImage imgIncorrect
        {
            get
            {
                if (xcBinder.ControlDictionary == null)
                    return null;
                return xcBinder.ControlDictionary.ContainsKey("imgEyeTestIncorrect") ? (xUI.xcImage)xcBinder.ControlDictionary["imgEyeTestIncorrect"] : null;
            }
        }


        xUI.xcImage ctrlGlasses
        {
            get
            {
                return xcBinder.ControlDictionary.ContainsKey("iconGlasses") ? (xUI.xcImage)xcBinder.ControlDictionary["iconGlasses"] : null;
            }
        }

        xUI.xcImage ctrlLeftHand
        {
            get
            {
                return xcBinder.ControlDictionary.ContainsKey("iconLeftHand") ? (xUI.xcImage)xcBinder.ControlDictionary["iconLeftHand"] : null;
            }
        }

        xUI.xcImage ctrlRightHand
        {
            get
            {
                return xcBinder.ControlDictionary.ContainsKey("iconRightHand") ? (xUI.xcImage)xcBinder.ControlDictionary["iconRightHand"] : null;
            }
        }


        xcBorder bdrLeftEye
        {
            get
            {
                return xcBinder.ControlDictionary.ContainsKey("ctrlEyeTestLeftEye") ? (xcBorder)xcBinder.ControlDictionary["ctrlEyeTestLeftEye"] : null;
            }
        }

        xcBorder bdrRightEye
        {
            get
            {
                return xcBinder.ControlDictionary.ContainsKey("ctrlEyeTestRightEye") ? (xcBorder)xcBinder.ControlDictionary["ctrlEyeTestRightEye"] : null;
            }
        }

        
        xcTextView txtTestingGrade
        {
            get
            {
                return (xcTextView)xcBinder.ControlDictionary["txtTestingGrade"];
            }
        }

        xcTextView txtTestResult
        {
            get
            {
                return (xcTextView)xcBinder.ControlDictionary["txtEyeTestResult"];
            }
        }

        xcTextView txtTestFinalResult
        {
            get
            {
                return (xcTextView)xcBinder.ControlDictionary["txtEyeTestFinalResult"];
            }
        }

        xcTextView txtTestGuide
        {
            get
            {
                return (xcTextView)xcBinder.ControlDictionary["txtEyeTestGuide"];
            }
        }
        

        public String RightEye
        {
            get
            {
                if (leftPassedIndex < 0)
                    leftPassedIndex = -1;

                if (leftPassedIndex < 0)
                    return null;

                return xcBinder.AppProperties.VisionTestNotation == xcVisualAcuity.NOTATION.DECIMAL ?
                                    xcBinder.AppProperties.TestGrades[leftPassedIndex] : xcBinder.AppProperties.VisualAcuities[leftPassedIndex].String;
            }
        }

        public String RightEyeValue
        {
            get
            {
                if (leftPassedIndex < 0)
                    leftPassedIndex = -1;

                if (leftPassedIndex < 0)
                    return null;

                return xcBinder.AppProperties.VisualAcuities[leftPassedIndex].DecimalString;
            }
        }

        public String RightGlassEye
        {
            get
            {
                if (leftGlassPassedIndex < 0)
                    leftGlassPassedIndex = -1;
                if (leftGlassPassedIndex < 0)
                    return null;

                return xcBinder.AppProperties.VisionTestNotation == xcVisualAcuity.NOTATION.DECIMAL ?
                                    xcBinder.AppProperties.TestGrades[leftGlassPassedIndex] : xcBinder.AppProperties.VisualAcuities[leftGlassPassedIndex].String;

            }
        }

        public String LeftEye
        {
            get
            {
                if (rightPassedIndex < 0)
                    rightPassedIndex = -1;
                if (rightPassedIndex < 0)
                    return "    ";

                return xcBinder.AppProperties.VisionTestNotation == xcVisualAcuity.NOTATION.DECIMAL ?
                                    xcBinder.AppProperties.TestGrades[rightPassedIndex] : xcBinder.AppProperties.VisualAcuities[rightPassedIndex].String;
            }
        }

        public String LeftEyeValue
        {
            get
            {
                if (rightPassedIndex < 0)
                    rightPassedIndex = -1;
                if (rightPassedIndex < 0)
                    return "    ";

                return xcBinder.AppProperties.VisualAcuities[rightPassedIndex].DecimalString;
            }
        }


        public String LeftGlassEye
        {
            get
            {
                if (rightGlassPassedIndex < 0)
                    rightGlassPassedIndex = -1;

                if (rightGlassPassedIndex < 0)
                    return "    ";

                return xcBinder.AppProperties.VisionTestNotation == xcVisualAcuity.NOTATION.DECIMAL ?
                                    xcBinder.AppProperties.TestGrades[rightGlassPassedIndex] : xcBinder.AppProperties.VisualAcuities[rightGlassPassedIndex].String;
            }
        }

        public double TestingGrade
        {
            get
            {
                if (testGradeIndex > VisualAcuities.Count - 1)
                    testGradeIndex = VisualAcuities.Count - 1;
                return VisualAcuities[testGradeIndex].Decimal;
            }
        }

        public String TestingGradeText
        {
            get
            {
                if (testGradeIndex > VisualAcuities.Count-1)
                    testGradeIndex = VisualAcuities.Count - 1;

                return xcBinder.AppProperties.VisionTestNotation == xcVisualAcuity.NOTATION.DECIMAL ?
                                    xcBinder.AppProperties.TestGrades[testGradeIndex] : xcBinder.AppProperties.VisualAcuities[testGradeIndex].String;
            }
        }

        public String TestResult
        {
            get; set;
        }

        xcLandolf landolf = new xcLandolf();
        private int leftFailed = 0;
        private int leftOk = 0;
        private int rightFailed = 0;
        private int rightOk = 0;

        private Boolean leftCompleted = false;
        private Boolean rightCompleted = false;

        private int testGradeIndex = -1;
        private int leftPassedIndex;
        private int leftGlassPassedIndex;
        private int leftFailedIndex;
        private int leftGlassFailedIndex;
        private int rightPassedIndex;
        private int rightGlassPassedIndex;
        private int rightFailedIndex;
        private int rightGlassFailedIndex;
        private bool withGlass = false;

        private DispatcherTimer autoProgressTimer;
        private const int MAX_COUNT = 15;
        private int TestingProgress
        {
            get; set;
        }

        private xcRemoteKeyboard remoteKeyboard;


        public MiHealthEyeTest() {
            remoteKeyboard = xcBinder.AppProperties.RemoteKeyboard;
            testGradeIndex = VisualAcuities.Count - 1;
            state = STATE.GUIDE;
            autoProgressTimer = new DispatcherTimer();
            autoProgressTimer.Tick += AutoProgressTimer_Tick;
            autoProgressTimer.Interval = new TimeSpan(0, 0, 1);

            //if (xcBinder.AppProperties.VisionTestTypeface.ToLower().StartsWith("e"))
            //{
            //    Uri typeUri = new System.Uri(imgLandolt.ParentUri, new Uri("image/ic_typeface_e.png", UriKind.RelativeOrAbsolute));
            //    imgLandolt.Source = new BitmapImage(typeUri);
            //}
        }

        private void AutoProgressTimer_Tick(object sender, EventArgs e) {
            TestingProgress--;
            OnPropertyChanged("TestingProgress");
            if (TestingProgress == 0) {
                TestingProgress = MAX_COUNT;
                if (state != STATE.RESULT)
                    calcGrade(false);
                else {
                    autoProgressTimer.Stop();
                    doEnd();
                }
            }
            if (String.IsNullOrEmpty(txtProgress.StringFormat))
                txtProgress.Text = TestingProgress.ToString();
            else
                txtProgress.Text = String.Format(txtProgress.StringFormat, TestingProgress);

        }

        public void doEnd() {
            autoProgressTimer.Stop();
            //Window.GetWindow(imgLandolf).KeyDown -= ImgLandolf_KeyDown;
            remoteKeyboard.DependencyObject = null;
            remoteKeyboard.OnActionKey = null;
            if (xcBinder.IsBatchMode)
            {
                initValues();
                setMode(MODE.AUTO);
            }
            xcBinder.doAction("xmpl:///action/home");
        }

        private String[] resultFormat;
        private String[] testGuide;

        public void initValues() {
            if (remoteKeyboard.DependencyObject == null) {
                remoteKeyboard.DependencyObject = imgLandolt;
                remoteKeyboard.OnActionKey = onActionKey;
            }

            if (testGuide == null) {
                testGuide = txtTestGuide.StringFormat.Split('|');
            }
            if (resultFormat == null) {
                resultFormat = txtTestResult.StringFormat.Split('|');
            }
            txtTestGuide.Text = testGuide[0];
            TestResult = null;

            TestingProgress = MAX_COUNT;
            curEye = EYE_SIDE.LEFT;

            leftCompleted = rightCompleted = false;
            leftFailed = rightFailed = 0;

            leftPassedIndex = leftGlassPassedIndex = rightPassedIndex = rightGlassPassedIndex = -1;
            leftFailedIndex = leftGlassFailedIndex =  rightFailedIndex = rightGlassFailedIndex = -1;

            state = MiHealthEyeTest.STATE.GUIDE;
            if (bdrLeftEye != null)
                bdrLeftEye.BorderBrush = bdrLeftEye.Background = new SolidColorBrush(Colors.Transparent);
            if (bdrRightEye != null)
                bdrRightEye.BorderBrush = bdrRightEye.Background = new SolidColorBrush(Colors.Transparent);

            btnEyeTestHome.Visibility = btnNext.Visibility = Visibility.Hidden;
            notifyChanged();
        }

        private void onActionKey(xcRemoteKeyboard.ACTION action) {
            Console.WriteLine("Action : " + action.ToString());
            switch (action) {
                case xcRemoteKeyboard.ACTION.NEXT:
                    if (state != STATE.DO_TEST)
                        this.doNext();
                    break;
                case xcRemoteKeyboard.ACTION.EYE_LEFT:
                    setCurEye(EYE_SIDE.LEFT);
                    break;
                case xcRemoteKeyboard.ACTION.EYE_RIGHT:
                    setCurEye(EYE_SIDE.RIGHT);
                    break;
                case xcRemoteKeyboard.ACTION.EYE_SWITCH:
                    switchEye();
                    break;
                case xcRemoteKeyboard.ACTION.GRADE_DOWN:
                    drawManualLevel(--this.testGradeIndex);
                    break;
                case xcRemoteKeyboard.ACTION.GRADE_UP:
                    drawManualLevel(++this.testGradeIndex);
                    break;
                case xcRemoteKeyboard.ACTION.GRADE_RENEW:
                    drawManualLevel(this.testGradeIndex);
                    break;
                case xcRemoteKeyboard.ACTION.SAVE:
                    showResult();
                    break;
                case xcRemoteKeyboard.ACTION.CONFIRM:
                    if (state != STATE.DO_TEST)
                        this.doNext();
                    else {
                        if (curEye == EYE_SIDE.LEFT)
                            leftPassedIndex = testGradeIndex;
                        else if (curEye == EYE_SIDE.RIGHT)
                            rightPassedIndex = testGradeIndex;

                        TestResult = String.Format(resultFormat[0], TestingGradeText, 1);
                        notifyChanged();
                        txtTestResult.Visibility = Visibility.Visible;
                    }
                    break;
                case xcRemoteKeyboard.ACTION.CANCEL:
                    doEnd();
                    break;
                case xcRemoteKeyboard.ACTION.UP:
                case xcRemoteKeyboard.ACTION.DOWN:
                case xcRemoteKeyboard.ACTION.LEFT:
                case xcRemoteKeyboard.ACTION.RIGHT:
                    ImgLandolfTest(action);
                    break;
            }
        }

        public void setMode(MODE mode) {
            switch (mode) {
                case MODE.AUTO:
                    this.mode = MODE.AUTO;
                    initValues();
                    txtProgress.Visibility = Visibility.Visible;
                    imgLandolt.Visibility = Visibility.Visible;

                    xcBinder.doAction("xmpl:///action/eyes_test_guide");
                    autoProgressTimer.Start();
                    break;
                case MODE.MANUAL:
                    this.mode = MODE.MANUAL;
                    initValues();
                    state = MiHealthEyeTest.STATE.GUIDE;
                    //Window.GetWindow(imgLandolf).KeyDown += ImgLandolf_KeyDown;
                    btnEyeTestHome.Visibility = btnNext.Visibility = Visibility.Visible;
                    imgLandolt.Visibility = Visibility.Hidden;
                    txtProgress.Visibility = Visibility.Hidden;
                    txtTestingGrade.Visibility = Visibility.Hidden;
                    xcBinder.doAction("xmpl:///action/eyes_test_guide");
                    break;
                case MODE.RESULT:
                    btnEyeTestHome.Visibility = Visibility.Visible;
                    btnNext.Visibility = Visibility.Hidden;
                    xcBinder.doAction("xmpl:///action/eyes_test_result");
                    break;
            }
        }

        public void doNext() {
            autoProgressTimer.Stop();
            switch (state) {
                case MiHealthEyeTest.STATE.GUIDE:
                    state = STATE.HIDE;
                    if (!leftCompleted) {
                        if (ctrlRightHand != null)
                            ctrlRightHand.Visibility = Visibility.Hidden;
                        if (ctrlLeftHand != null)
                            ctrlLeftHand.Visibility = Visibility.Visible;
                        if (bdrRightEye != null)
                            bdrRightEye.BorderBrush = bdrRightEye.Background = new SolidColorBrush(Colors.Transparent);
                        if (bdrLeftEye != null)
                        {
                            bdrLeftEye.BorderBrush = new SolidColorBrush(Colors.Red);
                            bdrLeftEye.Background = new SolidColorBrush(Colors.DimGray);
                        }
                    }
                    xcBinder.doAction("xmpl:///action/eyes_test_hide");
                    if (mode == MODE.AUTO)
                        autoProgressTimer.Start();
                    break;
                case MiHealthEyeTest.STATE.HIDE:
                    state = STATE.DO_TEST;
                    btnEyeTestHome.Visibility = btnNext.Visibility = Visibility.Hidden;
                    if (mode == MiHealthEyeTest.MODE.AUTO)
                        txtProgress.Visibility = Visibility.Visible;
                    xcBinder.doAction("xmpl:///action/eyes_test_do");
                    doTest();
                    break;
                case MiHealthEyeTest.STATE.DO_TEST:
                    if (!doTest()) {
                        showResult();
                    }
                    break;
                case STATE.RESULT:
                    doEnd();
                    break;
            }
        }

        private void showResult() {
            rightCompleted = true;
            testGradeIndex = 13;
            txtTestGuide.Text = testGuide[1];
            txtTestFinalResult.Text = String.Format(txtTestFinalResult.StringFormat, LeftEye, RightEye);

            state = MiHealthEyeTest.STATE.RESULT;
            TestingProgress = 10;
            autoProgressTimer.Start();
            btnEyeTestHome.Visibility = btnNext.Visibility = Visibility.Hidden;
            if (ctrlLeftHand != null)
                ctrlLeftHand.Visibility = Visibility.Hidden;
            if (ctrlRightHand != null)
                ctrlRightHand.Visibility = Visibility.Hidden;

            if (bdrLeftEye != null)
                bdrLeftEye.BorderBrush = bdrLeftEye.Background = new SolidColorBrush(Colors.Transparent);
            if (bdrRightEye != null)
                bdrRightEye.BorderBrush = bdrRightEye.Background = new SolidColorBrush(Colors.Transparent);
            txtProgress.Visibility = Visibility.Hidden;
            if (xcBinder.IsBatchMode)
            {
                xcBinder.doAction("xmpl:///action/batch_eyes_test_result");
            }
            else
            {
                xcBinder.doAction("xmpl:///action/eyes_test_result");
            }

            xcBinder.printEyes(LeftEye, RightEye);
            
            xcBinder.saveToMiHealth(LeftEyeValue, RightEyeValue);

            //remoteKeyboard.DependencyObject = null;
        }

        public Boolean doTest() {
            if (mode == MODE.AUTO) {
                TestingProgress = MAX_COUNT;
                if (leftCompleted & rightCompleted)
                    return false;
                autoProgressTimer.Start();
            }

            drawLandolt();
            //imgLandolf.Margin = imgLandolf.Margin = new Thickness(hMargin, imgLandolf.Margin.Top, hMargin, imgLandolf.Margin.Bottom);

            return true;
        }

        private void drawLandolt() {
            imgLandolt.Visibility = Visibility.Visible;
            if (imgCorrect != null)
                imgCorrect.Visibility = Visibility.Collapsed;
            if (imgIncorrect != null)
                imgIncorrect.Visibility = Visibility.Collapsed;

            landolf.setDistanceByMiliMeter(xcBinder.AppProperties.VisionTestDistance);

            imgLandolt.Height = imgLandolt.Width = xcBinder.AppProperties.VisualAcuities[testGradeIndex].Width;

            Random random = new Random();
            double randomX = random.Next(0, Math.Abs((int)(((Panel)imgLandolt.Parent).Width - imgLandolt.Height) - 100));
            double randomY = random.Next(0, Math.Abs((int)(((Panel)imgLandolt.Parent).Height - imgLandolt.Height) - 100)); ;

            double hMargin = (((Panel)imgLandolt.Parent).Width - imgLandolt.Height) / 2;
            
            Canvas.SetLeft(imgLandolt, randomX);
            Canvas.SetTop(imgLandolt, randomY);

            RotateTransform rotateTransform = new RotateTransform(90 * getNewAngle());
            imgLandolt.RenderTransform = rotateTransform;
        }

        int curAngle = -1;

        public event PropertyChangedEventHandler PropertyChanged;

        private void ImgLandolfTest(xcRemoteKeyboard.ACTION action) {
            if (action == xcRemoteKeyboard.ACTION.NEXT)
                doNext();

            if (imgLandolt.Visibility != Visibility.Visible)
                return;

            Boolean isCorrect = (action == xcRemoteKeyboard.ACTION.UP && curAngle == 0)
               || (action == xcRemoteKeyboard.ACTION.RIGHT && curAngle == 1)
               || (action == xcRemoteKeyboard.ACTION.DOWN && curAngle == 2)
               || (action == xcRemoteKeyboard.ACTION.LEFT && curAngle == 3);

            if (this.mode == MODE.AUTO) {
                if (state != STATE.DO_TEST)
                    return;
                calcGrade(isCorrect);
            }
            else {
                imgLandolt.Visibility = Visibility.Collapsed;
                if (imgIncorrect != null)
                    imgIncorrect.Visibility = Visibility.Collapsed;
                if (imgCorrect != null)
                    imgCorrect.Visibility = Visibility.Collapsed;

                if (isCorrect) {
                    if (imgCorrect != null)
                        imgCorrect.Visibility = Visibility.Visible;
                }
                else {
                    if (imgIncorrect != null)
                        imgIncorrect.Visibility = Visibility.Visible;
                }

                // Show result;
            }
        }

        private void drawManualLevel(int index) {
            if (index == -1)
                index = 0;
            if (index == VisualAcuities.Count)
                index = VisualAcuities.Count - 1;

            this.testGradeIndex = index;
            notifyChanged();
            drawLandolt();
            txtTestingGrade.Visibility = imgLandolt.Visibility = Visibility.Visible;
        }

        private void setCurEye(EYE_SIDE eye) {
            if (this.mode == MODE.AUTO) {
                return;
            }

            this.curEye = eye;
            txtTestingGrade.Visibility = txtTestResult.Visibility = Visibility.Hidden;
            imgLandolt.Visibility = Visibility.Hidden;
            switch (curEye) {
                case EYE_SIDE.LEFT:
                    if (ctrlRightHand != null)
                        ctrlRightHand.Visibility = Visibility.Hidden;
                    if (ctrlLeftHand != null)
                        ctrlLeftHand.Visibility = Visibility.Visible;
                    if (bdrRightEye != null)
                        bdrRightEye.BorderBrush = bdrRightEye.Background = new SolidColorBrush(Colors.Transparent);
                    if (bdrLeftEye != null)
                    {
                        bdrLeftEye.BorderBrush = new SolidColorBrush(Colors.Red);
                        bdrLeftEye.Background = new SolidColorBrush(Colors.DimGray);
                    }
                    break;
                case EYE_SIDE.RIGHT:
                    if (ctrlLeftHand != null)
                        ctrlLeftHand.Visibility = Visibility.Hidden;
                    if (ctrlRightHand != null)
                        ctrlRightHand.Visibility = Visibility.Visible;

                    if (bdrLeftEye != null)
                        bdrLeftEye.BorderBrush = bdrLeftEye.Background = new SolidColorBrush(Colors.Transparent);
                    if (bdrRightEye != null)
                    {
                        bdrRightEye.BorderBrush = new SolidColorBrush(Colors.Red);
                        bdrRightEye.Background = new SolidColorBrush(Colors.DimGray);
                    }
                    break;
            }

            drawManualLevel(this.testGradeIndex);
        }

        private void switchEye() {
            setCurEye(this.curEye == EYE_SIDE.LEFT ? EYE_SIDE.RIGHT : EYE_SIDE.LEFT);
        }

        private void calcGrade(bool isCorrect) {
            if (isCorrect) {
                leftFailed = rightFailed = 0;
                if (!leftCompleted) {
                    leftOk++;
                    if (leftOk < 3) {
                        TestResult = String.Format(resultFormat[0], TestingGradeText, leftOk);
                    }
                    else {
                        leftPassedIndex = testGradeIndex;
                        int remainedIndexes = (leftFailedIndex >= testGradeIndex ? leftFailedIndex - 1 : VisualAcuities.Count - 1) - testGradeIndex;

                        TestResult = String.Format(resultFormat[2], null, TestingGradeText);
                        leftOk = 0;

                        if (remainedIndexes <= 0) {
                            onLeftCompleted();
                        }
                        else if (remainedIndexes == 1) {
                            testGradeIndex += 1;
                        }
                        else
                            testGradeIndex += getMidLevel(remainedIndexes);

                    }
                }
                else { // doing right test
                    rightOk++;
                    if (rightOk < 3)
                        TestResult = String.Format(resultFormat[0], TestingGradeText, rightOk);
                    else {
                        rightPassedIndex = testGradeIndex;
                        int remainedIndexes = (rightFailedIndex >= testGradeIndex ? rightFailedIndex - 1 : VisualAcuities.Count - 1) - testGradeIndex;

                        TestResult = String.Format(resultFormat[2], null, TestingGradeText);
                        rightOk = 0;

                        if (remainedIndexes <= 0) {
                            onRightCompleted();
                        }
                        else if (remainedIndexes == 1) {
                            testGradeIndex += 1;
                        }
                        else
                            testGradeIndex += getMidLevel(remainedIndexes);

                    }
                }
            }
            else {
                leftOk = rightOk = 0;
                if (!leftCompleted) {
                    leftFailed++;
                    if (leftFailed < 3) {
                        TestResult = String.Format(resultFormat[1], TestingGradeText, leftFailed);
                    }
                    else {
                        leftFailedIndex = testGradeIndex;
                        if (testGradeIndex <= 1) {
                            testGradeIndex = 0;
                            leftPassedIndex = 0;
                            onLeftCompleted();
                        }
                        else {
                            int remainedIndexes = testGradeIndex == 1 ? 0 : testGradeIndex - (leftPassedIndex < testGradeIndex ? leftPassedIndex + 1 : 0);

                            TestResult = String.Format(resultFormat[3], null, TestingGradeText);
                            leftFailed = 0;

                            if (remainedIndexes == 0) {
                                onLeftCompleted();
                            }
                            else {
                                testGradeIndex -= getMidLevel(remainedIndexes);
                            }
                        }
                    }
                }
                else { // right testing
                    rightFailed++;
                    if (rightFailed < 3) {
                        TestResult = String.Format(resultFormat[1], TestingGradeText, rightFailed);
                    }
                    else {
                        rightFailedIndex = testGradeIndex;
                        if (testGradeIndex <= 1) {
                            testGradeIndex = 0;
                            rightPassedIndex = 0;
                            onRightCompleted();
                        }
                        else {
                            int remainedIndexes = testGradeIndex - (rightPassedIndex < testGradeIndex ? rightPassedIndex + 1 : 0);

                            TestResult = String.Format(resultFormat[3], null, TestingGradeText);
                            rightFailed = 0;

                            if (remainedIndexes == 0) {
                                onRightCompleted();
                            }
                            else {
                                testGradeIndex -= getMidLevel(remainedIndexes);
                            }
                        }
                    }
                }
            }

            notifyChanged();

            doNext();
        }

        private int getMidLevel(int remainedIndexes)
        {
            return (int)Math.Round(remainedIndexes / 2f, MidpointRounding.AwayFromZero);
        }

        private void onLeftCompleted() {
            leftCompleted = true;
            testGradeIndex = 13;
            TestResult = null;
            txtTestGuide.Text = testGuide[1];
            if (ctrlLeftHand != null)
                ctrlLeftHand.Visibility = Visibility.Hidden;
            if (ctrlRightHand != null)
                ctrlRightHand.Visibility = Visibility.Visible;

            if (bdrLeftEye != null)
                bdrLeftEye.BorderBrush = bdrLeftEye.Background = new SolidColorBrush(Colors.Transparent);
            if (bdrRightEye != null)
            {
                bdrRightEye.BorderBrush = new SolidColorBrush(Colors.Red);
                bdrRightEye.Background = new SolidColorBrush(Colors.DimGray);
            }
            state = MiHealthEyeTest.STATE.GUIDE;
            //xcBinder.doAction("xmpl:///action/eyes_test_hide");
            //if (mode == MODE.AUTO)
            //    autoProgressTimer.Start();
            notifyChanged();
        }

        private void onRightCompleted() {
            rightCompleted = true;
            testGradeIndex = 13;
            txtTestGuide.Text = testGuide[1];

            txtTestFinalResult.Text = String.Format(txtTestFinalResult.StringFormat, LeftEye, RightEye);
            if (bdrRightEye != null)
                bdrRightEye.BorderBrush = bdrRightEye.Background = new SolidColorBrush(Colors.Transparent);
            //state = MiHealthEyeTest.STATE.RESULT;
        }

        private int getNewAngle() {
            if (curAngle == -1) {
                curAngle = new Random().Next(0, 4);
            }
            else {
                int newAngle = -1;
                do {
                    newAngle = new Random().Next(0, 4);
                } while (newAngle == curAngle);
                curAngle = newAngle;
            }
            return curAngle;
        }


        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        private BitmapSource bimtmapSourceE = null;
        private BitmapSource BimtmapSourceE
        {
            get
            {
                if (bimtmapSourceE != null)
                    return bimtmapSourceE;

                int thickness = 200;
                int width = thickness * 5;
                System.Drawing.Image img = new Bitmap(width, width);
                Graphics drawing = Graphics.FromImage(img);
                
                drawing = Graphics.FromImage(img);

                //paint the background
                drawing.Clear(System.Drawing.Color.Transparent);

                //create a brush for the text
                System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.Black);
                drawing.FillRectangle(brush, 0, 0, width, thickness);
                drawing.FillRectangle(brush, 0, thickness*2, width, thickness);
                drawing.FillRectangle(brush, 0, thickness*4, width, thickness);
                drawing.FillRectangle(brush, 0, 0, thickness, width);
                drawing.Save();

                brush.Dispose();
                drawing.Dispose();


                
                var bitmap = new Bitmap(img);
                IntPtr bmpPt = bitmap.GetHbitmap();
                BitmapSource bitmapSource =
                 System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       bmpPt,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());

                //freeze bitmapSource and clear memory to avoid memory leaks
                bitmapSource.Freeze();
                //DeleteObject(bmpPt);

                bimtmapSourceE = bitmapSource;

                return bitmapSource;
            }
        }
    }
}
