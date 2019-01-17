using MitacHis.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.MiHIS.Database {
    public class RoomList : List<RoomInfo>{
        public RoomInfo this[string roomID] {
            get {
                var roomEnumerable = this.Where(room => room.ID.Equals(roomID));
                return roomEnumerable.FirstOrDefault();
            }
        }
    }
}
