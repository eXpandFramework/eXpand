using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Xpo;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.ExpressApp.SystemModule.Filtering {
    public interface IFilteringCriteria {
        string Name { get; set; }
        string ObjectTypeName {get; set; }
        Type ObjectType { get; set; }
        string Criteria { get; set; }
    }

    [ImageName("Action_Filter")]
    public class FilteringCriteria : XpandCustomObject, IFilteringCriteria {
        private String _name;
        private String _objectTypeName;
        private Type _objectType;
        [Persistent("Criteria")]
        private String _criteria;

        public FilteringCriteria(Session session)
            : base(session){
        }

        public String Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Browsable(false)]
        public string ObjectTypeName{
            get { return _objectTypeName; }
            set {
                Type type = XafTypesInfo.Instance.FindTypeInfo(value) == null ? null : XafTypesInfo.Instance.FindTypeInfo(value).Type;
                if (_objectType != type){
                    _objectType = type;
                }
                if (!IsLoading && value != _objectTypeName){
                    Criteria = String.Empty;
                }
                SetPropertyValue<string>("ObjectTypeName", ref _objectTypeName, value);
            }
        }

        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [ImmediatePostData, NonPersistent]
        public Type ObjectType
        {
            get { return _objectType; }
            set {
                if (_objectType != value) {
                    _objectType = value;
                    ObjectTypeName = (value == null) ? null : value.FullName;
                }
            }
        }

        [CriteriaOptions("ObjectType")]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [ModelDefault("RowCount", "0")]
        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        [PersistentAlias("_criteria")]
        [XafDisplayName("Criteria")]
        public string CriteriaPopup
        {
            get { return _criteria; }
            set { SetPropertyValue<string>("Criteria", ref _criteria, value); }
        }

        [CriteriaOptions("ObjectType")]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [ModelDefault("RowCount", "0")]
        [VisibleInListView(false)]
        [VisibleInDetailView(true)]
        [EditorAlias(EditorAliases.CriteriaPropertyEditor)]
        [PersistentAlias("_criteria")]
        public string Criteria
        {
            get { return _criteria; }
            set { SetPropertyValue<string>("Criteria", ref _criteria, value); }
        }

    }

}
