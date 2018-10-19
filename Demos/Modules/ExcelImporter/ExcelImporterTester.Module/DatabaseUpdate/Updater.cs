using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using ExcelImporterTester.Module.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base;
using Xpand.Persistent.BaseImpl.Security;
using Xpand.Utils.Helpers;

namespace ExcelImporterTester.Module.DatabaseUpdate {
    
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            
            if (ObjectSpace.FindObject<XpandPermissionPolicyRole>(null) == null) {
                var defaultRole = (XpandPermissionPolicyRole)ObjectSpace.GetDefaultRole();

                var adminRole = ObjectSpace.GetAdminRole("Admin");
                adminRole.GetUser("Admin");

                var userRole = ObjectSpace.GetRole("User");
                var user = (XpandPermissionPolicyUser)userRole.GetUser("User");
                user.Roles.Add(defaultRole);
                var resourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), "MasterData.xlsx");
                
//                var excelImport = ObjectSpace.CreateObject<ExcelImport>();
//                var excelImportFile = new XpandFileData {Content = resourceStream.ReadFully()};
//                excelImport.File = excelImportFile;
//                excelImport.Name = "Import Customers -Genders";
//                excelImport.SheetName = "Active_List";
//                excelImport.Type = typeof(Customer);
//                excelImport.HeaderRows = 1;
//                excelImport.UseHeaderRows = true;
//                excelImport.Map();
            }

            var genderObject = ObjectSpace.CreateObject<GenderObject>();
            genderObject.Gender1 = "Male";
            genderObject = ObjectSpace.CreateObject<GenderObject>();
            genderObject.Gender1 = "FeMale";
            ObjectSpace.CommitChanges();

        }
    }
}
