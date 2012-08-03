using System;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.Converters.ValueConverters {
    public class NullValueConverter : ValueConverter {
        public override object ConvertToStorageType(object value) {
            return null;
        }

        public override object ConvertFromStorageType(object value) {
            return null;
        }

        public override Type StorageType {
            get { return typeof(string); }
        }
    }
}