using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using IRule = Xpand.Persistent.Base.Logic.IRule;

namespace Xpand.ExpressApp.Logic.Security.Improved {
    [NonPersistent]
    public abstract class LogicRuleOperationPermissionData : XpandPermissionData, ILogicRule {

        protected LogicRuleOperationPermissionData(Session session)
            : base(session) {

        }
        public string ViewId { get; set; }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ExecutionContextGroup = LogicDefaultGroupContextNodeUpdater<IModelLogic, IModelNode>.Default;
        }


        private Type _objectTypeData;
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(XpandLocalizedClassInfoTypeConverter))]
        public Type ObjectTypeData {
            get {
                return _objectTypeData;
            }
            set {
                SetPropertyValue("ObjectTypeData", ref _objectTypeData, value);
            }
        }
        [RuleRequiredField]
        public string ID { get; set; }

        public string ExecutionContextGroup { get; set; }
        public string ActionExecutionContextGroup { get; set; }
        public string ViewContextGroup { get; set; }
        public string FrameTemplateContextGroup { get; set; }
        public FrameTemplateContext FrameTemplateContext { get; set; }

        public bool? IsRootView { get; set; }


        public int? Index { get; set; }

        public ViewType ViewType { get; set; }
        public ViewEditMode? ViewEditMode { get; set; }

        IModelView ILogicRule.View { get; set; }

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