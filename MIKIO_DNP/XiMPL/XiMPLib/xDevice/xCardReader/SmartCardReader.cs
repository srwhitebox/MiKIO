using SpringCard.PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xType;

namespace XiMPLib.xDevice.xCardReader {
    public enum MemoryCardType {
        MIFARE_CLASSIC_1K = 0x0001,
        MIFARE_CLASSIC_4K = 0x0002,
        MIFARE_UL = 0x0003,
        ST_MICRO_ST176 = 0x0006,
        ST_MICRO_OTHER_SR = 0x0007,
        ATMEL_AT88SC0808CRF = 0x000A,
        ATMEL_AT88SC1616CRF = 0x000B,
        ATMEL_AT88SC3216CRF = 0x000C,
        ATMEL_AT88SC6416CRF = 0x000D,
        TEXAS_TAGIT = 0x0012,
        ST_MICRO_LRI512 = 0x0013,
        NXP_ICODE_SLI = 0x0014,
        NXP_ICODE1 = 0x0016,
        ST_MICRO_LRI64 = 0x0021,
        ST_MICRO_LR12 = 0x0024,
        ST_MICRO_LRI128 = 0x0025,
        MIFARE_MINI = 0x0026,
        INNOVISION_JEWEL = 0x002F,
        INNOVISION_TOPAZ = 0x0030,
        ATMEL_AT88RF04C = 0x0034,
        NXP_ICODE_SL2 = 0x0035,
        MIFARE_UL_C = 0x003A,
        GENERIC_14443_A = 0xFFA0,
        GENERIC_14443_B = 0xFFB0,
        ASK_CTS_256B = 0xFFB1,
        ASK_CTS_512B = 0xFFB2,
        INSIDE_CONTACTLESS = 0xFFB7,
        UNIDENTIFIED_ATMEL = 0xFFB8,
        CALYPSO_INNOVATRON = 0xFFC0,
        UNIDENTIFIED_ISO15693 = 0xFFD0,
        UNIDENTIFIED_EMMARIN = 0xFFD1,
        UNIDENTIFIED_ST_MICRO = 0xFFD2,
        UNKNOWN = 0xFFFF
    }

    public static class SmartCardReader {
        public static readonly byte[] MemoryCardAtrPrefix = new byte[] {
            0x3B,
            0x8F,
            0x80,
            0x01,
            0x80,
            0x4F,
            0x0C,
            0xA0,
            0x00,
            0x00,
            0x03,
            0x06
        };

        public static xcNHICardInfo mCardInfo = new xcNHICardInfo();
        public static SCardReaderList mCardReaderList = new SCardReaderList(SCARD.SCOPE_SYSTEM, SCARD.ALL_READERS);
        public static uint PrevReaderState = SCARD.STATE_UNAWARE;
        public static uint ShareMode = SCARD.SHARE_SHARED;
        private static bool isMonitorStarted = false;

        public static void startCardReaderMonitor() {
            if (isMonitorStarted)
                return;
            isMonitorStarted = true;

            stopCardReaderMonitor();

            mCardReaderList.StartMonitor(new SCardReaderList.StatusChangeCallback(cardReaderStateChanged));
        }

        public static void stopCardReaderMonitor() {
            if (mCardReaderList != null) {
                mCardReaderList.StopMonitor();
            }
            isMonitorStarted = false;
        }

        delegate void cardReaderStateChangedInvoker(string readerName, uint readerState, CardBuffer cardAtr);
        static void cardReaderStateChanged(string readerName, uint readerState, CardBuffer cardAtr) {
            updateCardReaderState(readerName, readerState, cardAtr);
        }

        private static uint mCardPrevStatus = SCARD.STATE_UNKNOWN;
        static void updateCardReaderState(string readerName, uint readerState, CardBuffer cardAtr) {
            if (mCardPrevStatus == readerState)
                return;
            else
                mCardPrevStatus = readerState;

            // If Card is present
            if ((readerState & SCARD.STATE_PRESENT) != 0) {
                // If Card in use
                if ((readerState & SCARD.STATE_INUSE) != 0) {
                    /* Card in exclusive use */
                    if ((readerState & SCARD.STATE_EXCLUSIVE) != 0) {
                        // In use (exclusive);
                        ShareMode = SCARD.SHARE_EXCLUSIVE;
                        //addLog("In use : Exclusive");
                    }
                    else {
                        // In use (shared);
                        ShareMode = SCARD.SHARE_SHARED;
                        //addLog(readerName + " : In use : Shared");
                    }
                }
                else
                    // Card is mute
                    if ((readerState & SCARD.STATE_MUTE) != 0) {
                    //addLog(readerName + " : Card may not smart card or defected.");
                }
                else
                        // Card is not powered
                        if ((readerState & SCARD.STATE_UNPOWERED) != 0) {
                    // Present, not powered
                    //addLog(readerName + " : Card reading has been done.");
                }
                else {
                    // Card is powered
                    if ((PrevReaderState & SCARD.STATE_INUSE) == 0) {
                        SCardReader reader = new SCardReader(readerName);
                        SCardChannel channel = reader.GetChannel();
                        channel.Connect();
                        MemoryCardType cardType = getCardType(channel);
                        if (cardType == MemoryCardType.UNKNOWN)
                            readNicCardInfo(readerName, SCARD.SHARE_SHARED);
                        else {
                            byte[] uid = ReadUID(channel);
                            if (uid != null) {
                                string hex = string.Join("", uid.Select(bin => bin.ToString("X2")).ToArray());
                            }
                        }
                        channel.Disconnect();
                    }
                }

            }
            else { // If Card is not present
                if ((readerState & SCARD.STATE_UNAVAILABLE) != 0) {
                    // Problem.. 
                    // Reserved (direct) 
                    //addLog("Reader has been disconnected :" + readerState);
                }
                else
                    if ((readerState & SCARD.STATE_IGNORE) != 0) {
                    // Problem
                    // Error (ignore)
                    //addLog(readerName + " : Reader has been disconnected.");

                }
                else
                        if ((readerState & SCARD.STATE_UNKNOWN) != 0) {
                    // Problem
                    // Error (status unknown)
                    //addLog("Error : Unknown :" + readerState);
                }
                else
                            if ((readerState & SCARD.STATE_EMPTY) != 0) {
                    // No card
                    //Absent // check previous status
                    if (PrevReaderState == SCARD.STATE_UNAWARE || PrevReaderState == 65536) {
                        //addLog(readerName + " : CardReader is ready. Card is not exist.");
                    }
                    else {
                        //addLog(readerName + " : Card has been removed.");
                        mCardInfo.reset();
                    }
                }
                else {
                    // Problem
                    // Bad status;
                    if (readerState == SCARD.STATE_UNAWARE) {
                        //addLog("Reader is not available.");
                    }
                    else {
                        //addLog("Reader has been connected.");
                    }
                }
            }

            PrevReaderState = readerState;
        }

        private static MemoryCardType getCardType(SCardChannel Channel) {
            byte[] Atr = Channel.CardAtr.GetBytes();
            if (Atr == null)
                return MemoryCardType.UNKNOWN; /* ATR error */
            if (Atr.Length < MemoryCardAtrPrefix.Length + 3)
                return MemoryCardType.UNKNOWN;

            for (int i = 0; i < MemoryCardAtrPrefix.Length; i++)
                if (Atr[i] != MemoryCardAtrPrefix[i])
                    return MemoryCardType.UNKNOWN; /* ATR doesn't denote a memory card */

            var PixSS = Atr[MemoryCardAtrPrefix.Length];

            var PixNN = new byte[2];
            PixNN[0] = Atr[MemoryCardAtrPrefix.Length + 1];
            PixNN[1] = Atr[MemoryCardAtrPrefix.Length + 2];

            ushort wPixNN = (ushort)((PixNN[0] * 0x0100) | PixNN[1]);

            try {
                return (MemoryCardType)wPixNN;
            }
            catch {
                return MemoryCardType.UNKNOWN;
            }
        }

        private static byte[] ReadUID(SCardChannel Channel) {
            CAPDU capdu = new CAPDU(0xFF, 0xCA, 0x00, 0x00, 0x00);

            RAPDU rapdu = null;

            rapdu = Channel.Transmit(capdu);

            Channel.Disconnect();

            if (rapdu == null) {
                return null;
            }

            if (rapdu.SW != 0x9000) {
                return null;
            }

            if (!rapdu.hasData) {
                return null;
            }
            return rapdu.data.GetBytes();
        }

        private static void readNicCardInfo(string readerName, uint shareMode) {
            SCardChannel channel;
            channel = new SCardChannel(readerName);
            channel.ShareMode = shareMode;
            channel.Protocol = (uint)(SCARD.PROTOCOL_T0 | SCARD.PROTOCOL_T1);
            if (shareMode == SCARD.SHARE_DIRECT) {
                /* DIRECT mode opens Control page */
                /* ------------------------------ */
            }
            else {
                /* SHARED or EXCLUSIVE mode opens Transmit page */
                /* -------------------------------------------- */
            }

            if (!channel.Connect()) {
                // ShowError;
                return;
            }

            //Send command to prepare the information
            CAPDU capdu = new CAPDU(XiMPLib.xType.xcNHICardInfo.CmdSelectAdpu);
            RAPDU rapdu = channel.Transmit(capdu);
            //Send command to read information
            capdu = new CAPDU(XiMPLib.xType.xcNHICardInfo.CmdReadCardInfo);
            rapdu = channel.Transmit(capdu);

            //Dispatch to NHI card information
            xcNHICardInfo cardInfo = new XiMPLib.xType.xcNHICardInfo(rapdu);

            if (cardInfo.isNHICard()) {
                mCardInfo.InfoBytes = rapdu.GetBytes();

                mCardInfo.notifyChanged();
                //addLog(cardInfo.ToString());
                
            }
            else {
                //addLog("The card may not be the NHI card.");
            }
        }
    }
}
