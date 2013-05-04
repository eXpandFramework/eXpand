using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace DCSecurityDemo.Module.Security {
    [DomainComponent]
    [ImageName("BO_Role")]
    [XafDisplayName("Role")]
    [XafDefaultProperty("Name")]
    public interface IDCRole : ISecurityRole, ISecuritySystemRole, IOperationPermissionProvider {
        [RuleRequiredField("IDCRole_Name_RuleRequiredField", DefaultContexts.Save)]
        [RuleUniqueValue("IDCRole_Name_RuleUniqueValue", DefaultContexts.Save, "The role with the entered Name was already registered within the system.")]
        new String Name { get; set; }
        Boolean IsAdministrative { get; set; }
        Boolean CanEditModel { get; set; }
        [Aggregated]
        IList<IDCTypePermissions> TypePermissions { get; }
    }
    [DomainLogic(typeof(IDCRole))]
    public class IDCRoleLogic {
        public static IEnumerable<IOperationPermissionProvider> GetChildren() {
            return new IOperationPermissionProvider[0];
        }
        public static IEnumerable<IOperationPermission> GetPermissions(IDCRole role) {
            List<IOperationPermission> result = new List<IOperationPermission>();
            if(role.IsAdministrative) {
                result.Add(new IsAdministratorPermission());
            }
            if(role.CanEditModel) {
                result.Add(new ModelOperationPermission());
            }
            foreach(IDCTypePermissions persistentPermission in role.TypePermissions) {
                result.AddRange(persistentPermission.GetPermissions());
            }
            return result;
        }
    }
}
