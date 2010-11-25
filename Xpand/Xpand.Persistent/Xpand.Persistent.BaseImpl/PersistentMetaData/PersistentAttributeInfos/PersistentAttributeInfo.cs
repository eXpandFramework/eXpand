using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    public abstract class PersistentAttributeInfo : XpandCustomObject, IPersistentAttributeInfo {
        protected PersistentAttributeInfo(Session session) : base(session) { }


        [Persistent]
        [Size(255)]
        [VisibleInDetailView(false)]
        public string Name {
            get { return GetType().Name + ": " + ToString(); }
        }

        public abstract AttributeInfo Create();

        IPersistentTypeInfo IPersistentAttributeInfo.Owner {
            get { return Owner; }
            set { Owner = value as PersistentTypeInfo; }
        }
        PersistentTypeInfo _owner;

        [Association("TypeAttributes")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public PersistentTypeInfo Owner {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
    }
}