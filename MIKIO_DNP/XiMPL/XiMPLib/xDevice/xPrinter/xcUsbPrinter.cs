using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace XiMPLib.xDevice.xPrinter {
    /// <summary>
    /// Class for USB Printer
    /// </summary>
    public class xcUsbPrinter {
        /// <summary>
        /// Printer Name which is match with the name in printer lists
        /// </summary>
        public String Name {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printerName"></param>
        public xcUsbPrinter(String printerName) {
            Name = printerName;
        }


        /// <summary>
        /// Print bytes array
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public Boolean print(byte[] contents){
            IntPtr pBytes = Marshal.UnsafeAddrOfPinnedArrayElement(contents, 0);
            Int32 dwBytes = contents.Length;
            Boolean result =  print(Name, pBytes, dwBytes);
            Marshal.FreeCoTaskMem(pBytes);
            return result;
        }


        /// <summary>
        /// Print string
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public Boolean print(string contents, String encoding = "utf-8") {
            byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(contents);
            return print(bytes);
        }

        /// <summary>
        /// send the printer data to the printer
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount"></param>
        /// <returns></returns>
        protected static bool print(string szPrinterName, IntPtr pBytes, Int32 dwCount) {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.
            di.pDocName = "XiMPL raw doc";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero)) {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di)) {
                    // Start a page.
                    if (StartPagePrinter(hPrinter)) {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false) {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

    }

}
