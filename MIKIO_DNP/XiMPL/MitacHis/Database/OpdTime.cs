using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XiMPLib.Type;

namespace MitacHis.Database {
    /// <summary>
    /// OPD Time Range class
    /// </summary>
    public class OpdTime {
        /// <summary>
        /// AM OPD Time range
        /// </summary>
        public TimeRange AmRange {
            get;
            set;
        }

        /// <summary>
        /// PM OPD Time range
        /// </summary>
        public TimeRange PmRange {
            get;
            set;
        }

        /// <summary>
        /// Return current time id;
        /// AM OPD : 1, PM OPD : 3, NIGHT TIME : 5
        /// </summary>
        public int Id {
            get{
                TimeSpan curTime = DateTime.Now.TimeOfDay;
                return AmRange.contains(curTime) ? 1 : PmRange.contains(curTime) ? 3 : 5;
            }
        }

        /// <summary>
        /// Constructor
        /// Default : 上午 8:00~13:30 下午 13:30~17:30，夜間 17:30~
        /// </summary>
        public OpdTime() {
            AmRange = new TimeRange(new TimeSpan(8, 0, 0), new TimeSpan(13, 29, 59));
            PmRange = new TimeRange(new TimeSpan(13, 30, 0), new TimeSpan(17, 29, 59));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amFrom"></param>
        /// <param name="amTo"></param>
        /// <param name="pmFrom"></param>
        /// <param name="pmTo"></param>
        public OpdTime(TimeSpan amFrom, TimeSpan amTo, TimeSpan pmFrom, TimeSpan pmTo) {
            AmRange = new TimeRange(amFrom, amTo);
            PmRange = new TimeRange(pmFrom, pmTo);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amRange"></param>
        /// <param name="pmRange"></param>
        public OpdTime(TimeRange amRange, TimeRange pmRange) {
            AmRange = amRange;
            PmRange = pmRange;
        }
    }
}
