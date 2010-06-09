using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using IRule = eXpand.ExpressApp.Logic.IRule;

namespace eXpand.ExpressApp.Logic.Security
{
    public abstract class LogicRulePermission : PermissionBase,ILogicRule
    {
        public string ViewId { get; set; }


        [RuleRequiredField(null, DefaultContexts.Save)]
        public Type ObjectType { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
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

        public string Description { get; set; }


//        IModelClass ILogicModelClassRule.ModelClass {
//            get { return (IModelClass) XafTypesInfo.Instance.FindTypeInfo(ObjectType); }
//            set { throw new NotImplementedException(); }
//        }
    }
}