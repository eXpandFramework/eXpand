using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    public abstract class PersistentAssemblyAttributeInfo : XpandCustomObject, IPersistentAssemblyAttributeInfo {
        PersistentAssemblyInfo _owner;

        protected PersistentAssemblyAttributeInfo(Session session)
            : base(session) {
        }

        [Persistent]
        [Size(255)]
        [VisibleInDetailView(false)]
        public string Name {
            get { return GetType().Name + ": " + ToString(); }
        }
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Association("PersistentAssemblyInfo-Attributes")]
        public PersistentAssemblyInfo Owner {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
        #region IPersistentAssemblyAttributeInfo Members
        public abstract AttributeInfo Create();

        IPersistentAssemblyInfo IPersistentAssemblyAttributeInfo.Owner {
            get { return Owner; }
            set { Owner = value as PersistentAssemblyInfo; }
        }
        #endregion
    }
}