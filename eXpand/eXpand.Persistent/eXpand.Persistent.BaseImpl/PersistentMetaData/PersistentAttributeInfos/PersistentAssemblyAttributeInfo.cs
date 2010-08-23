using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    public abstract class PersistentAssemblyAttributeInfo : BaseObject, IPersistentAssemblyAttributeInfo {
        PersistentAssemblyInfo _owner;

        protected PersistentAssemblyAttributeInfo(Session session) : base(session) {
        }

        [Persistent]
        [Size(255)]
        [VisibleInDetailView(false)]
        public string Name {
            get { return GetType().Name + ": " + ToString(); }
        }

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