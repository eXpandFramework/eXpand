using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "Owner,Name")]
    public abstract class ExtendedMemberInfo : PersistentTypeInfo, IExtendedMemberInfo {
        Type _owner;

        PersistentClassInfo _ownerClassInfo;

        protected ExtendedMemberInfo(Session session) : base(session) {
        }
        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [Custom("GroupIndex","0")]
        public string TypeInfoName
        {
            get { return GetType().Name.Replace("Persistent", ""); }
        }
        public PersistentClassInfo OwnerClassInfo {
            get { return _ownerClassInfo; }
            set { SetPropertyValue("OwnerClassInfo", ref _ownerClassInfo, value); }
        }
        #region IExtendedMemberInfo Members
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type Owner {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
        #endregion
    }
}