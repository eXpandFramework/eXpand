using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("PropertyName")]
    public class PersistentCustomAttribute : PersistentAttributeInfo {
        string _propertyName;
        string _value;

        public PersistentCustomAttribute(Session session) : base(session) {
        }

        public string PropertyName {
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }

        public string Value {
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }

        public override AttributeInfo Create() {
            return new AttributeInfo(typeof (CustomAttribute).GetConstructor(new[] {typeof (string), typeof (string)}),
                                     PropertyName, Value);
        }
    }
}