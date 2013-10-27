using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using IRule = Xpand.Persistent.Base.Logic.IRule;

namespace Xpand.ExpressApp.Logic.Security.Improved {
    public abstract class LogicRulePermission : OperationPermissionBase, IContextLogicRule {
        protected LogicRulePermission(string operation, LogicRuleOperationPermissionData contextLogicRule)
            : base(operation) {
            ExecutionContextGroup = contextLogicRule.ExecutionContextGroup;
            FrameTemplateContextGroup = contextLogicRule.FrameTemplateContextGroup;
            ViewContextGroup = contextLogicRule.ViewContextGroup;
            ActionExecutionContextGroup=contextLogicRule.ActionExecutionContextGroup;
            View = CaptionHelper.ApplicationModel.Views[contextLogicRule.View];
            Description = contextLogicRule.Description;
            ID = contextLogicRule.ID;
            Index = contextLogicRule.Index;
            IsRootView = contextLogicRule.IsRootView;
            Nesting = contextLogicRule.Nesting;
            if (!string.IsNullOrEmpty(contextLogicRule.View))
                ((ILogicRule)this).View = CaptionHelper.ApplicationModel.Views[contextLogicRule.View];
            ViewEditMode = contextLogicRule.ViewEditMode;
            ViewType = contextLogicRule.ViewType;
            if (contextLogicRule.ObjectTypeData!=null)
                ObjectType = contextLogicRule.ObjectTypeData;
            NormalCriteria=contextLogicRule.NormalCriteria;
            EmptyCriteria=contextLogicRule.EmptyCriteria;
        }
        
        


        [RuleRequiredField]
        public Type ObjectType { get; set; }
        [RuleRequiredField]
        public string ID { get; set; }

        public string FrameTemplateContextGroup { get; set; }

        public string ExecutionContextGroup { get; set; }

        public string ActionExecutionContextGroup { get; set; }

        public string ViewContextGroup { get; set; }

        public string NormalCriteria { get; set; }

        public string EmptyCriteria { get; set; }
        public bool? IsNew { get; set; }

        public bool? IsRootView { get; set; }


        public int? Index { get; set; }

        public ViewType ViewType { get; set; }
        public ViewEditMode? ViewEditMode { get; set; }

        public IModelView View { get; set; }

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
