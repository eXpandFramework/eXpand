<<<<<<< HEAD
using System;
=======
>>>>>>> CodeDomApproachForWorldCreator
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    [RuleCombinationOfPropertiesIsUnique(null,DefaultContexts.Save,"Owner,Name")]
<<<<<<< HEAD
    public abstract class PersistentMemberInfo : PersistentTypeInfo, IPersistentMemberInfo {
=======
    public abstract class PersistentMemberInfo : PersistentTemplatedTypeInfo, IPersistentMemberInfo {
>>>>>>> CodeDomApproachForWorldCreator
        protected PersistentMemberInfo(Session session) : base(session) { }
        IPersistentClassInfo IPersistentMemberInfo.Owner {
            get { return _owner; }
            set { _owner=value as PersistentClassInfo; }
        }
        PersistentClassInfo _owner;

<<<<<<< HEAD
        [Association]
        [VisibleInDetailView(false)]
=======
        [Association("PersistentClassInfo-OwnMembers")]
>>>>>>> CodeDomApproachForWorldCreator
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public PersistentClassInfo Owner
        {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
    }
}