using System;
using System.Linq;
using TB.ComponentModel;

namespace Xpand.Utils.Helpers {
    [Flags]
    public enum ConcatFlags {
        None = 0,
        IgnoreNull = 1,
        IgnoreEmpty = 2,
    }

    public class StringBuilder : IStringBuilder {
        public const string DefaultStringSeperator = ";";
        public const string DefaultNullStringValue = ".null.";
        private readonly string _seperator;
        private readonly string _nullValue;
        private readonly ConcatFlags _concatFlags;

        public StringBuilder()
            : this(DefaultStringSeperator) {
        }

        public StringBuilder(string seperator)
            : this(seperator, ConcatFlags.None) {
        }

        public StringBuilder(string seperator, ConcatFlags concatFlags)
            : this(seperator, DefaultNullStringValue, concatFlags) {
        }

        public StringBuilder(string seperator, string nullValue)
            : this(seperator, nullValue, ConcatFlags.None) {
        }

        public StringBuilder(string seperator, string nullValue, ConcatFlags concatFlags) {
            if (seperator == null) {
                throw new ArgumentNullException("seperator");
            }
            _seperator = seperator;
            _nullValue = nullValue;
            _concatFlags = concatFlags;
        }

        public string Concatenate(string[] values) {
            var result = new System.Text.StringBuilder();
            foreach (string value in values.Where(value => !IgnoreValue(value)).Select(value => value ?? _nullValue)) {
                if (result.Length > 0) {
                    result.Append(_seperator);
                }
                result.Append(value);
            }
            return result.ToString();
        }

        private bool IgnoreValue(string value) {
            if (value == null &&
                (_concatFlags & ConcatFlags.IgnoreNull) == ConcatFlags.IgnoreNull) {
                return true;
            }
            if (value == String.Empty &&
                (_concatFlags & ConcatFlags.IgnoreEmpty) == ConcatFlags.IgnoreEmpty) {
                return true;
            }
            return false;
        }

    }
}
