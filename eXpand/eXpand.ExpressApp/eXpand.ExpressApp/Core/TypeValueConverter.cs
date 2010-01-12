using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.Core{
    public class TypeValueConverter : ValueConverter {
        public override Type StorageType {
            get { return typeof(string); }
        }
        public override object ConvertFromStorageType(object value){
            if (value != null) return ReflectionHelper.GetType(value.ToString());
            return null;
        }
        
        public override object ConvertToStorageType(object value) {
            if (value != null) return ((Type) value).Name;
            return value;
        }
    }
}