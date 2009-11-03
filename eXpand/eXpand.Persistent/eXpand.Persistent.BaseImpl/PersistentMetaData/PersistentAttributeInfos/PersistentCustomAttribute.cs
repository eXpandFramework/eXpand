using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("PropertyName")]
    public class PersistentCustomAttribute : PersistentAttributeInfo
    {
        public PersistentCustomAttribute(Session session) : base(session) {
        }

        private string  _value;

        private string _propertyName;
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                SetPropertyValue("PropertyName", ref _propertyName, value);
            }
        }
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetPropertyValue("Value", ref _value, value);
            }
        }
        public override AttributeInfo Create() {
            return new AttributeInfo(typeof(CustomAttribute).GetConstructor(new[] { typeof(string), typeof(string) }), PropertyName,Value);
        }
    }
}