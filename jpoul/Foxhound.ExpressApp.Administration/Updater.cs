using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using Foxhound.ExpressApp.Administration.BaseObjects;
using eXpand.Xpo;

namespace Foxhound.ExpressApp.Administration {
    public class Updater : ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            if (Session.FindObject<CompanyUnit>(null)== null){
                

                var taxonomy3 = new Taxonomy(Session) { Key = "administration", Name = "Administration", Group = "Module Taxonomies" };
                taxonomy3.Save();

                var structuralTerm = taxonomy3.GetTerm<StructuralTerm>("administration/Structural View/Companies/Offices/Units/Employees", string.Empty);
                structuralTerm.Save();
                taxonomy3.GetTerm<Term>("administration/Browsable View", string.Empty).Save();

//                using (var unitOfWork = new UnitOfWork(Session.DataLayer)){
//                    structuralTerm = (StructuralTerm) unitOfWork.GetObject(structuralTerm);
                    //structuralTerm.UpdateTypes(new[] { typeof(Employee), typeof(CompanyUnit), typeof(CompanyUnit), typeof(CompanyUnit)});
                ((StructuralTerm) structuralTerm.ParentTerm).TypeOfObject = typeof (CompanyUnit).AssemblyQualifiedName;
                structuralTerm.ParentTerm.Save();
                ((StructuralTerm)structuralTerm.ParentTerm.ParentTerm).TypeOfObject = typeof(CompanyUnit).AssemblyQualifiedName;
                structuralTerm.ParentTerm.ParentTerm.Save();
                ((StructuralTerm)structuralTerm.ParentTerm.ParentTerm.ParentTerm).TypeOfObject = typeof(CompanyUnit).AssemblyQualifiedName;
                structuralTerm.ParentTerm.ParentTerm.ParentTerm.Save();
//                    unitOfWork.CommitChanges();
//                }


                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Central Office/Accounting", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Central Office/Sales", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Central Office/Human Resources", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/London Offices", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Stores", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Stores/Store 1", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Stores/Store 2", string.Empty).Save();
                //taxonomy3.GetTerm<Term>("administration/Browsable View/This Company/Stores/Store 3", string.Empty).Save();

                
                //var acccounting = new CompanyUnit(Session){Name = "Acounting"};
                //acccounting.Save();
                //new CompanyUnit(Session) { Name = "My Company" }.Save();
                //new CompanyUnit(Session) { Name = "London Offices" }.Save();

                //var employee = new Employee(Session) { Caption = "Employee1"  };
                //employee.Save();
                //var employee1 = new Employee(Session) { Caption = "Employee2"  };
                //employee1.Save();
                //var employee2 = new Employee(Session) { Caption = "Employee3"  };
                //employee2.Save();
                //var employee3 = new Employee(Session) { Caption = "Employee4" };
                //employee3.Save();

            }
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
