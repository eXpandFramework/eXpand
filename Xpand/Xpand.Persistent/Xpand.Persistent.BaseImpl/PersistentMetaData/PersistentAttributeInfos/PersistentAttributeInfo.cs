using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    public abstract class PersistentAttributeInfo : XpandBaseCustomObject, IPersistentAttributeInfo {
        protected PersistentAttributeInfo(Session session) : base(session) { }


        [Persistent]
        [Size(SizeAttribute.Unlimited)]
        [VisibleInDetailView(false)]
        public string Name => GetType().Name + ": " + ToString();

        public abstract AttributeInfoAttribute Create();

        IPersistentTypeInfo IPersistentAttributeInfo.Owner {
            get => Owner;
            set => Owner = value as PersistentTypeInfo;
        }
        PersistentTypeInfo _owner;

        [Association("TypeAttributes")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public PersistentTypeInfo Owner {
            get => _owner;
            set => SetPropertyValue("Owner", ref _owner, value);
        }
    }
}