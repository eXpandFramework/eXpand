using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("PropertyName")]
    public class PersistentModelDefaultAttribute : PersistentAttributeInfo {
        string _propertyName;
        string _value;

        public PersistentModelDefaultAttribute(Session session) : base(session) {
        }

        public string PropertyName {
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }

        public string Value {
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }

        public override AttributeInfoAttribute Create() {
            return new AttributeInfoAttribute(typeof(ModelDefaultAttribute).GetConstructor(new[] { typeof(string), typeof(string) }),
                                     PropertyName, Value);
        }
    }
}