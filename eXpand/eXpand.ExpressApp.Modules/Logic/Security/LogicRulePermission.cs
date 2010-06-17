using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Logic.NodeUpdaters;
using PermissionBase = eXpand.ExpressApp.Security.Permissions.PermissionBase;

namespace eXpand.ExpressApp.Logic.Security
{
    public abstract class LogicRulePermission : PermissionBase, ILogicRule
    {
        protected LogicRulePermission() {
            ExecutionContextGroup = LogicDefaultGroupContextNodeUpdater.Default;
        }

        public string ViewId { get; set; }


        [RuleRequiredField]
        public Type ObjectType { get; set; }
        [RuleRequiredField]
        public string ID { get; set; }

        public string ExecutionContextGroup { get; set; }


        public int Index { get; set; }

        public ViewType ViewType { get; set; }
        public Nesting Nesting { get; set; }


        string IRule.Id {
            get { return ID; }
            set { ID=value; }
        }
        ITypeInfo ILogicRule.TypeInfo
        {
            get { return XafTypesInfo.Instance.FindTypeInfo(ObjectType); }
            set { }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description { get; set; }


    }
}