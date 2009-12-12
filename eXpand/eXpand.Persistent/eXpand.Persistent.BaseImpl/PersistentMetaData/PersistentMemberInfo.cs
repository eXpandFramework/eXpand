using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    [RuleCombinationOfPropertiesIsUnique(null,DefaultContexts.Save,"Owner,Name")]
    public abstract class PersistentMemberInfo : PersistentTemplatedTypeInfo, IPersistentMemberInfo {
        protected PersistentMemberInfo(Session session) : base(session) { }
        IPersistentClassInfo IPersistentMemberInfo.Owner {
            get { return _owner; }
            set { _owner=value as PersistentClassInfo; }
        }
        PersistentClassInfo _owner;
        [VisibleInListView(false)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode
        {
            get { return CodeEngine.GenerateCode(this); }
        }

        [Association("PersistentClassInfo-OwnMembers")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public PersistentClassInfo Owner
        {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
    }
}