using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XiMPLib.xUI;

namespace XiMPLib.xSystem {
    public class VirtualKey{
        public byte VKey { get; set; }

        public bool Shift { get; set; }

        public VirtualKey(char ch, bool shift = false) {
            VKey = (byte)ch;
            Shift = shift;
        }

        public VirtualKey(byte vKey, bool shift = false) {
            VKey = vKey;
            Shift = shift;
        }
    }

     class KeyMap : Dictionary<char, VirtualKey>{
         public VirtualKey this[char ch] {
             get {
                 if (Char.IsLetterOrDigit(ch)) {
                     return new VirtualKey(Char.ToUpper(ch), Char.IsUpper(ch));
                 }
                 
                 return this.ContainsKey(ch) ? base[ch] : new VirtualKey(ch);
             }
         }

         public void Add(char ch, char vKey, bool shift = false){
             this.Add(ch, new VirtualKey(vKey, shift));
         }
         public void Add(char ch, byte vKey, bool shift = false) {
             this.Add(ch, new VirtualKey(vKey, shift));
         }

     }

    public class xcKeyEvent {
        public const byte VK_SHIFT = 0x10;
        public const byte VK_CONTROL = 0x11;
        public const byte VK_MENU = 0x12;

        private static KeyMap KeyMap = new KeyMap() {
            {'\n', 0x0D},
            {'\r', 0x0D},
            {'\t', 0x09},
            {'`', 0xC0},
            {'~', 0xC0, true},
            {'!', '1', true},
            {'@', '2', true},
            {'#', '3', true},
            {'$', '4', true},
            {'%', '5', true},
            {'^', '6', true},
            {'&', '7', true},
            {'*', '8', true},
            {'(', '9', true},
            {')', '0', true},
            {'-', 0xBD},
            {'_', 0xBD, true},
            {'=', 0xBB},
            {'+', 0xBB, true},
            {'[', 0xDB},
            {'{', 0xDB, true},
            {']', 0xDD},
            {'}', 0xDD, true},
            {'\\', 0xDC},
            {'|', 0xDC, true},
            {';', 0xBA},
            {':', 0xBA, true},
            {'\'', 0xDE},
            {'"', 0xDE, true},
            {',', 0xBC},
            {'<', 0xBC, true},
            {'.', 0xBE},
            {'>', 0xBE, true},
            {'/', 0xBF},
            {'?', 0xBF, true},
            {' ', 0x20},

        };

        private static Dictionary<string, byte> FnKeyMap = new Dictionary<string, byte>() {
            {"lbutton", 0x01},
            {"rbutton", 0x02},
            {"cancel", 0x03},
            {"mbutton", 0x04},
            {"back", 0x08},
            {"tab", 0x09},
            {"clear", 0x0C},
            {"enter", 0x0D},
            {"\r\n", 0x0D},
            {"return", 0x0D},
            {"shift", 0x10},
            {"ctrl", 0x11},
            {"control", 0x11},
            {"alt", 0x12},
            {"menu", 0x12},
            {"pause", 0x13},
            {"capslock", 0x14},
            {"captial", 0x14},
            {"kana", 0x15},
            {"hanguel", 0x15},
            {"hangul", 0x15},
            {"junja", 0x17},
            {"final", 0x18},
            {"hanja", 0x19},
            {"kanji", 0x19},
            {"esc", 0x1B},
            {"escape", 0x1B},
            {"convert", 0x1C},
            {"noconvert", 0x1D},
            {"accept", 0x1E},
            {"modechange", 0x1F},
            {"space", 0x20},
            {"pageup", 0x21},
            {"pgup", 0x21},
            {"prior", 0x21},
            {"pagedown", 0x22},
            {"pgdn", 0x22},
            {"next", 0x22},
            {"end", 0x23},
            {"home", 0x24},
            {"left", 0x25},
            {"up", 0x26},
            {"right", 0x27},
            {"down", 0x28},
            {"select", 0x29},
            {"print", 0x2A},
            {"execute", 0x2B},
            {"prtsc", 0x2C},
            {"printscreen", 0x2C},
            {"snapshot", 0x2C},
            {"ins", 0x2D},
            {"insert", 0x2D},
            {"del", 0x2E},
            {"delete", 0x2E},
            {"help", 0x2F},
            {"lwin", 0x5B},
            {"lwindow", 0x5B},
            {"rwin", 0x5C},
            {"rwindow", 0x5C},
            {"apps", 0x5D},
            {"sleep", 0x5F},
            {"num0", 0x60},
            {"numpad0", 0x60},
            {"num1",    0x61},
            {"numpad1", 0x61},
            {"num2",    0x62},
            {"numpad2", 0x62},
            {"num3",    0x63},
            {"numpad3", 0x63},
            {"num4",    0x64},
            {"numpad4", 0x64},
            {"num5",    0x65},
            {"numpad5", 0x65},
            {"num6",    0x66},
            {"numpad6", 0x66},
            {"num7",    0x67},
            {"numpad7", 0x67},
            {"num8",    0x68},
            {"numpad8", 0x68},
            {"num9",    0x69},
            {"numpad9", 0x69},
            {"multiply", 0x6A},
            {"plus",    0x6B},
            {"add", 0x6B},
            {"separator", 0x6C},
            {"minus", 0x6D},
            {"subtractor", 0x6D},
            {"decimal", 0x6E},
            {"divide", 0x6F},
            {"f1", 0x70},
            {"f2", 0x71},
            {"f3", 0x72},
            {"f4", 0x73},
            {"f5", 0x74},
            {"f6", 0x75},
            {"f7", 0x76},
            {"f8", 0x77},
            {"f9", 0x78},
            {"f10", 0x79},
            {"f11", 0x7A},
            {"f12", 0x7B},
            {"f13", 0x7C},
            {"f14", 0x7D},
            {"f15", 0x7E},
            {"f16", 0x7F},
            {"f17", 0x80},
            {"f18", 0x81},
            {"f19", 0x82},
            {"f20", 0x83},
            {"f21", 0x84},
            {"f22", 0x85},
            {"f23", 0x86},
            {"f24", 0x87},
            {"numlock", 0x90},
            {"scroll", 0x91},
            {"scrlk", 0x91},
            {"scrolllock", 0x91},            
            {"lshift", 0xA0},
            {"rshift", 0xA1},
            {"lcontrol", 0xA2},
            {"lctrl", 0xA2},
            {"rcontrol", 0xA3},
            {"rctrl", 0xA3},
            {"lmenu", 0xA4},
            {"lalt", 0xA4},
            {"rmenu", 0xA5},
            {"ralt", 0xA5},
            {"browser_back", 0xA6},
            {"browser_foward", 0xA7},
            {"browser_refresh", 0xA8},
            {"browser_stop", 0xA9},
            {"browser_search", 0xAA},
            {"browser_favorite", 0xAB},
            {"browser_home", 0xAC},
            {"volume_mute", 0xAD},
            {"volume_down", 0xAE},
            {"volume_up", 0xAF},
            {"media_next", 0xB0},
            {"media_prev", 0xB1},
            {"media_stop", 0xB2},
            {"mdeia_play_pause", 0xB3},
            {"mail", 0xB4},
            {"media_select", 0xB5},
            {"app1", 0xB6},
            {"app2", 0xB7},
            {"play", 0xFA},
            {"zoom", 0xFB},
        };

        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public static void raiseKeybdUpDownEvent(char ch) {
            var keyMap = KeyMap[ch];
            raiseKeybdUpDownEvent(keyMap.VKey, keyMap.Shift);
        }

        public static void raiseKeybdUpDownEvent(string content) {
            foreach (char ch in content) {
                raiseKeybdUpDownEvent(ch);
            }
        }

        public static void raiseFnKeyUpDownEvent(string keyName) {
            if (keyName == null || string.IsNullOrEmpty(keyName))
                return;
            keyName = keyName.ToLower();
            if (FnKeyMap.ContainsKey(keyName))
                raiseKeybdUpDownEvent(FnKeyMap[keyName]);
        }

        public static void raiseKeybdUpDownEvent(byte vKey) {
            if (vKey == 0x0C) {
                clearValue();
            } else {
                keybd_event(vKey, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                keybd_event(vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        private static void clearValue() {
            var focusedElement = Keyboard.FocusedElement;
            var type = focusedElement.GetType();
            if (type.Equals(typeof(TextBox)) || type.Equals(typeof(xcEditText))) {
                TextBox textBox = (TextBox)focusedElement;
                textBox.Clear();
            } else if (type.Equals(typeof(PasswordBox))) {
                PasswordBox password = (PasswordBox)focusedElement;
                password.Clear();
            }
        }

        public static void raiseKeybdUpDownEvent(byte vKey, bool shift, bool ctrl=false, bool alt=false){

            if (shift)
                keybd_event(VK_SHIFT, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            if (ctrl)
                keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            if (alt)
                keybd_event(VK_MENU, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            
            keybd_event(vKey, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            keybd_event(vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            
            if (shift)
                keybd_event(VK_SHIFT, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            if (ctrl)
                keybd_event(VK_CONTROL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            if (alt)
                keybd_event(VK_MENU, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
    }
}
