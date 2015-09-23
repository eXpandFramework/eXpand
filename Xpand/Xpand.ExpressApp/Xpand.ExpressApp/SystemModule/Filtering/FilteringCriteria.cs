using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.SystemModule.Filtering{
    public interface IFilteringCriteria{
        string Name { get; set; }
        string ObjectTypeName { get; set; }
        Type ObjectType { get; set; }
        string Criteria { get; set; }
    }

    [ImageName("Action_Filter")]
    public class FilteringCriteria : XpandCustomObject, IFilteringCriteria{
        [Persistent("Criteria")] private string _criteria;

        private string _name;
        private Type _objectType;
        private string _objectTypeName;

        public FilteringCriteria(Session session)
            : base(session){
        }

        [CriteriaOptions("ObjectType")]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        [ModelDefault("RowCount", "0")]
        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        [PersistentAlias("_criteria")]
        [XafDisplayName("Criteria")]
        public string CriteriaPopup{
            get { return _criteria; }
            set { SetPropertyValue("Criteria", ref _criteria, value); }
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Browsable(false)]
        public string ObjectTypeName{
            get { return _objectTypeName; }
            set{
                var type = XafTypesInfo.Instance.FindTypeInfo(value) == null
                    ? null
                    : XafTypesInfo.Instance.FindTypeInfo(value).Type;
                _objectType = type;
                if (!IsLoading && value != _objectTypeName){
                    Criteria = string.Empty;
                }
                SetPropertyValue("ObjectTypeName", ref _objectTypeName, value);
            }
        }

        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        [ImmediatePostData, NonPersistent]
        public Type ObjectType{
            get { return _objectType; }
            set{
                if (_objectType != value){
                    _objectType = value;
                    ObjectTypeName = (value == null) ? null : value.FullName;
                }
            }
        }

        [CriteriaOptions("ObjectType")]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        [ModelDefault("RowCount", "0")]
        [VisibleInListView(false)]
        [VisibleInDetailView(true)]
        [EditorAlias(EditorAliases.CriteriaPropertyEditor)]
        [PersistentAlias("_criteria")]
        public string Criteria{
            get { return _criteria; }
            set { SetPropertyValue("Criteria", ref _criteria, value); }
        }
    }
}