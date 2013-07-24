using System;
using System.Globalization;

namespace Xpand.Utils.Helpers {
    public static class EnumExtensions {
        /// <summary>
        /// Converts the string representation of an enum to its Enum equivalent value. A return value indicates whether the operation succeeded.
        /// This method does not rely on Enum.Parse and therefore will never raise any first or second chance exception.
        /// </summary>
        /// <param name="type">The enum target type. May not be null.</param>
        /// <param name="input">The input text. May be null.</param>
        /// <param name="value">When this method returns, contains Enum equivalent value to the enum contained in input, if the conversion succeeded.</param>
        /// <returns>
        /// true if s was converted successfully; otherwise, false.
        /// </returns>
        public static bool EnumTryParse(this Type type, string input, out object value) {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsEnum)
                throw new ArgumentException(null, "type");

            if (input == null) {
                value = Activator.CreateInstance(type);
                return false;
            }

            input = input.Trim();
            if (input.Length == 0) {
                value = Activator.CreateInstance(type);
                return false;
            }

            string[] names = Enum.GetNames(type);
            if (names.Length == 0) {
                value = Activator.CreateInstance(type);
                return false;
            }

            Type underlyingType = Enum.GetUnderlyingType(type);
            Array values = Enum.GetValues(type);
            // some enums like System.CodeDom.MemberAttributes *are* flags but are not declared with Flags...
            if ((!type.IsDefined(typeof(FlagsAttribute), true)) && (input.IndexOfAny(_enumSeperators) < 0))
                return EnumToObject(type, underlyingType, names, values, input, out value);

            // multi value enum
            string[] tokens = input.Split(_enumSeperators, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) {
                value = Activator.CreateInstance(type);
                return false;
            }

            ulong ul = 0;
            foreach (string tok in tokens) {
                string token = tok.Trim(); // NOTE: we don't consider empty tokens as errors
                if (token.Length == 0)
                    continue;

                object tokenValue;
                if (!EnumToObject(type, underlyingType, names, values, token, out tokenValue)) {
                    value = Activator.CreateInstance(type);
                    return false;
                }

                ulong tokenUl;
                switch (Convert.GetTypeCode(tokenValue)) {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                        tokenUl = (ulong)Convert.ToInt64(tokenValue, CultureInfo.InvariantCulture);
                        break;

                    //case TypeCode.Byte:
                    //case TypeCode.UInt16:
                    //case TypeCode.UInt32:
                    //case TypeCode.UInt64:
                    default:
                        tokenUl = Convert.ToUInt64(tokenValue, CultureInfo.InvariantCulture);
                        break;
                }

                ul |= tokenUl;
            }
            value = Enum.ToObject(type, ul);
            return true;
        }

        private static readonly char[] _enumSeperators = new[] { ',', ';', '+', '|', ' ' };

        private static object EnumToObject(Type underlyingType, string input) {
            if (underlyingType == typeof(int)) {
                int s;
                if (int.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(uint)) {
                uint s;
                if (uint.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(ulong)) {
                ulong s;
                if (ulong.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(long)) {
                long s;
                if (long.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(short)) {
                short s;
                if (short.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(ushort)) {
                ushort s;
                if (ushort.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(byte)) {
                byte s;
                if (byte.TryParse(input, out s))
                    return s;
            }

            if (underlyingType == typeof(sbyte)) {
                sbyte s;
                if (sbyte.TryParse(input, out s))
                    return s;
            }

            return null;
        }

        private static bool EnumToObject(Type type, Type underlyingType, string[] names, Array values, string input, out object value) {
            for (int i = 0; i < names.Length; i++) {
                if (string.Compare(names[i], input, StringComparison.OrdinalIgnoreCase) == 0) {
                    value = values.GetValue(i);
                    return true;
                }
            }

            if ((char.IsDigit(input[0]) || (input[0] == '-')) || (input[0] == '+')) {
                object obj = EnumToObject(underlyingType, input);
                if (obj == null) {
                    value = Activator.CreateInstance(type);
                    return false;
                }
                value = obj;
                return true;
            }

            value = Activator.CreateInstance(type);
            return false;
        }
    }
}
