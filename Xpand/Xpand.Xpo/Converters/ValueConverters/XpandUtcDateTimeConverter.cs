using System;

namespace Xpand.Xpo.Converters.ValueConverters {
    public class XpandUtcDateTimeConverter : DevExpress.Xpo.Metadata.UtcDateTimeConverter {
        public override object ConvertToStorageType(object value) {
            object convertToStorageType = base.ConvertToStorageType(value);
            return new SqlDateTimeOverFlowValueConverter().ConvertToStorageType(convertToStorageType);
        }
        public override object ConvertFromStorageType(object value) {
            object fromStorageType = new SqlDateTimeOverFlowValueConverter().ConvertFromStorageType(value);
            object convertFromStorageType = base.ConvertFromStorageType(fromStorageType);
            return convertFromStorageType != null && ((DateTime)convertFromStorageType).Date == new DateTime(0001, 1, 1) ? null : convertFromStorageType;
        }
    }
}
