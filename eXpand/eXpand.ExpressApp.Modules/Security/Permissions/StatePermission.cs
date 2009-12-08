using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.Security.Permissions
{
    public abstract class StatePermission : PermissionBase,IStateRule
    {
        public string ViewId { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        public Type ObjectType { get; set; }
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string ID { get; set; }
        public ViewType ViewType { get; set; }
        public Nesting Nesting { get; set; }
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
        public State State { get; set; }
        public string Description { get; set; }

        ITypeInfo IStateRule.TypeInfo{
            get { return XafTypesInfo.Instance.FindTypeInfo(ObjectType); }
            set { }
        }
    }
}