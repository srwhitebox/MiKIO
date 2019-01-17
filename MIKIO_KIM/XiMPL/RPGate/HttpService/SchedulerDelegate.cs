using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kayak;
using Kayak.Http;

namespace RPGate.HttpService {
    public class SchedulerDelegate : ISchedulerDelegate {
        public void OnException(IScheduler scheduler, Exception e) {
            e.DebugStackTrace();
        }

        public void OnStop(IScheduler scheduler) {

        }
    }
}
