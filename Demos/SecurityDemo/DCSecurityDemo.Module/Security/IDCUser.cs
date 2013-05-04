using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;

namespace DCSecurityDemo.Module.Security {
    [DomainComponent]
    [ImageName("BO_User")]
    [XafDisplayName("User")]
    [XafDefaultProperty("UserName")]
    public interface IDCUser : ISecurityUser, ISecurityUserWithRoles, IOperationPermissionProvider, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser {
        new String UserName { get; set; }
        new Boolean IsActive { get; set; }
        new Boolean ChangePasswordOnFirstLogon { get; set; }
        [Browsable(false)]
        String StoredPassword { get; set; }
        [XafDisplayName("Roles")]
        [RuleRequiredField("IDCUser_StoredRoles_RuleRequiredField", DefaultContexts.Save, TargetCriteria = "IsActive=True", CustomMessageTemplate = "An active user must have at least one role assigned.")]
        IList<IDCRole> StoredRoles { get; }

        [Browsable(false)]
        new IList<ISecurityRole> Roles { get; }
    }
    [DomainLogic(typeof(IDCUser))]
    public class IDCUserLogic {
        public static Boolean ComparePassword(IDCUser user, String password) {
            return UserImpl.ComparePassword(user.StoredPassword, password);
        }
        public static void SetPassword(IDCUser user, String password) {
            user.StoredPassword = UserImpl.GeneratePassword(password);
        }
        public static IList<ISecurityRole> Get_Roles(IDCUser user) {
            return new ListConverter<ISecurityRole, IDCRole>(user.StoredRoles);
        }
        public static IEnumerable<IOperationPermission> GetPermissions() {
            return new IOperationPermission[0];
        }
        public static IEnumerable<IOperationPermissionProvider> GetChildren(IDCUser user) {
            return new ListConverter<IOperationPermissionProvider, IDCRole>(user.StoredRoles);
        }
    }
}
