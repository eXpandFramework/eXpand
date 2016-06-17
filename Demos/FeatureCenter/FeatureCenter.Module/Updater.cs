using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using FeatureCenter.Base;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.ModelDifference.Security;

namespace FeatureCenter.Module {

    public class Updater : ModuleUpdater {


        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }


        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            InitializeSecurity(ObjectSpace);

            new DummyDataBuilder((XPObjectSpace)ObjectSpace).CreateObjects();
            var workflowServiceUser = ObjectSpace.FindObject(SecuritySystem.UserType, new BinaryOperator("UserName", "WorkflowService"));
            if (workflowServiceUser == null) {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Name=?", SecurityStrategy.AdministratorRoleName);
                CreateworkflowServiceUser(ObjectSpace.FindObject<Role>(criteriaOperator));
                ObjectSpace.CommitChanges();


                var updaters = ReflectionHelper.FindTypeDescendants(XafTypesInfo.CastTypeToTypeInfo(typeof(FCUpdater)));
                foreach (var findTypeDescendant in updaters) {
                    var updater = (FCUpdater)Activator.CreateInstance(findTypeDescendant.Type, ObjectSpace, CurrentDBVersion);
                    updater.UpdateDatabaseAfterUpdateSchema();
                }
            }

        }

        public static void InitializeSecurity(IObjectSpace objectSpace) {
            var defaultRole = objectSpace.GetDefaultRole();
            var administratorRole = objectSpace.GetAdminRole("Administrator");
            var modelRole = objectSpace.GetDefaultModelRole("ModelDifference");

            objectSpace.GetUser("Admin", null, administratorRole);
            
            objectSpace.GetUser("User", "", defaultRole, modelRole);
            

            objectSpace.CommitChanges();
        }


        private void CreateworkflowServiceUser(Role role) {
            var workflowServiceUser = ObjectSpace.CreateObject<User>();
            workflowServiceUser.UserName = "WorkflowService";
            workflowServiceUser.FirstName = "WorkflowService";
            workflowServiceUser.Roles.Add(role);
        }
    }
}
