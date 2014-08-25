using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xpand.Utils.Helpers {
    [Flags]
    public enum Conversion {
        GuessValues = 0,
        None=1,
        TreatNullAsDefault = 2,
        TreatWhitespaceAsDefault = 4,
    }

    public static class XpandConvert {
        private const string ImplicitOperatorMethodName = "op_Implicit";
        private const string ExplicitOperatorMethodName = "op_Explicit";
        private static readonly CultureInfo _defaultCultureInfo = CultureInfo.CurrentCulture;
        private const Conversion DefaultConversion = Conversion.None;

        public static bool CanChange<T>(object value) {
            T result;            
            return TryToChange(value, out result);
        }

        public static bool CanChange<T>(string value, CultureInfo culture) {
            T result;
            return TryToChange(value, out result, culture);
        }

        public static bool CanChange<T>(object value, Conversion options) {
            T result;
            return TryToChange(value, out result, options);
        }

        public static bool CanChange<T>(object value, CultureInfo culture, Conversion options) {
            T result;
            return TryToChange(value, out result, culture, options);
        }

        public static bool TryToChange<T>(object value, out T result) {
            return TryToChange(value, out result, _defaultCultureInfo);
        }

        public static bool TryToChange<T>(object value, out T result, CultureInfo culture) {
            return TryToChange(value, out result, culture, DefaultConversion);
        }

        public static bool TryToChange<T>(object value, out T result, Conversion options) {
            return TryToChange(value, out result, _defaultCultureInfo, options);
        }

        public static bool TryToChange<T>(object value, out T result, CultureInfo culture, Conversion options) {
            object tmpResult;
            if (TryToChange(value, typeof(T), out tmpResult, options, culture)) {
                result = (T)tmpResult;
                return true;
            }
            result = default(T);
            return false;
        }
        
        public static T Change<T>(object value) {
            return Change<T>(value, _defaultCultureInfo);
        }

        public static T Change<T>(object value, CultureInfo culture) {
            return Change<T>(value, culture, DefaultConversion);
        }

        public static T Change<T>(object value, Conversion options) {
            return Change<T>(value, _defaultCultureInfo, options);
        }

        public static T Change<T>(object value, CultureInfo culture, Conversion options) {
            return (T)Change(value, typeof(T), culture, options);
        }

        public static bool CanChange(this object value, Type destinationType) {
            object result;
            return TryToChange(value, destinationType, out result);
        }

        public static bool CanChange(this object value, Type destinationType, CultureInfo culture) {
            object result;
            return TryToChange(value, destinationType, out result, culture);
        }

        public static bool CanChange(this object value, Type destinationType, Conversion options) {
            object result;
            return TryToChange(value, destinationType, out result, options);
        }

        public static bool CanChange(this object value, Type destinationType, CultureInfo culture, Conversion options) {
            object result;
            return TryToChange(value, destinationType, out result, options, culture);
        }

        public static bool TryToChange(this object value, Type destinationType, out object result) {
            return TryToChange(value, destinationType, out result, _defaultCultureInfo);
        }

        public static bool TryToChange(this object value, Type destinationType, out object result, CultureInfo culture) {
            return TryToChange(value, destinationType, out result, 
                DefaultConversion, culture);
        }

        public static bool TryToChange(this object value, Type destinationType, out object result, Conversion options) {
            return TryToChange(value, destinationType, out result, options, _defaultCultureInfo);
        }

        public static bool TryToChange(this object value, Type destinationType, out object result, Conversion options , CultureInfo culture) {
            if (destinationType == typeof(object)) {
                result = value;
                return true;
            }
            if (value.IsValueNull()) {
                return TryChangeFromNull(destinationType, out result, options);
            }
            if (destinationType.IsInstanceOfType(value)) {
                result = value;
                if (destinationType==typeof(string)&&IsNullString(value.ToString())) {
                    result = null;
                }
                return true;
            }
            Type coreDestinationType = destinationType.IsGeneric() ? destinationType.GetUnderlyingType() : destinationType;
            object tmpResult = null;
            if (TryChangeCore(value, coreDestinationType, ref tmpResult, culture, options)) {
                result = tmpResult;
                return true;
            }
            result = null;
            return false;
        }

        private static bool TryChangeFromNull(Type destinationType, out object result, Conversion options) {
            result = destinationType.GetDefaultValue();
            if (result == null) {
                return true;
            }
            return (options & Conversion.TreatNullAsDefault) ==
                   Conversion.TreatNullAsDefault;
        }

        private static bool TryChangeCore(object value, Type destinationType, ref object result, CultureInfo culture, Conversion options) {
            if (value.GetType() == destinationType) {
                result = value;
                return true;
            }
            
            if (TryChangeByTryParse(value.ToString(), destinationType, ref result)) {
                return true;
            }
            if (TryChangeXPlicit(value, destinationType, ExplicitOperatorMethodName, ref result)) {
                return true;
            }
            if (TryChangeXPlicit(value, destinationType, ImplicitOperatorMethodName, ref result)) {
                return true;
            }
            if (TryChangeByIntermediateConversion(value, destinationType, ref result, culture, options)) {
                return true;
            }
            if (destinationType.IsEnum) {
                if (TryChangeToEnum(value, destinationType, ref result)) {
                    return true;
                }
            }

            if ((options & Conversion.TreatWhitespaceAsDefault) == Conversion.TreatWhitespaceAsDefault) {
                var s = value as string;
                if (s != null && s.IsWhiteSpace()) {
                    result = destinationType.GetDefaultValue();
                    return true;
                }
            }
            if ((options & Conversion.GuessValues) == Conversion.GuessValues) {
                if (TryChangeGuessedValues(value, destinationType, ref result)) {
                    return true;
                }
            }

            return false;
        }

        static bool TryChangeNullString(object value, Type destinationType, ref object result) {
            var asString = value.ToString();
            return IsNullString(asString) && TryToChange(null, destinationType, out result, Conversion.TreatNullAsDefault);
        }

        static bool IsNullString(string asString) {
            return asString != String.Empty && string.CompareOrdinal("(NULL)", asString) == 0;
        }

        private static bool TryChangeGuessedValues(object value, Type destinationType, ref object result) {
            if (value is char && destinationType == typeof(bool)) {
                return TryChangeCharToBool((char)value, ref result);
            }
            var changeNullString = TryChangeNullString(value, destinationType, ref result);
            if (changeNullString)
                return true;
            if ( destinationType == typeof(bool)) {
                return TryChangeStringToBool(value.ToString(), ref result);
            }

            if (value is bool && destinationType == typeof(char)) {
                return ChangeBoolToChar((bool)value, out result);
            }
            if (destinationType ==typeof(TimeSpan)) {
                object o;
                if (TryToChange(value, typeof (long), out o,Conversion.GuessValues)) {
                    result = new TimeSpan((long) o);
                    return true;
                }
            }

            if (destinationType == typeof (Size) && !string.IsNullOrEmpty(value + "")){
                var strings = value.ToString().Split('x');
                if (strings.Length == 2 && (strings[0].CanChange(typeof (int)) && strings[1].CanChange(typeof (int)))){
                    result = new Size(int.Parse(strings[0]), int.Parse(strings[1]));
                    return true;
                }
            }

            return false;
        }

        private static bool TryChangeCharToBool(char value, ref object result) {
            if ("1JYT".Contains(value.ToString(CultureInfo.InvariantCulture).ToUpper())) {
                result = true;
                return true;
            }
            if ("0NF".Contains(value.ToString(CultureInfo.InvariantCulture).ToUpper())) {
                result = false;
                return true;
            }
            return false;
        }

        private static bool TryChangeStringToBool(string value, ref object result) {
            var trueValues = new List<string>(new[] { "1", "yes", "true" });
            if (trueValues.Contains(value.Trim().ToLower())) {
                result = true;
                return true;
            }
            var falseValues = new List<string>(new[] { "0", "no", "false" });
            if (falseValues.Contains(value.Trim().ToLower())) {
                result = false;
                return true;
            }
            return false;
        }

        private static bool ChangeBoolToChar(bool value, out object result) {
            result = value ? 'T' : 'F';
            return true;
        }

        private static bool TryChangeByTryParse(string value, Type destinationType, ref object result) {
            if (destinationType == typeof(Boolean)) {
                return TryParseBool(value, ref result);
            }
            if (destinationType == typeof(Byte)) {
                return TryParseByte(value, ref result);
            }
            if (destinationType == typeof(Char)) {
                return TryParseChar(value, ref result);
            }
            if (destinationType == typeof(DateTime)) {
                return TryParseDateTime(value, ref result);
            }
            if (destinationType == typeof(Decimal)) {
                return TryParseDecimal(value, ref result);
            }
            if (destinationType == typeof(Double)) {
                return TryParseDouble(value, ref result);
            }
            if (destinationType == typeof(Int16)) {
                return TryParseInt16(value, ref result);
            }
            if (destinationType == typeof(Int32)) {
                return TryParseInt32(value, ref result);
            }
            if (destinationType == typeof(Int64)) {
                return TryParseInt64(value, ref result);
            }
            if (destinationType == typeof(SByte)) {
                return TryParseSByte(value, ref result);
            }
            if (destinationType == typeof(Single)) {
                return TryParseSingle(value, ref result);
            }
            if (destinationType == typeof(UInt16)) {
                return TryParseUInt16(value, ref result);
            }
            if (destinationType == typeof(UInt32)) {
                return TryParseUInt32(value, ref result);
            }
            if (destinationType == typeof(UInt64)) {
                return TryParseUInt64(value, ref result);
            }
            if (destinationType == typeof(Guid)) {
                return TryParseUGuid(value, ref result);
            }
            return false;
        }

        static bool TryParseUGuid(string valueString, ref object result) {
            Guid guid;
            var tryParse = Guid.TryParse(valueString, out guid);
            if (tryParse)
                result = guid;
            return tryParse;
        }

        static bool TryParseUInt64(string valueString, ref object result) {
            ushort @ushort;
            var tryParse = UInt16.TryParse(valueString, out @ushort);
            if (tryParse)
                result = @ushort;
            return tryParse;
        }

        static bool TryParseUInt32(string valueString, ref object result) {
            uint u;
            var tryParse = UInt32.TryParse(valueString, out u);
            if (tryParse)
                result = u;
            return tryParse;
        }

        static bool TryParseUInt16(string valueString, ref object result) {
            ushort @ushort;
            var tryParse = UInt16.TryParse(valueString, out @ushort);
            if (tryParse)
                result = @ushort;
            return tryParse;
        }

        static bool TryParseSingle(string valueString, ref object result) {
            float f;
            var tryParse = Single.TryParse(valueString, out f);
            if (tryParse)
                result = f;
            return tryParse;
        }

        static bool TryParseSByte(string valueString, ref object result) {
            sbyte @sbyte;
            var tryParse = SByte.TryParse(valueString, out @sbyte);
            if (tryParse)
                result = @sbyte;
            return tryParse;
        }

        static bool TryParseInt64(string valueString, ref object result) {
            long l;
            var tryParse = Int64.TryParse(valueString, out l);
            if (tryParse)
                result = l;
            return tryParse;
        }

        static bool TryParseInt32(string valueString, ref object result) {
            int i;
            var tryParse = Int32.TryParse(valueString, out i);
            if (tryParse)
                result = i;
            return tryParse;
        }

        static bool TryParseInt16(string valueString, ref object result) {
            short s;
            var tryParse = Int16.TryParse(valueString, out s);
            if (tryParse)
                result = s;
            return tryParse;
        }

        static bool TryParseDouble(string valueString, ref object result) {
            double d;
            var tryParse = double.TryParse(valueString, out d);
            if (tryParse)
                result = d;
            return tryParse;
        }

        static bool TryParseDecimal(string value, ref object result) {
            decimal @decimal;
            var tryParse = decimal.TryParse(value, out @decimal);
            if (tryParse)
                result = @decimal;
            return tryParse;
        }

        static bool TryParseDateTime(string value, ref object result) {
            DateTime time;
            var tryParse = DateTime.TryParse(value, out time);
            if (tryParse)
                result = time;
            return tryParse;
        }

        static bool TryParseChar(string value, ref object result) {
            char c;
            var tryParse = char.TryParse(value, out c);
            if (tryParse)
                result = c;
            return tryParse;
        }

        static bool TryParseByte(string value, ref object result) {
            byte b;
            var tryParse = byte.TryParse(value, out b);
            if (tryParse)
                result = b;
            return tryParse;
        }

        static bool TryParseBool(string value, ref object result) {
            
            bool b;
            var tryParse = bool.TryParse(value, out b);
            if (tryParse)
                result = b;
            return tryParse;
        }

        private static bool TryChangeXPlicit(object value, Type destinationType, string operatorMethodName, ref object result) {
            if (TryChangeXPlicit(value, value.GetType(), destinationType, operatorMethodName, ref result)) {
                return true;
            }
            if (TryChangeXPlicit(value, destinationType, destinationType, operatorMethodName, ref result)) {
                return true;
            }
            return false;
        }

        private static bool TryChangeXPlicit(object value, Type invokerType, Type destinationType, string xPlicitMethodName, ref object result) {
            var methods = invokerType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods.Where(m => m.Name == xPlicitMethodName)) {
                if (destinationType.IsAssignableFrom(method.ReturnType)) {
                    var parameters = method.GetParameters();
                    if (parameters.Count() == 1 && parameters[0].ParameterType == value.GetType()) {
                        try {
                            result = method.Invoke(null, new[] { value });
                            return true;
                        } catch {
                        }
                    }
                }
            }
            return false;
        }

        private static bool TryChangeByIntermediateConversion(object value, Type destinationType, ref object result, CultureInfo culture, Conversion options) {
            if (value is char
                && (destinationType == typeof(double) || destinationType == typeof(float))) {
                return TryChangeCore(Convert.ToInt16(value), destinationType, ref result, culture, options);
            }
            if ((value is double || value is float) && destinationType == typeof(char)) {
                return TryChangeCore(Convert.ToInt16(value), destinationType, ref result, culture, options);
            }
            return false;
        }

        private static bool TryChangeToEnum(object value, Type destinationType, ref object result) {
            object o;
            var enumTryParse = destinationType.EnumTryParse(value.ToString(), out o);
            if (enumTryParse)
                result = o;
            return enumTryParse;
        }

        public static object Change(this object value, Type destinationType) {
            return Change(value, destinationType, _defaultCultureInfo);
        }

        public static object Change(this object value, Type destinationType, CultureInfo culture) {
            return Change(value, destinationType, culture, DefaultConversion);
        }

        public static object Change(this object value, Type destinationType, Conversion options) {
            return Change(value, destinationType, _defaultCultureInfo, options);
        }

        public static object Change(this object value, Type destinationType, CultureInfo culture, Conversion options) {
            object result;
            if (TryToChange(value, destinationType, out result, options, culture)) {
                return result;
            }
            throw new TypeCannotChanged(value, destinationType);
        }
    }
    /// <summary>
    /// The exception that is thrown when a conversion is invalid.
    /// </summary>
    public class TypeCannotChanged : InvalidOperationException {

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCannotChanged">TypeCannotChanged</see> class.
        /// </summary>
        /// <param name="valueToConvert"></param>
        /// <param name="destinationType"></param>
        public TypeCannotChanged(object valueToConvert, Type destinationType)
            : base(String.Format("'{0}' ({1}) is not convertible to '{2}'.",
                                 valueToConvert,
                                 valueToConvert == null ? null : valueToConvert.GetType(),
                                 destinationType)) {
        }
    }

}

