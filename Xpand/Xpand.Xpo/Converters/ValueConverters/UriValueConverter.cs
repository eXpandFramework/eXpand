using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters{
    public class UriValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof(string); }
        }
        public override object ConvertFromStorageType(object value) {
            return value == null ? null : new Uri(value.ToString());
        }
        
        public override object ConvertToStorageType(object value) {
            return value == null ? null : value.ToString();
        }
    }
}