using System;
using DevExpress.Xpo;

namespace eXpand.Xpo.PersistentMetaData
{
    public abstract class PersistentAttributeInfo : XPObject {
        protected PersistentAttributeInfo(Session session) : base(session) { }

        protected PersistentAttributeInfo()
        {
        }

        public abstract Attribute Create();
        
        PersistentTypeInfo _Owner;
        [Association]
        public PersistentTypeInfo Owner {
            get { return _Owner; }
            set { SetPropertyValue("Owner", ref _Owner, value); }
        }
    }
}