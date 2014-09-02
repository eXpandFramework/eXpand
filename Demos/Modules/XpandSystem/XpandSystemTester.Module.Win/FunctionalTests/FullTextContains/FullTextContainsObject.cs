using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Model;
using BaseObject = DevExpress.Persistent.BaseImpl.BaseObject;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace XpandSystemTester.Module.Win.FunctionalTests.FullTextContains {
    [DefaultClassOptions]
    [DefaultProperty("FullText")]
    [CloneView(CloneViewType.ListView, FullTextContains.PopupCriteriaPropertyEditorEx+"_ListView")]
    public class FullTextContainsObject:BaseObject {
        private string _criteria;
        private string _objectTypeName;

        public FullTextContainsObject(Session session) : base(session){
        }

        public string FullText { get; set; }

        [CriteriaOptions("DataType")]
        [EditorAlias(EditorAliases.CriteriaPropertyEditorEx)]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [VisibleInListView(false)]
        [ModelDefault("RowCount", "0")]
        public string Criteria {
            get { return _criteria; }
            set { SetPropertyValue("Criteria", ref _criteria, value); }
        }
        [Browsable(false)]
        public string ObjectTypeName {
            get { return _objectTypeName; }
            set { SetPropertyValue("ObjectTypeName", ref _objectTypeName, value); }
        }

        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [ImmediatePostData, NonPersistent]
        [VisibleInListView(false)]
        public Type DataType {
            get {
                if (_objectTypeName != null) {
                    return ReflectionHelper.GetType(_objectTypeName);
                }
                return null;
            }
            set {
                string stringValue = value == null ? null : value.FullName;
                string savedObjectTypeName = ObjectTypeName;
                try {
                    if (stringValue != _objectTypeName) {
                        ObjectTypeName = stringValue;
                    }
                }
                catch (Exception) {
                    ObjectTypeName = savedObjectTypeName;
                }
            }
        }

    }
}
