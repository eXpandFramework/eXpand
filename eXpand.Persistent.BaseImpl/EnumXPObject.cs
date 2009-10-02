using DevExpress.Xpo;
using eXpand.Persistent.Base.General;
using eXpand.Xpo;

namespace eXpand.Persistent.BaseImpl{
    [NonPersistent]
    public abstract class EnumXPObject : eXpandCustomObject, IEnumObject
    {
        protected EnumXPObject(Session session) : base(session){
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetPropertyValue("Name", ref name, value);
            }
        }        
    }
}