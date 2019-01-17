using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Net.Http;
using System.Data;

using XiMPLib.Type;
using XiMPLib.xThreading;
using System.Threading;
using XiMPLib.MiKIO;
using XiMPLib.MiHIS;
using XiMPLib.xBinding;
using XiMPLib.xType;

namespace MitacHis.Database {
    /// <summary>
    /// OpdProgress
    /// List for Room Info
    /// </summary>
    public class OpdProgress : List<RoomInfo>{
        public List<RoomInfo> DisplaySource {
            get;
            set;
        }

        private List<RoomInfo> CurOpdProgress {
            get;
            set;
        }


        /// <summary>
        /// Return Room Info which the room ID is
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public RoomInfo this[string roomId] {
            get {
                var roomEnumerable = this.Where(room => room.ID == roomId);
                return roomEnumerable.FirstOrDefault();
            }
        }

        private List<string> RoomIDs {
            get {
                List<string> ids = new List<string>();
                foreach (RoomInfo roomInfo in this) {
                    ids.Add(roomInfo.ID);
                }
                return ids;
            }
        }

        /// <summary>
        /// Return latest room
        /// </summary>
        public RoomInfo Latest {
            get {
                return Count > 0 ? this[0] : null;
            }
        }

        public OpdTimeList OpdTimeList {
            get;
            set;
        }

        private Task perdiodicTask;
        private CancellationTokenSource cancelTokenSource;
        private System.Timers.Timer OpdPrgoressRotateTimer;
        private const int RotateInterval = 6000;
        public OpdProgress() {
            DisplaySource = new List<RoomInfo>();
            AppProperties Properties = new AppProperties();
            var rooms = Properties.RoomList;
            if (rooms != null) {
                foreach (RoomInfo room in rooms) {
                    Add(room);
                    DisplaySource.Add(new RoomInfo(room.ID));
                }
            } else {
                setAsDefault();
            }

            OpdTimeList = new OpdTimeList();

            var opdTimes = Properties.OpdTimeList;
            if (opdTimes != null) {
                foreach (OpdTime opdTime in opdTimes) {
                    OpdTimeList.Add(opdTime);
                }
            } else {
                OpdTimeList.setAsDefault();
            }

            OpdPrgoressRotateTimer = new System.Timers.Timer(RotateInterval);
            OpdPrgoressRotateTimer.Elapsed += OpdPrgoressRotateTimer_Elapsed;
        }

        void OpdPrgoressRotateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            OpdPrgoressRotateTimer.Stop();
            List<RoomInfo> rotatedList = new List<RoomInfo>();
            for (int i = 1; i < CurOpdProgress.Count; i++) {
                rotatedList.Add(CurOpdProgress[i]);
            }
            rotatedList.Add(CurOpdProgress[0]);
            CurOpdProgress = rotatedList;
            setDisplaySource();
            notifyAll();
            OpdPrgoressRotateTimer.Start();
        }

        /// <summary>
        /// Add a room with ID
        /// </summary>
        /// <param name="roomId"></param>
        public void add(string roomId, string name = "", string title = "") {
            if (this[roomId] == null)
                Add(new RoomInfo(roomId, name, title, OpdTimeList));
        }

        /// <summary>
        /// Find the room and update the information
        /// Sort by the updated time
        /// </summary>
        /// <param name="roomInfo"></param>
        public RoomInfo updateRoomInfo(RoomInfo roomInfo){
            RoomInfo curRoom = this[roomInfo.ID];
            if (curRoom.update(roomInfo)) {
                Sort();
            }
            return curRoom;
        }

        public RoomInfo updateRoomInfo(DataTable table) {
            RoomInfo curRoom = null;
            bool hasUpdate = false;
            int i = 0;
            try
            {
                foreach (var row in table.AsEnumerable())
                {
                    string id = Convert.ChangeType(row["roomID"], typeof(string)) as string;
                    curRoom = this[id];
                    if (curRoom != null)
                        hasUpdate = curRoom.update(row);
                    i++;
                }
            }catch
            {
                return curRoom;
            }

            if (hasUpdate) {
                OpdPrgoressRotateTimer.Stop();
                Sort();

                if (CurOpdProgress == null)
                    CurOpdProgress = new List<RoomInfo>();
                else
                    CurOpdProgress.Clear();
                for (int j = 0; j < i; j++) {
                    CurOpdProgress.Add(this[j]);
                }
                setDisplaySource();
                notifyAll();
                OpdPrgoressRotateTimer.Start();
            }

            return curRoom;
        }

        private void setDisplaySource() {
            for (int i = 0; i < this.Count && i < CurOpdProgress.Count; i++) {
                xcObject.copyProperties(DisplaySource[i], CurOpdProgress[i]);
            }
        }

        public void startMonitor() {
        }

        public void stopMonitor() {
        }

        // add the default rooms
        public void setAsDefault() {
            for (int i = 1; i <= 4; i++ )
                this.add("0"+i);
        }

        public void notifyAll() {
            foreach (RoomInfo info in DisplaySource) {
                info.notifyChanged();
            }
        }
    }
}
