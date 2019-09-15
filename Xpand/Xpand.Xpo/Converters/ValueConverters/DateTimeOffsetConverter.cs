using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters{
    public class DateTimeOffsetConverter : ValueConverter {
        public override object ConvertFromStorageType(object value) {
            return value;
        }
 
        public override object ConvertToStorageType(object value) {
            return value is DateTimeOffset dto ? dto.ToString() : value;
        }
 
        public override Type StorageType => typeof(string);
    }
}