using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.Security.Permissions
{
    public abstract class StatePermission : PermissionBase,IStateRule
    {
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name { get; set; }
        public string View { get; set; }
        public Type ObjectType { get; set; }
        public ViewType ViewType { get; set; }
        public Nesting Nesting { get; set; }
        public string NormalCriteria { get; set; }
        public string EmptyCriteria { get; set; }
        
    }
}