using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Printing;

namespace XiMPLib.xDevice.xPrinter {
    public class xcPrinter : xcDevice{

        public bool IsReady {
            get {
                return IsPrinterReady(Name);
            }
        }

        public bool IsInstalled {
            get {
                return this.Printer != null;
            }
        }

        public bool IsDefault {
            get {
                return IsInstalled ? this.Printer.GetPropertyValue("Default").Equals(true) : false;
            }
        }

        public bool IsNetwork {
            get {
                return IsInstalled ? this.Printer.GetPropertyValue("Network").Equals(true) : false;
            }
        }

        private ManagementBaseObject Printer {
            get;
            set;
        }

        private PrintDocument PrintDocument {
            get;
            set;
        }

        public PrinterSettings PrinterSettings {
            get {
                return this.PrintDocument != null ? this.PrintDocument.PrinterSettings : null;
            }
        }

        public xcPrinter(string vender="Unknown", string name="", DEVICE_INTERFACE deviceInterface = DEVICE_INTERFACE.USB)
            : base(vender, name, DEVICE_TYPE.PRINTER, deviceInterface) {

                if (string.IsNullOrEmpty(Name)) {
                    Name = DefaultPrinter;
                }

            var printerQuery = new ManagementObjectSearcher("SELECT * from Win32_Printer where Name = '" + Name + "'");
            try {
                foreach (var printer in printerQuery.Get()) {
                    this.Printer = printer;
                }
            } catch {

            }
        }

        public override void Open() {
            if (IsInstalled) {
                this.PrintDocument = new PrintDocument();
                this.PrintDocument.PrinterSettings.PrinterName = Name;

                
            }
        }

        public override void Close() {
            PrintDocument.Dispose();
        }

        public static bool IsPrinterReady(string printerName) {
            PrinterSettings settings = new PrinterSettings();
            settings.PrinterName = printerName;
            return settings != null ? settings.IsValid : false;
        }

        public static string DefaultPrinter{
            get {
                var printerQuery = new ManagementObjectSearcher("SELECT Name from Win32_Printer where Default = true");
                foreach (var printer in printerQuery.Get()) {
                    return (string)printer.GetPropertyValue("Name");
                }
                return null;
            }
        }
    }
}
