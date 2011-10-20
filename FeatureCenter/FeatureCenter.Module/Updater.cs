using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
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

        public override IUserWithRoles EnsureUserExists(string userName, string firstName, ICustomizableRole customizableRole) {
            var ensureUserExists = base.EnsureUserExists(userName, firstName, customizableRole);
            if (ensureUserExists.UserName == Admin) {
                ((User)ensureUserExists).SetPassword("Admin");
                ObjectSpace.CommitChanges();
                UserFilterProvider.UpdaterUserKey = ((User)ensureUserExists).Oid;
            }
            return ensureUserExists;

        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            initializeSecurity = InitializeSecurity();
            if (initializeSecurity) {
                var workflowServiceUser = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "WorkflowService"));
                if (workflowServiceUser == null) {
                    workflowServiceUser = ObjectSpace.CreateObject<User>();
                    workflowServiceUser.UserName = "WorkflowService";
                    workflowServiceUser.FirstName = "WorkflowService";
                    var role = ObjectSpace.FindObject<Role>(CriteriaOperator.Parse("Name=?", Administrators));
                    workflowServiceUser.Roles.Add(role);
                    ObjectSpace.CommitChanges();
                    new DummyDataBuilder((ObjectSpace)ObjectSpace).CreateObjects();
                }
                var updaters = ReflectionHelper.FindTypeDescendants(XafTypesInfo.CastTypeToTypeInfo(typeof(FCUpdater)));
                foreach (var findTypeDescendant in updaters) {
                    var updater = (FCUpdater)Activator.CreateInstance(findTypeDescendant.Type, ObjectSpace, CurrentDBVersion, this);
                    updater.UpdateDatabaseAfterUpdateSchema();
                }
            }

        }

    }
}
