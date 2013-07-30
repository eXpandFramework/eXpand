using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.General;
using StateMachineTester.Module.BusinessObjects;
using Xpand.ExpressApp.Security.Core;
using System.Linq;
using Xpand.ExpressApp.StateMachine.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;

namespace StateMachineTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

            var adminRole =  ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");
            

            var userRole = (XpandRole) ObjectSpace.GetRole("User");
            if (ObjectSpace.IsNewObject(userRole)) {
                var permissionData = ObjectSpace.CreateObject<StateMachineTransitionOperationPermissionData>();
                permissionData.StateCaption = "InProgress";
                permissionData.StateMachineName = "Test";
                permissionData.Modifier = StateMachineTransitionModifier.Deny;
                userRole.Permissions.Add(permissionData);
            }

            userRole.SetTypePermissions<TestTask>( SecurityOperations.FullAccess, SecuritySystemModifier.Allow);
            userRole.SetTypePermissions<XpoStateMachine>(SecurityOperations.FullAccess, SecuritySystemModifier.Allow);
            userRole.SetTypePermissions<XpoState>(SecurityOperations.FullAccess, SecuritySystemModifier.Allow);
            userRole.SetTypePermissions<XpoTransition>(SecurityOperations.FullAccess, SecuritySystemModifier.Allow);
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            var stateMachine = ObjectSpace.FindObject<XpoStateMachine>(CriteriaOperator.Parse("Name=?", "Test")) ??
                               ObjectSpace.CreateObject<XpoStateMachine>();
            stateMachine.Active = true;
            stateMachine.Name = "Test";
            stateMachine.TargetObjectType = typeof (TestTask);
            stateMachine.StatePropertyName = new StringObject("Status");
            stateMachine.StartState = GetXpoState(TaskStatus.NotStarted,stateMachine);
            foreach (var taskStatus in Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>()) {
                GetXpoState(taskStatus, stateMachine);
            }
            CreateTransition(TaskStatus.NotStarted, TaskStatus.InProgress);
            CreateTransition(TaskStatus.InProgress, TaskStatus.Completed);
            ObjectSpace.FindObject<XpoState>(CriteriaOperator.Parse("Caption=?", TaskStatus.Completed.ToString())).TargetObjectCriteria = IsAllowedToRoleOperator.OperatorName + "('Admin')";
            ObjectSpace.CommitChanges();
        }

        void CreateTransition(TaskStatus startStatus, TaskStatus endStatus) {
            var caption = startStatus.ToString() + endStatus;
            var transition = ObjectSpace.FindObject<XpoTransition>(CriteriaOperator.Parse("Caption=?", caption)) ?? ObjectSpace.CreateObject<XpoTransition>();
            transition.Caption = caption;
            transition.SourceState = ObjectSpace.FindObject<XpoState>(CriteriaOperator.Parse("Caption=?", startStatus.ToString()));
            transition.TargetState =ObjectSpace.FindObject<XpoState>(CriteriaOperator.Parse("Caption=?", endStatus.ToString()));
        }

        XpoState GetXpoState(TaskStatus taskStatus, XpoStateMachine stateMachine) {
            var xpoState = ObjectSpace.FindObject<XpoState>(CriteriaOperator.Parse("Caption=?", taskStatus.ToString()))??ObjectSpace.CreateObject<XpoState>();
            xpoState.Caption = taskStatus.ToString();
            xpoState.StateMachine=stateMachine;
            xpoState.MarkerValue = taskStatus.ToString();
            return xpoState;
        }
    }
}
