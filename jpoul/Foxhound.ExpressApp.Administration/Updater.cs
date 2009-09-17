using System;
using System.Drawing;
using System.Security.Principal;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using Foxhound.ExpressApp.Administration.BaseObjects;

namespace Foxhound.ExpressApp.Administration {
    public class Updater : ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
//            base.UpdateDatabaseAfterUpdateSchema();
//
//            _EmployeeRole userRole = Session.FindObject<_EmployeeRole>(new BinaryOperator("Name", "Users"));
//            if (userRole == null) {
//                userRole = new _EmployeeRole(Session);
//                userRole.Name = "Users";
//            }
//
//            _EmployeeRole adminRole = Session.FindObject<_EmployeeRole>(new BinaryOperator("Name", "Administrators"));
//            if (adminRole == null) {
//                adminRole = new _EmployeeRole(Session);
//                adminRole.Name = "Administrators";
//            }
//            
//            adminRole.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
//            adminRole.AddPermission(new EditModelPermission(ModelAccessModifier.Allow));
//            adminRole.Save();
//            
//            userRole.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
//            userRole.AddPermission(new ObjectAccessPermission(typeof(Employee),
//               ObjectAccess.ChangeAccess, ObjectAccessModifier.Deny));
//            userRole.AddPermission(new ObjectAccessPermission(typeof(_EmployeeRole),
//               ObjectAccess.AllAccess, ObjectAccessModifier.Deny));
//            userRole.AddPermission(new EditModelPermission(ModelAccessModifier.Deny));
//            userRole.Save();
//
//            Employee srvAdmin = Session.FindObject<Employee>(new BinaryOperator("UserName", @"acz\srvadmin"));
//            if (srvAdmin == null) {
//                srvAdmin = new Employee(Session);
//                srvAdmin.Color = Color.Red;
//                srvAdmin.UserName = @"acz\srvadmin";
//                srvAdmin.FirstName = "Service Administrator";
//                srvAdmin.Caption = srvAdmin.FirstName;
//            }
//            
//            Employee cliAdmin = Session.FindObject<Employee>(new BinaryOperator("UserName", @"cli02\Administrator"));
//            if (cliAdmin == null) {
//                cliAdmin = new Employee(Session);
//                cliAdmin.Color = Color.Red;
//                cliAdmin.UserName = @"cli02\Administrator";
//                cliAdmin.FirstName = "CLI02 Administrator";
//                cliAdmin.Caption = cliAdmin.FirstName;
//            }
//
//            Employee spAdmin = Session.FindObject<Employee>(new BinaryOperator("UserName", @"SHAREPOINT\system"));
//            if (spAdmin == null) {
//                spAdmin = new Employee(Session);
//                spAdmin.Color = Color.Red;
//                spAdmin.UserName = @"SHAREPOINT\system";
//                spAdmin.FirstName = "SharePoint System Account";
//                spAdmin.Caption = cliAdmin.FirstName;
//            }
//            
//            spAdmin.EmployeeRoles.Add(adminRole);
//            cliAdmin.EmployeeRoles.Add(adminRole);
//            srvAdmin.EmployeeRoles.Add(adminRole);
//            
//            spAdmin.Save();
//            cliAdmin.Save();
//            srvAdmin.Save();
        }
    }
}
