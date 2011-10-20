using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using FeatureCenter.Base;
using Xpand.ExpressApp.FilterDataStore.Providers;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;

namespace FeatureCenter.Module {

    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        protected override IUserWithRoles EnsureUserExists(string userName, string firstName, ICustomizableRole customizableRole) {
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
            
            var o = ((ObjectSpace)ObjectSpace).Session.FindObject<ObjectChangedXpoStartWorkflowRequest>(null);
            var count = ObjectSpace.GetObjects<XpoStartWorkflowRequest>().Count;
            count = ObjectSpace.GetObjects<ObjectChangedWorkflow>().Count;
            InitializeSecurity();
            var workflowServiceUser = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "WorkflowService"));
            if (workflowServiceUser == null) {
                workflowServiceUser = ObjectSpace.CreateObject<User>();
                workflowServiceUser.UserName = "WorkflowService";
                workflowServiceUser.FirstName = "WorkflowService";
                var role = ObjectSpace.FindObject<Role>(CriteriaOperator.Parse("Name=?", Administrators));
                workflowServiceUser.Roles.Add(role);
            }
            ObjectSpace.CommitChanges();
            new DummyDataBuilder((ObjectSpace)ObjectSpace).CreateObjects();
        }

    }
}
