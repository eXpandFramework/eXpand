using System;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Converters.ValueConverters{
    public class TypeValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof(string); }
        }
        public override object ConvertFromStorageType(object value){
            if (value != null) return Type.GetType((string) value);
            return null;
        }

        public override object ConvertToStorageType(object value) {
            return ((Type) value).AssemblyQualifiedName;
        }
    }
}