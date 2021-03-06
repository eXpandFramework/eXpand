using System;
using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "OwnerType,Name")]
    [Appearance("Hide_ExtendedMemberInfo_TemplateInfos", AppearanceItemType.ViewItem, null, TargetItems = "TemplateInfos", Visibility = ViewItemVisibility.Hide)]
    public abstract class ExtendedMemberInfo : PersistentTypeInfo, IExtendedMemberInfo {

        Type _owner;

        PersistentClassInfo _ownerClassInfo;

        protected ExtendedMemberInfo(Session session)
            : base(session) {
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [ModelDefault("GroupIndex", "0")]
        public string TypeInfoName => GetType().Name.Replace("Persistent", "");

        [Browsable(false)]
        public PersistentClassInfo OwnerClassInfo {
            get => _ownerClassInfo;
            set => SetPropertyValue("OwnerClassInfo", ref _ownerClassInfo, value);
        }

        #region IExtendedMemberInfo Members
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [TypeConverter(typeof(XpandLocalizedClassInfoTypeConverter))]
        public Type Owner {
            get => _owner;
            set => SetPropertyValue("Owner", ref _owner, value);
        }

        [PersistentAlias("Concat(_owner,'')")]
        public string OwnerType => (string)EvaluateAlias();

        #endregion
    }
}