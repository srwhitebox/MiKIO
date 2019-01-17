using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using XiMPLib.xBinding;

namespace XiMPLib.xNetwork {
    public delegate void OnNetworkConnectionChanged(Boolean isConnected);

    /// <summary>
    /// Network management class
    /// </summary>
    public class xcNetwork : INotifyPropertyChanged {

        public bool IsAvailable {
            get {
                return isAvailable();
            }
        }

        public bool IsNotAvailable
        {
            get
            {
                return !this.IsAvailable;
            }
        }

        public IPAddress IpAddress {
            get {
                return getCurIp();
            }
        }

        public string MacAddress {
            get {
                return getCurMacAddress();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public OnNetworkConnectionChanged OnNetworkConnectionChanged
        {
            get; set;
        }

        public xcNetwork() {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e) {
            notifyChanged();
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e) {
            notifyChanged();
            xcBinder.onNetworkChanged(e.IsAvailable);
            if (OnNetworkConnectionChanged != null)
                OnNetworkConnectionChanged(e.IsAvailable);
        }

        private void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public static bool isAvailable() {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        /// <summary>
        /// Returns the MacAddress as String
        /// </summary>
        /// <returns></returns>
        public static String getCurMacAddress(){
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                if (nic.OperationalStatus == OperationalStatus.Up) {
                    return nic.GetPhysicalAddress().ToString();
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Returns the Current IP Address
        /// </summary>
        /// <returns></returns>
        public static IPAddress getCurIp() {
            String host = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(host);
            foreach (IPAddress ipAddr in ipEntry.AddressList) {
                if (ipAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ipAddr;
            }
            return null;
        }

    }
}
