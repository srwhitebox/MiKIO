using SpringCard.PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiMPLib.xType;

namespace XiMPLib.xDevice.xCardReader {
    public class xcSmartCardReader : xcDevice {
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

        public static readonly byte[] MemoryCardAtrPrefix = new byte[] {
            0x3B,0x8F, 0x80, 0x01, 0x80, 0x4F, 0x0C, 0xA0, 0x00, 0x00, 0x03, 0x06
        };

        public enum ReaderState {
            CONNECTED = 0x00000000,       // Card reader has been connected
            DISCONNECTED = 0x00000005,       // Card reader has been disaconnected
            CARD_UNKNOWN = 0x00000220,       // Card has been inserted, but it is abnormal.
            CARD_REMOVED = 0x00000010,       // Card has been removed.
            CARD_INSERTED = 0x00000020,       // Card has been inserted
            CARD_UNPOWERED = 0x00000420,       // Card is exist, but the power has been turned off.
        }

        //public const uint STATE_CONNECTED = 0x00000000;
        //public const uint STATE_DISCONNECTED = 0x00000005;
        //public const uint STATE_CARD_UNKNOWN = 0x00000220;
        //public const uint STATE_CARD_REMOVED = 0x00000010;
        //public const uint STATE_CARD_INSERTED = 0x00000020;
        //public const uint STATE_CARD_UNPOWERED = 0x00000420;

        public class CardReaderEventArgs : EventArgs {
            public string ReaderName
            {
                get;
                set;
            }

            public ReaderState ReaderState
            {
                get;
                set;
            }

            public CardBuffer CardAttr
            {
                get;
                set;
            }

            public byte[] CardUid
            {
                get; set;
            }

            public xcNHICardInfo NhiCardInfo
            {
                get; set;
            }

            public CardReaderEventArgs(string readerName, uint readerState, CardBuffer cardAtrr) {
                this.ReaderName = readerName;
                this.ReaderState = (ReaderState)(readerState & 0x0000FFFF);
                this.CardAttr = cardAtrr;
            }

            public CardReaderEventArgs(string readerName, uint readerState, byte[] cardUid) {
                this.ReaderName = readerName;
                this.ReaderState = (ReaderState)(readerState & 0x0000FFFF);
                this.CardUid = cardUid;
            }

            public CardReaderEventArgs(string readerName, uint readerState, xcNHICardInfo nhiCardInfo) {
                this.ReaderName = readerName;
                this.ReaderState = (ReaderState)(readerState & 0x0000FFFF);
                this.NhiCardInfo = nhiCardInfo;
            }

        }

        public static xcNHICardInfo CardInfo
        {
            get; set;
        }

        public SCardReaderList CardReaderList
        {
            get; set;
        }

        public EventHandler OnStateChanged
        {
            get; set;
        }

        private uint PrevReaderState = SCARD.STATE_UNAWARE;
        public uint ShareMode = SCARD.SHARE_SHARED;
        private uint CardPrevStatus = SCARD.STATE_UNKNOWN;

        public xcSmartCardReader()
            : base("Castles Technology", "EZUSB PC/SC Smart Card Reader", DEVICE_TYPE.CARD_READER, DEVICE_INTERFACE.USB) {

        }

        public override void Open() {
            CardReaderList = new SCardReaderList(SCARD.SCOPE_SYSTEM, SCARD.ALL_READERS);
            CardReaderList.StartMonitor(new SCardReaderList.StatusChangeCallback(updateCardReaderState));
        }

        public override void Close() {
            if (CardReaderList != null) {
                CardReaderList.StopMonitor();
                CardReaderList = null;
            }
        }

        private void updateCardReaderState(string readerName, uint readerState, CardBuffer cardAttr) {
            if (CardPrevStatus == readerState)
                return;
            else
                CardPrevStatus = readerState;

            // If Card is present
            if ((readerState & SCARD.STATE_PRESENT) != 0) {
                // If Card in use
                if ((readerState & SCARD.STATE_INUSE) != 0) {
                    /* Card in exclusive use */
                    if ((readerState & SCARD.STATE_EXCLUSIVE) != 0) {
                        // In use (exclusive);
                        ShareMode = SCARD.SHARE_EXCLUSIVE;
                        //addLog("In use : Exclusive");
                    } else {
                        // In use (shared);
                        ShareMode = SCARD.SHARE_SHARED;
                        //addLog(readerName + " : In use : Shared");
                    }
                } else if ((readerState & SCARD.STATE_MUTE) != 0) { // Card is mute
                    onStateChanged(readerName, readerState, cardAttr);
                    //addLog(readerName + " : Card may not smart card or defected.");
                } else if ((readerState & SCARD.STATE_UNPOWERED) != 0) { // Card is not powered
                    onStateChanged(readerName, readerState, cardAttr);
                    // Present, not powered
                    //addLog(readerName + " : Card reading has been done.");
                } else {
                    // Card is powered
                    if ((PrevReaderState & SCARD.STATE_INUSE) == 0) {
                        OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, cardAttr));

                        SCardReader reader = new SCardReader(readerName);
                        SCardChannel channel = reader.GetChannel();
                        channel.Connect();
                        MemoryCardType cardType = getCardType(channel);
                        byte[] uid = null; // ReadUID(channel);
                        if (cardType == MemoryCardType.UNKNOWN) {
                            readNicCardInfo(readerName, SCARD.SHARE_SHARED);
                            if (CardInfo != null)
                                OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, CardInfo));
                            else
                            {
                                uid = ReadUID(channel);
                                if (uid != null)
                                {
                                    OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, uid));
                                }
                            }

                        } else {
                            uid = ReadUID(channel);
                            if (uid != null) {
                                OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, uid));
                            }
                        }
                        channel.Disconnect();
                    }
                }

            }
            else { // If Card is not present
                if ((readerState & SCARD.STATE_UNAVAILABLE) != 0) {
                    onStateChanged(readerName, (uint)ReaderState.DISCONNECTED, cardAttr);
                    //addLog("Reader has been disconnected :" + readerState);
                } else if ((readerState & SCARD.STATE_IGNORE) != 0) {
                    onStateChanged(readerName, readerState, cardAttr);
                    //addLog(readerName + " : Reader has been disconnected.");
                } else if ((readerState & SCARD.STATE_UNKNOWN) != 0) {
                    // Problem
                    // Error (status unknown)
                    //addLog("Error : Unknown :" + readerState);
                } else if ((readerState & SCARD.STATE_EMPTY) != 0) {
                    // No card
                    //Absent // check previous status
                    if (PrevReaderState == SCARD.STATE_UNAWARE || PrevReaderState == 65536) {
                        onStateChanged(readerName, readerState, cardAttr);
                        //addLog(readerName + " : CardReader is ready. Card is not exist.");
                    } else {
                        //addLog(readerName + " : Card has been removed.");
                        if (CardInfo != null)
                            CardInfo.reset();
                        onStateChanged(readerName, readerState, cardAttr);
                    }
                }
                else {
                    // Problem
                    // Bad status;
                    if (readerState == SCARD.STATE_UNAWARE) {
                        //addLog("Reader is not available.");
                    }
                    else {
                        onStateChanged(readerName, readerState, cardAttr);
                        //addLog("Reader has been connected.");
                    }
                }
            }

            PrevReaderState = readerState;
        }

        private MemoryCardType getCardType(SCardChannel Channel) {
            byte[] Atr = null;
            try {
                Atr = Channel.CardAtr.GetBytes();
            }
            catch {
                return MemoryCardType.UNKNOWN;
            }

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

        private byte[] ReadUID(SCardChannel Channel) {
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

        private void readNicCardInfo(string readerName, uint shareMode) {
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
            CardInfo = new xcNHICardInfo(rapdu);

            if (CardInfo.isNHICard()) {
                CardInfo.InfoBytes = rapdu.GetBytes();
                CardInfo.notifyChanged();
            }
            else {
                CardInfo = null;
                //addLog("The card may not be the NHI card.");
            }
        }

        private void onStateChanged(String readerName, uint readerState, CardBuffer cardAttr) {
            if (OnStateChanged != null)
                OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, cardAttr));
        }

        private void onStateChanged(string readerName, uint readerState, byte[] cardUid) {
            if (OnStateChanged != null)
                OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, cardUid));
        }

        private void onStateChanged(string readerName, uint readerState, xcNHICardInfo nhiCardInfo) {
            if (OnStateChanged != null)
                OnStateChanged(this, new CardReaderEventArgs(readerName, readerState, nhiCardInfo));
        }

    }
}
