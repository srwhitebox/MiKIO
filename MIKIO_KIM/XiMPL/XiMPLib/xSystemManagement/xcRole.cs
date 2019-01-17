using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace XiMPLib.xSystemManagement {
    public class xcRole {
        /// <summary>
        /// Determine current user is administrator
        /// </summary>
        /// <returns></returns>
        public static Boolean isAdmin() {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
