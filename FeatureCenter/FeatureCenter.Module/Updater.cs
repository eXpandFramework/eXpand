using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using FeatureCenter.Base;
using Xpand.ExpressApp.FilterDataStore.Providers;

namespace FeatureCenter.Module {

    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        protected bool initializeSecurity;

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override object EnsureUserExists(string userName, string firstName, object customizableRole) {
            var ensureUserExists = base.EnsureUserExists(userName, firstName, customizableRole);
            if (!IsNewSecuritySystem) {
                if (((IUser)ensureUserExists).UserName == Admin) {
                    ((User)ensureUserExists).SetPassword("Admin");
                    ObjectSpace.CommitChanges();
                    UserFilterProvider.UpdaterUserKey = ((User)ensureUserExists).Oid;
                }
            } else {
                if (((SecurityUser)ensureUserExists).UserName == Admin) {
                    ((SecurityUser)ensureUserExists).SetPassword("Admin");
                    ObjectSpace.CommitChanges();
                    UserFilterProvider.UpdaterUserKey = ((SecurityUser)ensureUserExists).Oid;
                }
            }
            return ensureUserExists;
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();


            InitializeSecurity();
            new DummyDataBuilder((ObjectSpace)ObjectSpace).CreateObjects();
            var workflowServiceUser = ObjectSpace.FindObject(SecuritySystem.UserType, new BinaryOperator("UserName", "WorkflowService"));
            if (workflowServiceUser == null) {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("Name=?", SecurityStrategy.AdministratorRoleName);
                CreateworkflowServiceUser(ObjectSpace.FindObject<Role>(criteriaOperator));
                CreateworkflowServiceUser(ObjectSpace.FindObject<SecurityRole>(criteriaOperator));
                ObjectSpace.CommitChanges();
                

                var updaters = ReflectionHelper.FindTypeDescendants(XafTypesInfo.CastTypeToTypeInfo(typeof(FCUpdater)));
                foreach (var findTypeDescendant in updaters) {
                    var updater = (FCUpdater)Activator.CreateInstance(findTypeDescendant.Type, ObjectSpace, CurrentDBVersion, this);
                    updater.UpdateDatabaseAfterUpdateSchema();
                }
            }

        }


        private void CreateworkflowServiceUser(SecurityRole securityRole) {
            var workflowServiceUser = ObjectSpace.CreateObject<SecurityUser>();
            workflowServiceUser.UserName = "WorkflowService";
            workflowServiceUser.Roles.Add(securityRole);
        }

        private void CreateworkflowServiceUser(Role role) {
            var workflowServiceUser = ObjectSpace.CreateObject<User>();
            workflowServiceUser.UserName = "WorkflowService";
            workflowServiceUser.FirstName = "WorkflowService";
            workflowServiceUser.Roles.Add(role);
        }
    }
}
