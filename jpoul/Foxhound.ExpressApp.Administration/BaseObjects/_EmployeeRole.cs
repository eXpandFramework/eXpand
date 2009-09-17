using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    [DefaultClassOptions]
    public class _EmployeeRole : RoleBase, IRole{
        public const string DefaultAdministratorsGroupName = "Administrators";
        public const string DefaultUsersGroupName = "Users";
        public _EmployeeRole(Session session) : base(session) {}

        [Association("User-Role", UseAssociationNameAsIntermediateTableName = true)]
        public XPCollection<Employee> Employees{
            get { return GetCollection<Employee>("Employees"); }
        }

        [Association("EmployeeRole-EmployeeContracts")]
        public XPCollection<EmployeeContract> EmployeeContracts{
            get { return GetCollection<EmployeeContract>("EmployeeContracts"); }
        }
        #region IRole Members
        IList<IUser> IRole.Users{
            get { return new ListConverter<IUser, Employee>(Employees); }
        }
        #endregion
    }
}