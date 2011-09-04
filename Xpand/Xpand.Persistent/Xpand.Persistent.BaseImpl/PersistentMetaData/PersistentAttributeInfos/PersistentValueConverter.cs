using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
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

        public override AttributeInfoAttribute Create() {
            ConstructorInfo constructorInfo = typeof (ValueConverterAttribute).GetConstructor(new[] {typeof (Type)});
            return new AttributeInfoAttribute(constructorInfo, ConverterType);
        }
    }
}