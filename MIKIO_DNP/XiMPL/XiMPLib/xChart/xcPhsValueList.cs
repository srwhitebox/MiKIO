using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.MiPHS;
using XiMPLib.xBinding;

namespace XiMPLib.xChart {
    public class xcPhsValueList : SortedList {
        public string Field {
            get;
            set;
        }

        public DateTime FromDate {
            get;
            set;
        }

        public DateTime ToDate {
            get;
            set;
        }

        public double MinValue {
            get;
            set;
        }

        public double MaxValue {
            get;
            set;
        }

        public WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE PeriodType
        {
            get;
            set;
        }

        public String DataSource {
            get; set;
        }

        public delegate void LoadListCompletedCallBack(xcPhsValueList list);

        public LoadListCompletedCallBack OnLoadListCompleted {
            get;
            set;
        }

        public xcPhsValueList(String dataSource="phs") {
            DataSource = dataSource;
        }

        public void loadList(string field, WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE periodType) {
            PeriodType = periodType;
            DateTime toDate = DateTime.Today;
            DateTime fromDate;
            switch (periodType) {
                case WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.WEEK:
                    fromDate = toDate.AddDays(-7);
                    break;
                case WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.MONTH:
                    fromDate = toDate.AddMonths(-1);
                    break;
                case WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.QUATER:
                    fromDate = toDate.AddMonths(-3);
                    break;
                case WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.YEAR:
                    fromDate = toDate.AddYears(-1);
                    break;
                default:
                    fromDate = toDate;
                    break;
            }
            loadList(field, fromDate, toDate);
        }

        public void loadList(string field, DateTime fromDate, DateTime toDate) {
            this.Clear();
            int totalDays = (int)((toDate - fromDate).TotalDays);

            if ( totalDays > 31 && totalDays < 100)
            {
                PeriodType = WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.WEEK;
            }else if ((toDate - fromDate).TotalDays > 100)
            {
                PeriodType = WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.MONTH;
            }
            else
            {
                PeriodType = WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.DAY;
            }

            Field = field;
            FromDate = fromDate;
            ToDate = toDate;

            getMeasurementRecord(field, fromDate, toDate, getRecordsCallback);
        }
        private void getMeasurementRecord(String field, DateTime fromDate, DateTime toDate, Action<List<xcPhsMeasurementRecord>> getRecordCallback) {
            switch (DataSource.ToLower()) {
                case "phs":
                case "miphs":
                    xcBinder.MiPhs.getMeasurementRecord(field, fromDate, toDate, getRecordsCallback);
                    break;
                case "mihealth":
                case "campus":
                case "micampus":
                    xcBinder.MiHealth.getMeasurementRecord(field, fromDate, toDate, getRecordsCallback);
                    break;
            }
        }

        private void loginCallback(bool loggedIn) {
            if (loggedIn) {
                xcBinder.MiPhs.getMeasurementRecord(Field, FromDate, ToDate, getRecordsCallback);
            } else {
                OnLoadListCompleted(null);
            }
        }

        private void getRecordsCallback(List<xcPhsMeasurementRecord> listMeasurementRecord) {
            if (listMeasurementRecord == null)
                return;
            MinValue = 9999999;
            MaxValue = 0;
            foreach(xcPhsMeasurementRecord record in listMeasurementRecord){
                xcPhsValue value = new xcPhsValue(Field, record);
                if (PeriodType == WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.WEEK)
                {
                    addByWeek(value);
                }
                else if(PeriodType == WXiMPLib.xChart.xcPhsChart.PERIOD_TYPE.MONTH)
                {
                    addByMonth(value);
                } 
                else
                {
                    addByDate(value);
                }
                if (value.Value < MinValue)
                {
                    MinValue = value.Value;
                }
                if (value.Value > MaxValue)
                {
                    MaxValue = value.Value;
                }
            }

            if (OnLoadListCompleted != null)
                OnLoadListCompleted(this);
        }

        private void addByDate(xcPhsValue value)
        {
            xcPhsValue existValue = (xcPhsValue)this[value.MeasuredDateTime.Date];
            if (existValue == null)
            {
                Add(value.MeasuredDateTime.Date, value);
            }
            else
            {
                if (value.MeasuredDateTime > existValue.MeasuredDateTime)
                {
                    this.Remove(value.MeasuredDateTime.Date);
                    Add(value.MeasuredDateTime.Date, value);
                }
            }
        }

        private static DateTimeFormatInfo DFI = DateTimeFormatInfo.CurrentInfo;
        private static Calendar Calendar = DFI.Calendar;

        private void addByWeek(xcPhsValue value)
        {
            int weekNum = getWeekNum(value.MeasuredDateTime);

            xcPhsValue existValue = null;
            foreach (DateTime itemDate in this.Keys)
            {
                int  itemWeekNum = getWeekNum(itemDate);
                if (weekNum == itemWeekNum)
                {
                    existValue = (xcPhsValue)this[itemDate];
                    break;
                }
            }

            //xcPhsValue existValue = (xcPhsValue)this[value.MeasuredDateTime.Date];

            if (existValue == null)
            {
                Add(value.MeasuredDateTime.Date, value);
            }
            else
            {
                if (value.MeasuredDateTime > existValue.MeasuredDateTime)
                {
                    this.Remove(value.MeasuredDateTime.Date);
                    Add(value.MeasuredDateTime.Date, value);
                }
            }
        }

        private void addByMonth(xcPhsValue value)
        {
            int month = value.MeasuredDateTime.Month;

            xcPhsValue existValue = null;
            foreach (DateTime itemDate in this.Keys)
            {
                int itemMonth = itemDate.Month;
                if (month == itemMonth)
                {
                    existValue = (xcPhsValue)this[itemDate];
                    break;
                }
            }

            //xcPhsValue existValue = (xcPhsValue)this[value.MeasuredDateTime.Date];

            if (existValue == null)
            {
                Add(value.MeasuredDateTime.Date, value);
            }
            else
            {
                if (value.MeasuredDateTime > existValue.MeasuredDateTime)
                {
                    this.Remove(value.MeasuredDateTime.Date);
                    Add(value.MeasuredDateTime.Date, value);
                }
            }
        }

        private int getWeekNum(DateTime date)
        {
            return Calendar.GetWeekOfYear(date, DFI.CalendarWeekRule, DFI.FirstDayOfWeek);
        }
    }
}
