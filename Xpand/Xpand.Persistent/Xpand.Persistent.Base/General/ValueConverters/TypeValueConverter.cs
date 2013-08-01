using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;

namespace Xpand.Persistent.Base.General.ValueConverters {
    public class TypeValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof (string); }
        }

        public override object ConvertFromStorageType(object value) {
            if (value == null)
                return null;

            try {
                return ReflectionHelper.GetType(value.ToString());
            }
            catch (TypeWasNotFoundException) {
            }

            return null;
        }

        public override object ConvertToStorageType(object value) {
            return value != null ? ((Type) value).FullName : null;
        }
    }
}