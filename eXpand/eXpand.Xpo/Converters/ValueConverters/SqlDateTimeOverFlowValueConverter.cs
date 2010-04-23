using System;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Converters.ValueConverters {
    public class SqlDateTimeOverFlowValueConverter:ValueConverter {
        public override Type StorageType {
            get { return typeof(DateTime); }
        }

        public override object ConvertToStorageType(object value) {
            if (value!= null) {
                var dateTime = new DateTime(1753, 1, 1);
                if (dateTime>(DateTime) value)
                    return dateTime;
                dateTime = new DateTime(9999, 12, 31);
                if (dateTime<(DateTime) value)
                    return dateTime;
            }
            return value;
        }

        public override object ConvertFromStorageType(object value) {
            return value;
        }
    }
}