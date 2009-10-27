using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    
    public class PersistentCustomAttribute : PersistentAttributeInfo
    {
        public PersistentCustomAttribute(Session session) : base(session) {
        }

        private string  _value;

        public PersistentCustomAttribute() {
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
            return new AttributeInfo(typeof(CustomAttribute).GetConstructor(new[] { typeof(string), typeof(string) }), Name,Value);
        }
    }
}