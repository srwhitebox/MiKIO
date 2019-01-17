using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using XiMPLib.xBinding;
using SpringCard.PCSC;

namespace XiMPLib.xType {
    public class xcNHICardInfo : INotifyPropertyChanged {
        public static byte[] CmdSelectAdpu = { 0x00, 0xA4, 0x04, 0x00, 0x10, 0xD1, 0x58, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00 };
        public static byte[] CmdReadCardInfo = { 0x00, 0xCA, 0x11, 0x00, 0x02, 0x00, 0x00 };
        private Encoding Encoding = Encoding.GetEncoding("Big5");

        private byte[] vertualInfo = {
            (byte)'1', (byte)'2', (byte)'3', (byte)'4',(byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'1', (byte)'2',
            (byte)'K', (byte)'I', (byte)'M', (byte)' ',(byte)'D', (byte)'E', (byte)'O', (byte)'K', (byte)'H', (byte)'A', (byte)'N', (byte)'\0', (byte)'\0', (byte)'\0', (byte)'\0', (byte)'\0', (byte)'\0', (byte)'\0', (byte)'\0', (byte)'\0',
            //(byte)'Q', (byte)'1', (byte)'2', (byte)'2',(byte)'5', (byte)'6', (byte)'3', (byte)'3', (byte)'5', (byte)'9', (byte)'\0',
            //(byte)'6', (byte)'7', (byte)'0', (byte)'5',(byte)'2', (byte)'8',
            (byte)'D', (byte)'1', (byte)'2', (byte)'2',(byte)'0', (byte)'6', (byte)'3', (byte)'6', (byte)'2', (byte)'1', (byte)'\0',
            (byte)'8', (byte)'0', (byte)'1', (byte)'0',(byte)'2', (byte)'1',
            (byte)'M',
            (byte)'8', (byte)'8', (byte)'8',
            (byte)'M',
            (byte)'\0',
            (byte)'5', (byte)'9', (byte)'0', (byte)'1',(byte)'0', (byte)'7'
        };
        
        public byte[] InfoBytes {
            get;
            set;
        }
        public string CardId {
            get {
                return getString(0, 12);
            }
        }

        public string HolderName {
            get {
                return getString(12, 20).TrimEnd(new char[]{'\0'});
            }
        }

        public string HolderIDN {
            get {
                return getString(32, 10);
            }
        }

        public string HolderIDNMasked {
            get {
                if (string.IsNullOrEmpty(HolderIDN))
                    return string.Empty;
                string value = HolderIDN.Substring(0, HolderIDN.Length - 4);
                return value + new string('*', 4);
            }
        }

        public char Gender {
            get {
                try {
                    return (char)InfoBytes[49];
                } catch {
                    return (char)0xff;
                }
            }
        }

        public DateTime BirthDate {
            get {
                try {
                    int year = int.Parse(getString(43, 2)) + 1911;
                    int month = int.Parse(getString(45, 2));
                    int day = int.Parse(getString(47, 2));
                    return new DateTime(year, month, day);
                } catch {
                    return new DateTime();
                }
            }
        }

        public DateTime IssuedDate {
            get {
                try {
                    int year = int.Parse(getString(51, 2)) + 1911;
                    int month = int.Parse(getString(53, 2));
                    int day = int.Parse(getString(55, 2));
                    return new DateTime(year, month, day);
                } catch {
                    return new DateTime();
                }
            }
        }

        public bool HasCard {
            get {
                return isNHICard();
            }
        }

        public xcNHICardInfo() {

        }

        public xcNHICardInfo(RAPDU rapdu) {
            try {
                InfoBytes = rapdu.GetBytes();
            } catch {

            }
        }

        public Boolean isNHICard() {
            return InfoBytes != null && !string.IsNullOrWhiteSpace(CardId) && !string.IsNullOrWhiteSpace(HolderIDN);
        }

        private string getString(int index, int count) {
            try {
                if (InfoBytes == null)
                    return string.Empty;
                return this.Encoding.GetString(InfoBytes, index, count);
            } catch {
                return string.Empty;
            }
        }

        public void reset() {
            InfoBytes=null;
            notifyChanged();
        }

        public override string ToString() {
            JObject jCardInfo = new JObject();
            jCardInfo.Add("cardID", CardId);
            jCardInfo.Add("holderIDN", HolderIDN);
            jCardInfo.Add("holderName", HolderName);
            jCardInfo.Add("gender", Gender.ToString());
            jCardInfo.Add("birthDate", BirthDate.ToShortDateString());
            jCardInfo.Add("issuedDate", IssuedDate.ToShortDateString());

            return jCardInfo.ToString();
        }

        public void notifyChanged() {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties) {
                OnPropertyChanged(property.Name);
            }
            //OnPropertyChanged("CardId");
            //OnPropertyChanged("HolderIDN");
            //OnPropertyChanged("HolderIDNMasked");
            //OnPropertyChanged("HolderName");
            //OnPropertyChanged("Gender");
            //OnPropertyChanged("BirthDate");
            //OnPropertyChanged("HasCard");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void simulateInserted() {
            this.InfoBytes = vertualInfo;
            this.notifyChanged();

            xcBinder.CardInfo.InfoBytes = vertualInfo;
            xcBinder.Patient.set(xcBinder.CardInfo);
            xcBinder.CardInfo.notifyChanged();
            xcBinder.MiPhs.login(xcBinder.phsLoginCallBack);
            xcBinder.RegInfoList.loads();

            // xcBinder.MiPhs.login(xcBinder.CardInfo);
            // xcBinder.RegInfoList.loads();

        }

        public void simulateRemoved() {
            xcBinder.onSmartCardRemoved();
        }

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
