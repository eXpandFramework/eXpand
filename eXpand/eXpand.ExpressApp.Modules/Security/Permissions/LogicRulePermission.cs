using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Security.Permissions
{
    public abstract class LogicRulePermission : PermissionBase,ILogicRule
    {
        public string ViewId { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        public Type ObjectType { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string ID { get; set; }
        public ViewType ViewType { get; set; }
        public Nesting Nesting { get; set; }
        
        
        public string Description { get; set; }

        ITypeInfo ILogicRule.TypeInfo{
            get { return XafTypesInfo.Instance.FindTypeInfo(ObjectType); }
            set { }
        }
    }
}