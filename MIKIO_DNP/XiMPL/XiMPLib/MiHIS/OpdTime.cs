using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xType;

namespace XiMPLib.MiHIS {
    /// <summary>
    /// OPD Time item
    /// </summary>
    public class OpdTime : TimeRange{
        /// <summary>
        /// Time ID
        /// </summary>
        public int Id {
            get;
            set;
        }

        /// <summary>
        /// Time Name
        /// </summary>
        public string Name {
            get;
            set;
        }

        public string RangeString {
            get {
                return string.Format("{0:hhmm}-{1:hhmm}", Start, End);
            }
        }

        public TimeRange CheckinRange {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public OpdTime(int id, string name, TimeSpan start, TimeSpan end) : base(start, end) {
            setID(id);
            Name = name;
        }

        public OpdTime(int id, string name, string timeRange, string checkinRange=null) : base(new TimeSpan(), new TimeSpan()) {
            setID(id);
            Name = name;
            setRange(timeRange);
            if (!string.IsNullOrEmpty(checkinRange))
                setCheckinRange(checkinRange);
        }

        public OpdTime(int id, string name)
            : base(new TimeSpan(), new TimeSpan()) {
            setID(id);
            Name = name;
        }

        public OpdTime(string id, string name)
            : base(new TimeSpan(), new TimeSpan()) {
            setID(id);
            Name = name;
        }

        /// <summary>
        /// Is current time is in this time range?
        /// </summary>
        /// <returns></returns>
        public bool isNow() {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return contains(new TimeSpan(now.Hours, now.Minutes, now.Seconds));
        }

        public bool isAfter() {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now > this.End;
        }

        public bool canCheckin() {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return CheckinRange==null ? true : now < CheckinRange.End;
        }

        public void setCheckinRange(string range) {
            if (!string.IsNullOrEmpty(range))
                this.CheckinRange = new TimeRange(range);
        }

        private void setID(int id) {
            this.Id = id;
        }

        private void setID(string id) {
            if (!string.IsNullOrEmpty(id))
                this.Id = int.Parse(id);
        }
    }
}
