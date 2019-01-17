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

namespace MitacHis.Database {
    public class OpdProgress : List<RoomInfo>{
        public  OpdTime OpdTime {
            get;
            set;
        }

        /// <summary>
        /// Hostpital ID
        /// ex) salesdemo
        /// </summary>
        public string HospitalId {
            get;
            set;
        }

        /// <summary>
        /// Authentification Key
        /// ex) 123456
        /// </summary>
        public string AuthKey {
            get;
            set;
        }


        /// <summary>
        /// Language
        /// ex) zh-TW
        /// </summary>
        public string Language {
            get {
                return "zh-TW";
            }
        }

        private List<string> RoomIDs {
            get {
                List<string> ids = new List<string>();
                foreach (RoomInfo roomInfo in this) {
                    ids.Add(roomInfo.roomID);
                }
                return ids;
            }
        }

        private Task perdiodicTask;
        private CancellationTokenSource cancelTokenSource;

        public RoomInfo this[string roomId] {
            get {
                var roomEnumerable = this.Where(room => room.roomID == roomId);
                return roomEnumerable.FirstOrDefault();
            }
        }

        public OpdProgress(string hospitalId, string authKey) {
            OpdTime = new OpdTime();
            this.HospitalId = hospitalId;
            this.AuthKey = authKey;
        }

        public void add(string roomId) {
            if (this[roomId] == null)
                Add(new RoomInfo(roomId));
        }

        public void updateRoomInfo(RoomInfo roomInfo){
            RoomInfo curRoom = this[roomInfo.roomID];
            if (curRoom.updateFrom(roomInfo)) {
                Sort();
            }
        }

        public void startMonitor() {
            cancelTokenSource = new CancellationTokenSource();
            perdiodicTask = PeriodicTaskFactory.Start(updateAsync, 60*1000, 0, Timeout.Infinite, -1, false, cancelTokenSource.Token, TaskCreationOptions.None);
            
            try {
                perdiodicTask.Wait();
            } catch (AggregateException e) {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(e.Message + " " + v.Message);
            } finally {
                cancelTokenSource.Dispose();
            }
        }

        public void stopMonitor() {
            cancelTokenSource.Cancel();
        }

        public async void updateAsync() {
            List<string> ids = new List<string>();
            foreach (RoomInfo roomInfo in this) {
                ids.Add(roomInfo.roomID);
            }

            foreach (string id in ids) {
                //string page = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&RoomID={5}&opdTimeID={6}", "https://thcloud.mihis.com.tw/THESEHisSRV/STheseHis.asmx", "GetOPDProgress", Language, HospitalId, AuthKey, id, OpdTime.Id);
                string page = string.Format("{0}/{1}?language={2}&hospitalID={3}&authKey={4}&RoomID={5}&opdTimeID={6}", "https://thcloud.mihis.com.tw/THESEHisSRV/STheseHis.asmx", "GetOPDProgress", Language, HospitalId, AuthKey, id, 1);
                using (HttpClient client = new HttpClient()) {
                    try {
                        System.IO.Stream stream = await client.GetStreamAsync(page);
                        DataSet ds = new DataSet();
                        ds.ReadXml(stream);

                        List<RoomInfo> rooms = xcDataSet.DataTableToList<RoomInfo>(ds.Tables[0]);
                        foreach (RoomInfo room in rooms) {
                            RoomInfo curRoom = this[room.roomID];
                            if (curRoom.updateFrom(room)) {
                                Sort();
                            }
                            Console.WriteLine(curRoom.roomID + " has been updated.");
                        }
                        stream.Close();
                    } catch (HttpRequestException e){
                        Console.WriteLine(id + " has no information in this time.");
                    }
                }
            }
        }
    }
}
