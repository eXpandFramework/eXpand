using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;

namespace Xpand.Utils.Helpers {
    public sealed class KeyShortcut {
        public static KeyShortcut Empty = new KeyShortcut();
        readonly Keys _keys;

        public KeyShortcut(Shortcut shortcut)
            : this((Keys)shortcut) {
        }

        public KeyShortcut()
            : this(Keys.None) {
        }

        public KeyShortcut(Keys key) {
            _keys = CheckKey(key, false);
        }

        public Keys Key {
            get { return _keys; }
        }

        public bool IsExist {
            get {
                if (Key == Keys.None || !IsValidShortcut(Key)) return false;
                return true;
            }
        }

        public static string AltKeyName {
            get { return GetModifierKeyName(Keys.Alt); }
        }

        public static string ShiftKeyName {
            get { return GetModifierKeyName(Keys.Shift); }
        }

        public static string ControlKeyName {
            get { return GetModifierKeyName(Keys.Control); }
        }

        public override string ToString() {
            if (this == Empty) return "(none)";
            if (!IsExist) return "";
            string res = GetKeyDisplayText(Key);
            return res;
        }

        Keys CheckKey(Keys key, bool isSecond) {
            Keys v = IsValidShortcut(key) ? key : Keys.None;
            if (isSecond) {
                if (Key == Keys.None) v = Keys.None;
            }
            return v;
        }

        bool IsValidShortcut(Keys key) {
            if (key == Keys.None) return false;
            key = key & (~Keys.Modifiers);
            if (key == Keys.None || key == Keys.ControlKey || key == Keys.ShiftKey || key == Keys.Alt) return false;
            return true;
        }

        public static string GetKeyDisplayText(Keys key) {
            string res = "";
            if (key == Keys.None) return res;
            if ((key & Keys.Control) != 0 || key == Keys.ControlKey) res = ControlKeyName;
            if ((key & Keys.Shift) != 0 || key == Keys.ShiftKey) res += (res.Length > 0 ? "+" : "") + ShiftKeyName;
            if ((key & Keys.Alt) != 0 || key == Keys.Alt) res += (res.Length > 0 ? "+" : "") + AltKeyName;
            key = key & (~Keys.Modifiers);
            if (key != Keys.None) res += (res.Length > 0 ? "+" : "") + key.ToString();
            return res;
        }

        static string GetModifierKeyName(Keys key) {
            string keyName = new KeysConverter().ConvertToString(key);
            if (keyName != null) {
                int index = keyName.IndexOf("+", StringComparison.Ordinal);
                if (index == -1) return keyName;
                return keyName.Substring(0, index);
            }
            return null;
        }

        public static bool operator ==(KeyShortcut left, KeyShortcut right) {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null)) return false;
            if (ReferenceEquals(right, null)) return false;
            return (left.Key == right.Key);
        }

        public static bool operator !=(KeyShortcut left, KeyShortcut right) {
            return !(left == right);
        }

        public override bool Equals(object value) {
            var shcut = value as KeyShortcut;
            if (shcut == null) return false;
            return _keys == shcut.Key;
        }

        public override int GetHashCode() {
            // ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
            // ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
        }
    }

    public class KeysConverter : TypeConverter, IComparer {
        List<string> displayOrder;
        IDictionary keyNames;
        StandardValuesCollection values;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public KeysConverter() {
        }

        IDictionary KeyNames {
            get {
                if (keyNames == null)
                    Initialize();
                return keyNames;
            }
        }

        IEnumerable<string> DisplayOrder {
            get {
                if (displayOrder == null)
                    Initialize();
                return displayOrder;
            }
        }

        public int Compare(object a, object b) {
            return string.Compare(ConvertToString(a), ConvertToString(b), false, CultureInfo.InvariantCulture);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof (string) || sourceType == typeof (Enum[]))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof (Enum[]))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            var s = value as string;
            if (s != null) {
                string str = s.Trim();
                if (str.Length == 0)
                    return null;
                string[] strArray = str.Split(new[]{
                    '+'
                });
                for (int index = 0; index < strArray.Length; ++index)
                    strArray[index] = strArray[index].Trim();
                var keys1 = Keys.None;
                bool flag = false;
                foreach (string t in strArray) {
                    object obj = KeyNames[t] ?? Enum.Parse(typeof (Keys), t);
                    if (obj != null) {
                        var keys2 = (Keys) obj;
                        if ((keys2 & Keys.KeyCode) != Keys.None) {
                            if (flag)
                                throw new FormatException("KeysConverterInvalidKeyCombination");
                            flag = true;
                        }
                        keys1 |= keys2;
                    }
                    else
                        throw new FormatException("KeysConverterInvalidKeyName");
                }
                return keys1;
            }
            if (!(value is Enum[]))
                return base.ConvertFrom(context, culture, value);
            long num = ((Enum[]) value).Aggregate(0L,
                                                  (current, @enum) =>
                                                  current | Convert.ToInt64(@enum, CultureInfo.InvariantCulture));
            return Enum.ToObject(typeof (Keys), num);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType) {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");
            if (value is Keys || value is int) {
                bool flag1 = destinationType == typeof (string);
                bool flag2 = false;
                if (!flag1)
                    flag2 = destinationType == typeof (Enum[]);
                if (flag1 || flag2) {
                    var keys1 = (Keys) value;
                    bool flag3 = false;
                    var arrayList = new ArrayList();
                    Keys keys2 = keys1 & Keys.Modifiers;
                    foreach (string str in DisplayOrder) {
                        var keys3 = (Keys) keyNames[str];
                        if ((keys3 & keys2) != Keys.None) {
                            if (flag1) {
                                if (flag3)
                                    arrayList.Add("+");
                                arrayList.Add(str);
                            }
                            else
                                arrayList.Add(keys3);
                            flag3 = true;
                        }
                    }
                    Keys keys4 = keys1 & Keys.KeyCode;
                    bool flag4 = false;
                    if (flag3 && flag1)
                        arrayList.Add("+");
                    foreach (string str in DisplayOrder) {
                        var keys3 = (Keys) keyNames[str];
                        if (keys3.Equals(keys4)) {
                            if (flag1)
                                arrayList.Add(str);
                            else
                                arrayList.Add(keys3);
                            flag4 = true;
                            break;
                        }
                    }
                    if (!flag4 && Enum.IsDefined(typeof (Keys), keys4)) {
                        if (flag1)
                            arrayList.Add(((object) keys4).ToString());
                        else
                            arrayList.Add(keys4);
                    }
                    if (!flag1)
                        return arrayList.ToArray(typeof (Enum));
                    var stringBuilder = new StringBuilder(32);
                    foreach (string str in arrayList)
                        stringBuilder.Append(str);
                    return (stringBuilder).ToString();
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (values == null) {
                var arrayList = new ArrayList();
                foreach (object obj in KeyNames.Values)
                    arrayList.Add(obj);
                arrayList.Sort(this);
                values = new StandardValuesCollection(arrayList.ToArray());
            }
            return values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        void AddKey(string key, Keys value) {
            keyNames[key] = value;
            displayOrder.Add(key);
        }

        void Initialize() {
            keyNames = new Hashtable(34);
            displayOrder = new List<string>(34);
            AddKey("Enter", Keys.Return);
            AddKey("F12", Keys.F12);
            AddKey("F11", Keys.F11);
            AddKey("F10", Keys.F10);
            AddKey("End", Keys.End);
            AddKey("Control", Keys.Control);
            AddKey("F8", Keys.F8);
            AddKey("F9", Keys.F9);
            AddKey("Alt", Keys.Alt);
            AddKey("F4", Keys.F4);
            AddKey("F5", Keys.F5);
            AddKey("F6", Keys.F6);
            AddKey("F7", Keys.F7);
            AddKey("F1", Keys.F1);
            AddKey("F2", Keys.F2);
            AddKey("F3", Keys.F3);
            AddKey("PageDown", Keys.Next);
            AddKey("Insert", Keys.Insert);
            AddKey("Home", Keys.Home);
            AddKey("Delete", Keys.Delete);
            AddKey("Shift", Keys.Shift);
            AddKey("PageUp", Keys.Prior);
            AddKey("Back", Keys.Back);
            AddKey("0", Keys.D0);
            AddKey("1", Keys.D1);
            AddKey("2", Keys.D2);
            AddKey("3", Keys.D3);
            AddKey("4", Keys.D4);
            AddKey("5", Keys.D5);
            AddKey("6", Keys.D6);
            AddKey("7", Keys.D7);
            AddKey("8", Keys.D8);
            AddKey("9", Keys.D9);
        }
    }

    [Flags]
    public enum Keys {
        KeyCode = 65535,
        Modifiers = -65536,
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = RButton | LButton,
        MButton = 4,
        XButton1 = MButton | LButton,
        XButton2 = MButton | RButton,
        Back = 8,
        Tab = Back | LButton,
        LineFeed = Back | RButton,
        Clear = Back | MButton,
        Return = Clear | LButton,
        Enter = Return,
        ShiftKey = 16,
        ControlKey = ShiftKey | LButton,
        Menu = ShiftKey | RButton,
        Pause = Menu | LButton,
        Capital = ShiftKey | MButton,
        CapsLock = Capital,
        KanaMode = CapsLock | LButton,
        HanguelMode = KanaMode,
        HangulMode = HanguelMode,
        JunjaMode = HangulMode | RButton,
        FinalMode = ShiftKey | Back,
        HanjaMode = FinalMode | LButton,
        KanjiMode = HanjaMode,
        Escape = KanjiMode | RButton,
        IMEConvert = FinalMode | MButton,
        IMENonconvert = IMEConvert | LButton,
        IMEAccept = IMEConvert | RButton,
        IMEAceept = IMEAccept,
        IMEModeChange = IMEAceept | LButton,
        Space = 32,
        Prior = Space | LButton,
        PageUp = Prior,
        Next = Space | RButton,
        PageDown = Next,
        End = PageDown | LButton,
        Home = Space | MButton,
        Left = Home | LButton,
        Up = Home | RButton,
        Right = Up | LButton,
        Down = Space | Back,
        Select = Down | LButton,
        Print = Down | RButton,
        Execute = Print | LButton,
        Snapshot = Down | MButton,
        PrintScreen = Snapshot,
        Insert = PrintScreen | LButton,
        Delete = PrintScreen | RButton,
        Help = Delete | LButton,
        D0 = Space | ShiftKey,
        D1 = D0 | LButton,
        D2 = D0 | RButton,
        D3 = D2 | LButton,
        D4 = D0 | MButton,
        D5 = D4 | LButton,
        D6 = D4 | RButton,
        D7 = D6 | LButton,
        D8 = D0 | Back,
        D9 = D8 | LButton,
        A = 65,
        B = 66,
        C = B | LButton,
        D = 68,
        E = D | LButton,
        F = D | RButton,
        G = F | LButton,
        H = 72,
        I = H | LButton,
        J = H | RButton,
        K = J | LButton,
        L = H | MButton,
        M = L | LButton,
        N = L | RButton,
        O = N | LButton,
        P = 80,
        Q = P | LButton,
        R = P | RButton,
        S = R | LButton,
        T = P | MButton,
        U = T | LButton,
        V = T | RButton,
        W = V | LButton,
        X = P | Back,
        Y = X | LButton,
        Z = X | RButton,
        LWin = Z | LButton,
        RWin = X | MButton,
        Apps = RWin | LButton,
        Sleep = Apps | RButton,
        NumPad0 = 96,
        NumPad1 = NumPad0 | LButton,
        NumPad2 = NumPad0 | RButton,
        NumPad3 = NumPad2 | LButton,
        NumPad4 = NumPad0 | MButton,
        NumPad5 = NumPad4 | LButton,
        NumPad6 = NumPad4 | RButton,
        NumPad7 = NumPad6 | LButton,
        NumPad8 = NumPad0 | Back,
        NumPad9 = NumPad8 | LButton,
        Multiply = NumPad8 | RButton,
        Add = Multiply | LButton,
        Separator = NumPad8 | MButton,
        Subtract = Separator | LButton,
        Decimal = Separator | RButton,
        Divide = Decimal | LButton,
        F1 = NumPad0 | ShiftKey,
        F2 = F1 | LButton,
        F3 = F1 | RButton,
        F4 = F3 | LButton,
        F5 = F1 | MButton,
        F6 = F5 | LButton,
        F7 = F5 | RButton,
        F8 = F7 | LButton,
        F9 = F1 | Back,
        F10 = F9 | LButton,
        F11 = F9 | RButton,
        F12 = F11 | LButton,
        F13 = F9 | MButton,
        F14 = F13 | LButton,
        F15 = F13 | RButton,
        F16 = F15 | LButton,
        F17 = 128,
        F18 = F17 | LButton,
        F19 = F17 | RButton,
        F20 = F19 | LButton,
        F21 = F17 | MButton,
        F22 = F21 | LButton,
        F23 = F21 | RButton,
        F24 = F23 | LButton,
        NumLock = F17 | ShiftKey,
        Scroll = NumLock | LButton,
        LShiftKey = F17 | Space,
        RShiftKey = LShiftKey | LButton,
        LControlKey = LShiftKey | RButton,
        RControlKey = LControlKey | LButton,
        LMenu = LShiftKey | MButton,
        RMenu = LMenu | LButton,
        BrowserBack = LMenu | RButton,
        BrowserForward = BrowserBack | LButton,
        BrowserRefresh = LShiftKey | Back,
        BrowserStop = BrowserRefresh | LButton,
        BrowserSearch = BrowserRefresh | RButton,
        BrowserFavorites = BrowserSearch | LButton,
        BrowserHome = BrowserRefresh | MButton,
        VolumeMute = BrowserHome | LButton,
        VolumeDown = BrowserHome | RButton,
        VolumeUp = VolumeDown | LButton,
        MediaNextTrack = LShiftKey | ShiftKey,
        MediaPreviousTrack = MediaNextTrack | LButton,
        MediaStop = MediaNextTrack | RButton,
        MediaPlayPause = MediaStop | LButton,
        LaunchMail = MediaNextTrack | MButton,
        SelectMedia = LaunchMail | LButton,
        LaunchApplication1 = LaunchMail | RButton,
        LaunchApplication2 = LaunchApplication1 | LButton,
        OemSemicolon = MediaStop | Back,
        Oem1 = OemSemicolon,
        Oemplus = Oem1 | LButton,
        Oemcomma = LaunchMail | Back,
        OemMinus = Oemcomma | LButton,
        OemPeriod = Oemcomma | RButton,
        OemQuestion = OemPeriod | LButton,
        Oem2 = OemQuestion,
        Oemtilde = 192,
        Oem3 = Oemtilde,
        OemOpenBrackets = Oem3 | Escape,
        Oem4 = OemOpenBrackets,
        OemPipe = Oem3 | IMEConvert,
        Oem5 = OemPipe,
        OemCloseBrackets = Oem5 | LButton,
        Oem6 = OemCloseBrackets,
        OemQuotes = Oem5 | RButton,
        Oem7 = OemQuotes,
        Oem8 = Oem7 | LButton,
        OemBackslash = Oem3 | PageDown,
        Oem102 = OemBackslash,
        ProcessKey = Oem3 | Left,
        Packet = ProcessKey | RButton,
        Attn = Oem102 | CapsLock,
        Crsel = Attn | LButton,
        Exsel = Oem3 | D8,
        EraseEof = Exsel | LButton,
        Play = Exsel | RButton,
        Zoom = Play | LButton,
        NoName = Exsel | MButton,
        Pa1 = NoName | LButton,
        OemClear = NoName | RButton,
        Shift = 65536,
        Control = 131072,
        Alt = 262144,
    }

    public enum Shortcut {
        None = 0,
        Ins = 45,
        Del = 46,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        ShiftIns = 65581,
        ShiftDel = 65582,
        ShiftF1 = 65648,
        ShiftF2 = 65649,
        ShiftF3 = 65650,
        ShiftF4 = 65651,
        ShiftF5 = 65652,
        ShiftF6 = 65653,
        ShiftF7 = 65654,
        ShiftF8 = 65655,
        ShiftF9 = 65656,
        ShiftF10 = 65657,
        ShiftF11 = 65658,
        ShiftF12 = 65659,
        CtrlIns = 131117,
        CtrlDel = 131118,
        Ctrl0 = 131120,
        Ctrl1 = 131121,
        Ctrl2 = 131122,
        Ctrl3 = 131123,
        Ctrl4 = 131124,
        Ctrl5 = 131125,
        Ctrl6 = 131126,
        Ctrl7 = 131127,
        Ctrl8 = 131128,
        Ctrl9 = 131129,
        CtrlA = 131137,
        CtrlB = 131138,
        CtrlC = 131139,
        CtrlD = 131140,
        CtrlE = 131141,
        CtrlF = 131142,
        CtrlG = 131143,
        CtrlH = 131144,
        CtrlI = 131145,
        CtrlJ = 131146,
        CtrlK = 131147,
        CtrlL = 131148,
        CtrlM = 131149,
        CtrlN = 131150,
        CtrlO = 131151,
        CtrlP = 131152,
        CtrlQ = 131153,
        CtrlR = 131154,
        CtrlS = 131155,
        CtrlT = 131156,
        CtrlU = 131157,
        CtrlV = 131158,
        CtrlW = 131159,
        CtrlX = 131160,
        CtrlY = 131161,
        CtrlZ = 131162,
        CtrlF1 = 131184,
        CtrlF2 = 131185,
        CtrlF3 = 131186,
        CtrlF4 = 131187,
        CtrlF5 = 131188,
        CtrlF6 = 131189,
        CtrlF7 = 131190,
        CtrlF8 = 131191,
        CtrlF9 = 131192,
        CtrlF10 = 131193,
        CtrlF11 = 131194,
        CtrlF12 = 131195,
        CtrlShift0 = 196656,
        CtrlShift1 = 196657,
        CtrlShift2 = 196658,
        CtrlShift3 = 196659,
        CtrlShift4 = 196660,
        CtrlShift5 = 196661,
        CtrlShift6 = 196662,
        CtrlShift7 = 196663,
        CtrlShift8 = 196664,
        CtrlShift9 = 196665,
        CtrlShiftA = 196673,
        CtrlShiftB = 196674,
        CtrlShiftC = 196675,
        CtrlShiftD = 196676,
        CtrlShiftE = 196677,
        CtrlShiftF = 196678,
        CtrlShiftG = 196679,
        CtrlShiftH = 196680,
        CtrlShiftI = 196681,
        CtrlShiftJ = 196682,
        CtrlShiftK = 196683,
        CtrlShiftL = 196684,
        CtrlShiftM = 196685,
        CtrlShiftN = 196686,
        CtrlShiftO = 196687,
        CtrlShiftP = 196688,
        CtrlShiftQ = 196689,
        CtrlShiftR = 196690,
        CtrlShiftS = 196691,
        CtrlShiftT = 196692,
        CtrlShiftU = 196693,
        CtrlShiftV = 196694,
        CtrlShiftW = 196695,
        CtrlShiftX = 196696,
        CtrlShiftY = 196697,
        CtrlShiftZ = 196698,
        CtrlShiftF1 = 196720,
        CtrlShiftF2 = 196721,
        CtrlShiftF3 = 196722,
        CtrlShiftF4 = 196723,
        CtrlShiftF5 = 196724,
        CtrlShiftF6 = 196725,
        CtrlShiftF7 = 196726,
        CtrlShiftF8 = 196727,
        CtrlShiftF9 = 196728,
        CtrlShiftF10 = 196729,
        CtrlShiftF11 = 196730,
        CtrlShiftF12 = 196731,
        AltBksp = 262152,
        AltLeftArrow = 262181,
        AltUpArrow = 262182,
        AltRightArrow = 262183,
        AltDownArrow = 262184,
        Alt0 = 262192,
        Alt1 = 262193,
        Alt2 = 262194,
        Alt3 = 262195,
        Alt4 = 262196,
        Alt5 = 262197,
        Alt6 = 262198,
        Alt7 = 262199,
        Alt8 = 262200,
        Alt9 = 262201,
        AltF1 = 262256,
        AltF2 = 262257,
        AltF3 = 262258,
        AltF4 = 262259,
        AltF5 = 262260,
        AltF6 = 262261,
        AltF7 = 262262,
        AltF8 = 262263,
        AltF9 = 262264,
        AltF10 = 262265,
        AltF11 = 262266,
        AltF12 = 262267,
    }
}