using System;
using System.Reflection;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos
{
    public class PersistentValueConverter : PersistentAttributeInfo
    {
        public PersistentValueConverter(Session session) : base(session) {
        }
        private Type _converterType;
        public Type ConverterType
        {
            get
            {
                return _converterType;
            }
            set
            {
                SetPropertyValue("ConverterType", ref _converterType, value);
            }
        }
        public override AttributeInfo Create() {
            ConstructorInfo constructorInfo = typeof(ValueConverterAttribute).GetConstructor(new[] { typeof(Type) });
            return new AttributeInfo(constructorInfo,ConverterType);
        }
    }
}
