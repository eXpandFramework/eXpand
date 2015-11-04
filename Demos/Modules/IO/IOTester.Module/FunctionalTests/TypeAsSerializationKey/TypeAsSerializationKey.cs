using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace IOTester.Module.FunctionalTests.TypeAsSerializationKey{
    [DefaultClassOptions]
    [DefaultProperty("TypeKey")]
    public class TypeAsSerializationKey : BaseObject {
        public TypeAsSerializationKey(Session session)
            : base(session) {
        }
        [ValueConverter(typeof(TypeToStringConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type TypeKey {
            get { return GetPropertyValue<Type>("TypeKey"); }
            set { SetPropertyValue("TypeKey", value); }
        }
    }
}