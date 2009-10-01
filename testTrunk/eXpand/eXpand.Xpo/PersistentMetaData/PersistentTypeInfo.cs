using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;

namespace eXpand.Xpo.PersistentMetaData
{
    public abstract class PersistentTypeInfo : XPObject {
        protected PersistentTypeInfo(Session session) : base(session) { }

        internal PersistentTypeInfo()
        {
        }
        
        public new XPClassInfo ClassInfo
        {
            get { return Session.Dictionary.GetClassInfo("", Name); }
        }
        
        string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }

        [Association]
        public XPCollection<PersistentAttributeInfo> TypeAttributes { get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); } }

        protected void CreateAttributes(XPTypeInfo ti) {
            foreach(PersistentAttributeInfo a in TypeAttributes) {
                ti.AddAttribute(a.Create());
            }
        }
    }
}