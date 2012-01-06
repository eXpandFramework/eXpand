using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Logic.Security.Improved {
    public abstract class LogicRulePermissionRequest : OperationPermissionRequestBase, ILogicRule {
        protected LogicRulePermissionRequest(string operation,ILogicRule logicRule) : base(operation) {
            Description = logicRule.Description;
            ExecutionContextGroup = logicRule.ExecutionContextGroup;
            FrameTemplateContext = logicRule.FrameTemplateContext;
            FrameTemplateContextGroup = logicRule.FrameTemplateContextGroup;
            ID = logicRule.Id;
            Index = logicRule.Index;
            IsRootView = logicRule.IsRootView;
            Nesting = logicRule.Nesting;
            ((ILogicRule)this).View = logicRule.View;
            ViewContextGroup = logicRule.ViewContextGroup;
            ViewEditMode = logicRule.ViewEditMode;
            ViewType = logicRule.ViewType;

        }
        public string ViewId { get; set; }


        [RuleRequiredField]
        public Type ObjectType { get; set; }
        [RuleRequiredField]
        public string ID { get; set; }

        public string ExecutionContextGroup { get; set; }
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
            get { return XafTypesInfo.Instance.FindTypeInfo(ObjectType); }
            set { }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description { get; set; }
    }
}