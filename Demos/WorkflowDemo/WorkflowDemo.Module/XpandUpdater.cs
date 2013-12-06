using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using WorkflowDemo.Module.Objects;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace WorkflowDemo.Module {
    public class XpandUpdater:ModuleUpdater {
        const string CreateTaskForObjectChangedWorkflow = @"<Activity mc:Ignorable=""sads sap"" x:Class=""DevExpress.Workflow.xWF1""
 xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
 xmlns:dpb=""clr-namespace:DevExpress.Persistent.BaseImpl;assembly=DevExpress.Persistent.BaseImpl.v13.2""
 xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v13.2""
 xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Data.v13.2""
 xmlns:dx1=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v13.2""
 xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v13.2""
 xmlns:dxh1=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Xpo.v13.2""
 xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v13.2""
 xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
 xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities""
 xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities""
 xmlns:sads=""http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger""
 xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation""
 xmlns:wmo=""clr-namespace:WorkflowDemo.Module.Objects;assembly=WorkflowDemo.Module""
 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
    <x:Property Name=""propertyName"" Type=""InArgument(x:String)"" />
    <x:Property Name=""oldValue"" Type=""InArgument(x:Object)"" />
  </x:Members>
  <sap:VirtualizedContainerService.HintSize>308,245</sap:VirtualizedContainerService.HintSize>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""268,205"">
    <dwa:ObjectSpaceTransactionScope.Variables>
      <Variable x:TypeArguments=""wmo:Task"" Name=""task"" />
    </dwa:ObjectSpaceTransactionScope.Variables>
    <dwa:CreateObject x:TypeArguments=""wmo:Task"" sap:VirtualizedContainerService.HintSize=""242,22"" Result=""[task]"" />
    <Assign sap:VirtualizedContainerService.HintSize=""242,60"">
      <Assign.To>
        <OutArgument x:TypeArguments=""x:String"">[task.Subject]</OutArgument>
      </Assign.To>
      <Assign.Value>
        <InArgument x:TypeArguments=""x:String"">Task created from ObjectChanged Workflow</InArgument>
      </Assign.Value>
    </Assign>
  </dwa:ObjectSpaceTransactionScope>
</Activity>";


        const string CreateTaskForScheduledWorkflow = @"<Activity mc:Ignorable=""sads sap"" x:Class=""DevExpress.Workflow.XafWorkflow""
 xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
 xmlns:dpb=""clr-namespace:DevExpress.Persistent.BaseImpl;assembly=DevExpress.Persistent.BaseImpl.v13.2""
 xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v13.2""
 xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Data.v13.2""
 xmlns:dx1=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v13.2""
 xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v13.2""
 xmlns:dxh1=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Xpo.v13.2""
 xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v13.2""
 xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
 xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities""
 xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities""
 xmlns:sads=""http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger""
 xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation""
 xmlns:wmo=""clr-namespace:WorkflowDemo.Module.Objects;assembly=WorkflowDemo.Module""
 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
  </x:Members>
  <sap:VirtualizedContainerService.HintSize>308,245</sap:VirtualizedContainerService.HintSize>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""268,205"">
    <dwa:ObjectSpaceTransactionScope.Variables>
      <Variable x:TypeArguments=""wmo:Task"" Name=""task"" />
    </dwa:ObjectSpaceTransactionScope.Variables>
    <dwa:CreateObject x:TypeArguments=""wmo:Task"" sap:VirtualizedContainerService.HintSize=""242,22"" Result=""[task]"" />
    <Assign sap:VirtualizedContainerService.HintSize=""242,60"">
      <Assign.To>
        <OutArgument x:TypeArguments=""x:String"">[task.Subject]</OutArgument>
      </Assign.To>
      <Assign.Value>
        <InArgument x:TypeArguments=""x:String"">Task created from Scheduled Workflow</InArgument>
      </Assign.Value>
    </Assign>
  </dwa:ObjectSpaceTransactionScope>
</Activity>";

        public XpandUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<ScheduledWorkflow>(null) == null) {
                CreateScheduledWorkflow();
                CreateObjectChangedWorkflow();
                var user = CreateSecurityObjects();
                CreateIssues(user);
                ObjectSpace.CommitChanges();
            }            
        }

        void CreateObjectChangedWorkflow() {
            var objectChangedWorkflow = ObjectSpace.CreateObject<ObjectChangedWorkflow>();
            objectChangedWorkflow.IsActive = true;
            objectChangedWorkflow.Name = "Create Task on Issue Subject changed";
            objectChangedWorkflow.PropertyName = "Subject";
            objectChangedWorkflow.TargetObjectType = typeof (Issue);
            objectChangedWorkflow.ExecutionDomain=ExecutionDomain.Client;
            objectChangedWorkflow.Xaml = CreateTaskForObjectChangedWorkflow.Replace(".v13.2",AssemblyInfo.VSuffix);
        }

        void CreateScheduledWorkflow() {
            var scheduledWorkflow = ObjectSpace.CreateObject<ScheduledWorkflow>();
            scheduledWorkflow.Name = "Create Task every day";
            scheduledWorkflow.Xaml = CreateTaskForScheduledWorkflow.Replace(".v13.2", AssemblyInfo.VSuffix);
            scheduledWorkflow.IsActive = true;
            var launchSchedule = ObjectSpace.CreateObject<ScheduledWorkflowLaunchSchedule>();
            launchSchedule.StartMode = StartMode.Daily;
            launchSchedule.StartTime = DateTime.Now.TimeOfDay;
            launchSchedule.RuntASAPIfScheduledStartIsMissed = true;
            scheduledWorkflow.LaunchScheduleItems.Add(launchSchedule);
        }


        void CreateIssues(User user) {
            if (ObjectSpace.GetObjects<Issue>().Count == 0) {
                var issue = ObjectSpace.CreateObject<Issue>();
                issue.Subject = "Processed issue";
                issue.Active = false;
                issue.SetCreatedBy(user);

                var issue2 = ObjectSpace.CreateObject<Issue>();
                issue2.Subject = "Active issue";
                issue2.Active = true;
                issue2.SetCreatedBy(user);
            }
        }
        User CreateSecurityObjects() {
            var user = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "Sam"));
            if (user == null) {
                user = ObjectSpace.CreateObject<User>();
                user.UserName = "Sam";
                user.FirstName = "Sam";
            }
            var user2 = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "John"));
            if (user2 == null) {
                user2 = ObjectSpace.CreateObject<User>();
                user2.UserName = "John";
                user2.FirstName = "John";
            }

            var workflowServiceUser = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "WorkflowService"));
            if (workflowServiceUser == null) {
                workflowServiceUser = ObjectSpace.CreateObject<User>();
                workflowServiceUser.UserName = "WorkflowService";
                workflowServiceUser.FirstName = "WorkflowService";
            }

            var adminRole = ObjectSpace.FindObject<Role>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null) {
                adminRole = ObjectSpace.CreateObject<Role>();
                adminRole.Name = "Administrators";
                adminRole.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
                adminRole.AddPermission(new EditModelPermission(ModelAccessModifier.Allow));
                adminRole.Users.Add(user);
                adminRole.Users.Add(workflowServiceUser);
            }
            return user;
        }

    }
}
