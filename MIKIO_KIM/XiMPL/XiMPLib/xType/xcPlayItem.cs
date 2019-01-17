using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcPlayItem {
        public int ID {
            get;
            set;
        }

        public string Url {
            get;
            set;
        }

        public Uri Uri{
            get {
                return new Uri(Url);
            }
        }

        public TimeSpan StartTime {
            get;
            set;
        }

        public TimeSpan EndTime {
            get;
            set;
        }

        public int Duration {
            get;
            set;
        }


        public xcPlayItem(string id, string url, string length, string start, string end) {
            ID = int.Parse(id);
            Url = url;
            if (!string.IsNullOrEmpty(length)) {
                Duration = int.Parse(length);
            }
            if (!string.IsNullOrEmpty(start)) {
                StartTime = TimeSpan.Parse(start);
            }
            if (!string.IsNullOrEmpty(end)) {
                EndTime = TimeSpan.Parse(end);
            }
        }

        public xcPlayItem(int id, string url, int length) {
            this.ID = id;
            this.Url = url;
            this.Duration = length;
        }

        public xcPlayItem(int id, string url, TimeSpan start, TimeSpan end) {
            this.ID = id;
            this.Url = url;
            this.StartTime = start;
            this.EndTime = end;
        }
    }
}
