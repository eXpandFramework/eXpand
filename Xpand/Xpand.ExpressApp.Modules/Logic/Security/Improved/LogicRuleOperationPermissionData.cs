using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Logic.DomainLogic;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using IRule = Xpand.Persistent.Base.Logic.IRule;
using System.Linq;

namespace Xpand.ExpressApp.Logic.Security.Improved {
    [NonPersistent]
    [RuleCriteria("LogicRulePermission", DefaultContexts.Save, "(Not IsNullOrEmpty(ExecutionContextGroup)) OR (Not IsNullOrEmpty(ActionExecutionContextGroup))", SkipNullOrEmptyValues = false, CustomMessageTemplate = "At least one of ExecutionContextGroup, ActionExecutionContextGroup should not be null")]
    [Appearance("ActionContextGroupVisibility", AppearanceItemType.ViewItem, "HasActionContextGroup=false", Visibility = ViewItemVisibility.Hide, TargetItems = "ActionExecutionContextGroup",Context = "DetailView")]
    [Appearance("ExecutionContextGroupVisibility", AppearanceItemType.ViewItem, "HasExecutionContextGroup=false", Visibility = ViewItemVisibility.Hide, TargetItems = "ExecutionContextGroup", Context = "DetailView")]
    [Appearance("FrameTemplateContextGroupVisibility", AppearanceItemType.ViewItem, "HasFrameTemplateContextGroup=false", Visibility = ViewItemVisibility.Hide, TargetItems = "FrameTemplateContextGroup", Context = "DetailView")]
    [Appearance("ViewContextGroupVisibility", AppearanceItemType.ViewItem, "HasViewContextGroup=false", Visibility = ViewItemVisibility.Hide, TargetItems = "ViewContextGroup", Context = "DetailView")]
    public abstract class LogicRuleOperationPermissionData : XpandPermissionData, IContextLogicRule {

        protected LogicRuleOperationPermissionData(Session session)
            : base(session) {

        }
        [Browsable(false)]
        public bool HasFrameTemplateContextGroup {
            get { return LogicInstallerManager.Instance[this].GetModelLogic().FrameTemplateContextsGroup.Any(); }
        }
        [Browsable(false)]
        public bool HasViewContextGroup {
            get { return LogicInstallerManager.Instance[this].GetModelLogic().ViewContextsGroup.Any(); }
        }
        [Browsable(false)]
        public bool HasExecutionContextGroup {
            get { return LogicInstallerManager.Instance[this].GetModelLogic().ExecutionContextsGroup.Any(); }
        }
        [Browsable(false)]
        public bool HasActionContextGroup {
            get { return LogicInstallerManager.Instance[this].GetModelLogic().ActionExecutionContextGroup.Any(); }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ExecutionContextGroup = ContextLogicRuleDomainLogic.DefaultExecutionContextGroup;
        }

        
        private Type _objectTypeData;

        [RuleRequiredField]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (XpandLocalizedClassInfoTypeConverter))]
        [Index(2)]
        public Type ObjectTypeData {
            get { return _objectTypeData; }
            set {
                SetPropertyValue("ObjectTypeData", ref _objectTypeData, value);
                OnChanged("TypeInfo");
            }
        }
        [RuleRequiredField]
        [Index(0)]
        public string ID { get; set; }
        [Index(9)]
        public string ExecutionContextGroup { get; set; }
        [Index(12)]
        public string ActionExecutionContextGroup { get; set; }
        [Index(10)]
        public string ViewContextGroup { get; set; }
        [Index(11)]
        public string FrameTemplateContextGroup { get; set; }
        [CriteriaOptions("ObjectTypeData")]
        [EditorAlias(EditorAliases.ExtendedCriteriaPropertyEditor)]
        [ModelDefault("RowCount","3")]
        public string NormalCriteria { get; set; }
        [CriteriaOptions("ObjectTypeData")]
        [EditorAlias(EditorAliases.ExtendedCriteriaPropertyEditor)]
        [ModelDefault("RowCount", "3")]
        public string EmptyCriteria { get; set; }

        public bool? IsNew { get; set; }

        [Index(7)]
        public FrameTemplateContext FrameTemplateContext { get; set; }
        [Index(8)]
        public bool? IsRootView { get; set; }

        [Index(1)]
        public int? Index { get; set; }
        [Index(4)]
        public ViewType ViewType { get; set; }
        [Index(6)]
        public ViewEditMode? ViewEditMode { get; set; }
        [Index(3)]
        public string View { get; set; }

        IModelView ILogicRule.View { get; set; }
        [Index(5)]
        public Nesting Nesting { get; set; }


        string IRule.Id {
            get { return ID; }
            set { ID = value; }
        }
        ITypeInfo ILogicRule.TypeInfo {
            get { return XafTypesInfo.Instance.FindTypeInfo(ObjectTypeData); }
            set { }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description { get; set; }
    }

}