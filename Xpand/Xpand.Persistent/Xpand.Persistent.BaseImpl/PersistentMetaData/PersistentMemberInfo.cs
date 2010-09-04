using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "Owner,Name")]
    public abstract class PersistentMemberInfo : PersistentTemplatedTypeInfo, IPersistentMemberInfo {
        PersistentClassInfo _owner;


        protected PersistentMemberInfo(Session session) : base(session) {
        }
        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [Custom("GroupIndex", "0")]
        public string TypeInfoName
        {
            get { return GetType().Name.Replace("Persistent", ""); }
        }
        [VisibleInListView(false)]
        [Custom("AllowEdit", "false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode {
            get { return CodeEngine.GenerateCode(this); }
        }

        [Association("PersistentClassInfo-OwnMembers")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public PersistentClassInfo Owner {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
        #region IPersistentMemberInfo Members
        IPersistentClassInfo IPersistentMemberInfo.Owner {
            get { return _owner; }
            set { _owner = value as PersistentClassInfo; }
        }
        #endregion
    }
}