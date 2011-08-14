using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters {
    public class SqlDateTimeOverFlowValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof(DateTime); }
        }

        public override object ConvertToStorageType(object value) {
            if (value != null) {
                var dateTime = new DateTime(1753, 1, 1);
                if (dateTime > (DateTime)value) {
                    var time = ((DateTime)value).TimeOfDay;
                    DateTime storageType = dateTime.AddTicks(time.Ticks);
                    return storageType;
                }
                dateTime = new DateTime(9999, 12, 31);
                if (dateTime < (DateTime)value)
                    return dateTime;
            }
            return value;
        }

        public override object ConvertFromStorageType(object value) {
            return value != null && (DateTime)value == new DateTime(1753, 1, 1) ? DateTime.MinValue : value;
        }
    }
    public class SqlDateTimeOffSetOverFlowValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof(DateTime); }
        }

        public override object ConvertToStorageType(object value) {
            if (value != null) {
                var dateTime = new DateTimeOffset(new DateTime(1753, 1, 1));
                if (dateTime > (DateTimeOffset)value) {
                    var time = ((DateTimeOffset)value).TimeOfDay;
                    DateTimeOffset storageType = dateTime.AddTicks(time.Ticks);
                    return storageType.DateTime;
                }
                dateTime = new DateTimeOffset(new DateTime(9999, 12, 31));
                return dateTime < (DateTimeOffset)value ? dateTime.DateTime : ((DateTimeOffset)value).DateTime;
            }
            return null;
        }

        public override object ConvertFromStorageType(object value) {
            return value != null ? (object)new DateTimeOffset((DateTime)value) : null;
        }
    }
}