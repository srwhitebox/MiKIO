using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiHIS {
    /// <summary>
    /// Opd Time List
    /// </summary>
    public class OpdTimeList : List<OpdTime>{

        public OpdTime this[string opdTimeId] {
            get {
                if (string.IsNullOrEmpty(opdTimeId))
                    return new OpdTime("", "");
                var roomEnumerable = this.Where(opdTime => opdTime.Id.Equals(int.Parse(opdTimeId)));
                return roomEnumerable.FirstOrDefault();
            }
        }

        public OpdTime this[int opdTimeId] {
            get {
                var roomEnumerable = this.Where(opdTime => opdTime.Id.Equals(opdTimeId));
                return roomEnumerable.FirstOrDefault();
            }
        }

        /// <summary>
        /// Return current OpdTime
        /// </summary>
        public OpdTime CurOpdTime {
            get {
                foreach(OpdTime opdTime in this){
                    if (opdTime.isNow())
                        return opdTime;
                }
                return null;
            }
        }

        /// <summary>
        /// Add OpdTime item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void add(int id, string name, TimeSpan start, TimeSpan end) {
            OpdTime opdTime = new OpdTime(id, name, start, end);
            Add(opdTime);
        }
         
        /// <summary>
        /// Set the list as default
        /// </summary>
        public void setAsDefault(){
            add(1, "上午診", new TimeSpan(8, 0, 0), new TimeSpan(13, 29, 59));
            add(2, "下午診", new TimeSpan(13, 30, 0), new TimeSpan(17, 29, 59));
            add(3, "夜間診", new TimeSpan(0, 0, 0), new TimeSpan(7, 59, 59));
            add(3, "夜間診", new TimeSpan(17, 30, 0), new TimeSpan(23, 59, 59));
        }

        public void configTimeRange(DataTable table) {
            foreach (var row in table.AsEnumerable()) {
                string id = Convert.ChangeType(row["opdTimeID"], typeof(string)) as string;
                string range = Convert.ChangeType(row["opdPeriod"], typeof(string)) as string;
                OpdTime opdTime = this[id];
                if (opdTime == null){
                    opdTime = new OpdTime(int.Parse(id), "", new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0));
                    Add(opdTime);
                }
                opdTime.setRange(range);
            }
        }
    }
}
