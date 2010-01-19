using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("ConverterType")]
    public class PersistentValueConverter : PersistentAttributeInfo {
        Type _converterType;

        public PersistentValueConverter(Session session) : base(session) {
        }
        [RuleRequiredField(null,DefaultContexts.Save)]
        public Type ConverterType {
            get { return _converterType; }
            set { SetPropertyValue("ConverterType", ref _converterType, value); }
        }

        public override AttributeInfo Create() {
            ConstructorInfo constructorInfo = typeof (ValueConverterAttribute).GetConstructor(new[] {typeof (Type)});
            return new AttributeInfo(constructorInfo, ConverterType);
        }
    }
}