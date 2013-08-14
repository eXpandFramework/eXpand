using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xpand.Utils.Helpers {
    public static class EnumExtensions {
        public static bool IsPowerOfTwo(this int value) {
            return (value & (value - 1)) == 0;
        }

        public static IEnumerable<T> ToUniqueFlagEnumValues<T>(this IEnumerable<T> flagsEnumValues) where T : struct {
            foreach (T item in flagsEnumValues) {
                int intValue = Convert.ToInt32(item);
                //if our int is a power of two, its a unique value of the flags enum
                if (intValue.IsPowerOfTwo()) {
                    yield return item;
                }
                    //otherwise its a combination of several unique values and we need to break it down further
                else {
                    //the enum value output as binary string representation
                    string fullBinaryString = Convert.ToString(intValue, 2);
                    //an empty template with all 0's that is the length of our binary string
                    char[] individualBitTemplate = new string('0', fullBinaryString.Length).ToCharArray();

                    IEnumerable<T> individualFlagsEnumValues = fullBinaryString
                        .Select((character, index) => {
                                //project each individual bit into its own binary string with 0's in every position
                                //other than the index of the individual bit
                                //Example: binary string 1111
                                //produces 4 individual binary strings
                                //0001
                                //0010
                                //0100
                                //1000
                                var template = (char[])individualBitTemplate.Clone();
                                template[index] = character;
                                return new string(template);
                            })
                        .Where(individualBitBinaryString => individualBitBinaryString.Any(character => character != '0'))
                        .Select(individualBitBinaryString => {
                                //cast the individual binary strings back to their int value, and then into the enum value
                                int intValueOfIndividualBit = Convert.ToInt32(individualBitBinaryString, 2);
                                return (T)Enum.ToObject(typeof(T), intValueOfIndividualBit);
                            });

                    foreach (T value in individualFlagsEnumValues) {
                        yield return value;
                    }
                }
            }
        }
        public static string ToCommaSeperatedList<T>(this IEnumerable<T> enumerable) {
            var stringBuilder = new StringBuilder();
            foreach (T item in enumerable) {
                stringBuilder.Append(item).Append(',');
            }
            //trim the last , off
            if (stringBuilder.Length > 0) {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();
        }
        public static IEnumerable<T> GetIndividualValues<T>(this Enum myEnum) where T : struct {
            return myEnum
                .ToString()
                .Split(new[] { ',' })
                .Select(x => (T)Enum.Parse(typeof(T), x.Trim()))
                .ToUniqueFlagEnumValues();
        }
        public static FlagsEnumDifference<T> GetDifference<T>(this Enum source, Enum compare) where T : struct {
            IEnumerable<T> sourceValues = source.GetIndividualValues<T>();
            IEnumerable<T> compareValues = compare.GetIndividualValues<T>();

            IEnumerable<T> added = compareValues.Where(value => !sourceValues.Contains(value));
            IEnumerable<T> removed = sourceValues.Where(value => !compareValues.Contains(value));
            return new FlagsEnumDifference<T>(added, removed);
        }
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
    public class FlagsEnumDifference<T> {
        public FlagsEnumDifference(IEnumerable<T> added, IEnumerable<T> removed) {
            Added = new List<T>(added);
            Removed = new List<T>(removed);
        }

        public List<T> Added { get; private set; }

        public List<T> Removed { get; private set; }

        public override string ToString() {
            return string.Format("Added: {0} - Removed: {1}", Added.ToCommaSeperatedList(), Removed.ToCommaSeperatedList());
        }
    }
}
