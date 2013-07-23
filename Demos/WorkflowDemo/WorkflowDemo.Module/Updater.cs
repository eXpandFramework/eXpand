using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.BaseImpl;
using WorkflowDemo.Module.Objects;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;
using Task = WorkflowDemo.Module.Objects.Task;

namespace WorkflowDemo.Module {
    public class Updater : ModuleUpdater {
        const string CreateTaskForScheduledWorkflow = @"<Activity mc:Ignorable=""sads sap"" x:Class=""DevExpress.Workflow.XafWorkflow""
 xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
 xmlns:dpb=""clr-namespace:DevExpress.Persistent.BaseImpl;assembly=DevExpress.Persistent.BaseImpl.v13.1""
 xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v13.1""
 xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Data.v13.1""
 xmlns:dx1=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v13.1""
 xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v13.1""
 xmlns:dxh1=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Xpo.v13.1""
 xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v13.1""
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
        <InArgument x:TypeArguments=""x:String"">Created from Scheduled Workflow</InArgument>
      </Assign.Value>
    </Assign>
  </dwa:ObjectSpaceTransactionScope>
</Activity>";
        const string CreateTaskForActiveIssueWorkflowXaml =
            @"
<Activity mc:Ignorable=""sap"" x:Class=""DevExpress.Workflow.xWF1"" xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" xmlns:dpb=""clr-namespace:DevExpress.Persistent.BaseImpl;assembly=DevExpress.Persistent.BaseImpl.v13.1"" xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v13.1"" xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v13.1"" xmlns:dx1=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Data.v13.1"" xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Xpo.v13.1"" xmlns:dxh1=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v13.1"" xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v13.1"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:s1=""clr-namespace:System;assembly=System"" xmlns:s2=""clr-namespace:System;assembly=System.Core"" xmlns:s3=""clr-namespace:System;assembly=System.ServiceModel"" xmlns:s4=""clr-namespace:System;assembly=System.Drawing.Design"" xmlns:s5=""clr-namespace:System;assembly=System.Configuration.Install"" xmlns:s6=""clr-namespace:System;assembly=System.DirectoryServices.Protocols"" xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities"" xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"" xmlns:wc=""clr-namespace:WorkflowDemo.Module.Objects;assembly=WorkflowDemo.Module"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
  </x:Members>
  <sap:VirtualizedContainerService.HintSize>307.2,412.8</sap:VirtualizedContainerService.HintSize>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces serialized as XML namespaces</mva:VisualBasic.Settings>
  <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""267.2,372.8"" mva:VisualBasic.Settings=""Assembly references and imported namespaces serialized as XML namespaces"">
    <dwa:ObjectSpaceTransactionScope.Variables>
      <Variable x:TypeArguments=""wc:Issue"" Name=""issue"" />
      <Variable x:TypeArguments=""wc:Task"" Name=""task"" />
    </dwa:ObjectSpaceTransactionScope.Variables>
    <dwa:GetObjectByKey x:TypeArguments=""wc:Issue"" sap:VirtualizedContainerService.HintSize=""241.6,59.2"" Key=""[targetObjectId]"" Result=""[issue]"" />
    <dwa:CreateObject x:TypeArguments=""wc:Task"" sap:VirtualizedContainerService.HintSize=""241.6,22.4"" Result=""[task]"" />
    <Assign sap:VirtualizedContainerService.HintSize=""241.6,59.2"">
      <Assign.To>
        <OutArgument x:TypeArguments=""x:String"">[task.Subject]</OutArgument>
      </Assign.To>
      <Assign.Value>
        <InArgument x:TypeArguments=""x:String"">[""New active issue: "" + issue.Subject]</InArgument>
      </Assign.Value>
    </Assign>
    <Assign sap:VirtualizedContainerService.HintSize=""241.6,59.2"">
      <Assign.To>
        <OutArgument x:TypeArguments=""wc:Issue"">[task.Issue]</OutArgument>
      </Assign.To>
      <Assign.Value>
        <InArgument x:TypeArguments=""wc:Issue"">[issue]</InArgument>
      </Assign.Value>
    </Assign>
    <Delay Duration=""00:00:01"" sap:VirtualizedContainerService.HintSize=""242,22"" />
  </dwa:ObjectSpaceTransactionScope>
</Activity>";

        const string CodeActivityCreateTaskForActiveIssueWorkflowXaml =
            @"<Activity mc:Ignorable=""sap"" x:Class=""DevExpress.Workflow.xWF1"" xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:s1=""clr-namespace:System;assembly=System"" xmlns:s2=""clr-namespace:System;assembly=System.Core"" xmlns:s3=""clr-namespace:System;assembly=System.ServiceModel"" xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities"" xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"" xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" xmlns:wm=""clr-namespace:WorkflowDemo.Module.Activities;assembly=WorkflowDemo.Module"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
  </x:Members>
  <sap:VirtualizedContainerService.HintSize>273,285</sap:VirtualizedContainerService.HintSize>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces serialized as XML namespaces</mva:VisualBasic.Settings>
  <Sequence sap:VirtualizedContainerService.HintSize=""233,245"" mva:VisualBasic.Settings=""Assembly references and imported namespaces serialized as XML namespaces"">
    <Sequence.Variables>
      <Variable x:TypeArguments=""x:Object"" Name=""createdTaskKey"" />
    </Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg:Dictionary x:TypeArguments=""x:String, x:Object"">
        <x:Boolean x:Key=""IsExpanded"">True</x:Boolean>
      </scg:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <wm:CreateTask sap:VirtualizedContainerService.HintSize=""211,22"" createdTaskKey=""[createdTaskKey]"" issueKey=""[targetObjectId]"" />
    <WriteLine sap:VirtualizedContainerService.HintSize=""211,59"" Text=""[&quot;Write to track record: &quot; + createdTaskKey.ToString()]"" />
  </Sequence>
</Activity>";

        const string StartWorkflowViaReceiveAndCustomContractXaml =
            @"<Activity mc:Ignorable=""sap"" x:Class=""DevExpress.Workflow.xWF1"" sap:VirtualizedContainerService.HintSize=""812,627"" mva:VisualBasic.Settings=""Assembly references and imported namespaces serialized as XML namespaces"" xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" xmlns:dpb=""clr-namespace:DevExpress.Persistent.BaseImpl;assembly=DevExpress.Persistent.BaseImpl.v13.1"" xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v13.1"" xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Data.v13.1"" xmlns:dx1=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v13.1"" xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v13.1"" xmlns:dxh1=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Xpo.v13.1"" xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v13.1"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities"" xmlns:p=""http://schemas.microsoft.com/netfx/2009/xaml/servicemodel"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:s1=""clr-namespace:System;assembly=System"" xmlns:s2=""clr-namespace:System;assembly=System.Core"" xmlns:s3=""clr-namespace:System;assembly=System.ServiceModel"" xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities"" xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"" xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" xmlns:wmo=""clr-namespace:WorkflowDemo.Module.Objects;assembly=WorkflowDemo.Module"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Sequence sap:VirtualizedContainerService.HintSize=""772,587"" mva:VisualBasic.Settings=""Assembly references and imported namespaces serialized as XML namespaces"">
    <sap:WorkflowViewStateService.ViewState>
      <scg:Dictionary x:TypeArguments=""x:String, x:Object"">
        <x:Boolean x:Key=""IsExpanded"">True</x:Boolean>
      </scg:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <Pick sap:VirtualizedContainerService.HintSize=""750,463"">
      <PickBranch sap:VirtualizedContainerService.HintSize=""298,417"">
        <PickBranch.Variables>
          <Variable x:TypeArguments=""x:Int32"" Name=""intData"" />
        </PickBranch.Variables>
        <PickBranch.Trigger>
          <p:Receive CanCreateInstance=""True"" sap:VirtualizedContainerService.HintSize=""268,100"" OperationName=""PassIntegerData"" ServiceContractName=""IPassIntegerData"">
            <p:ReceiveParametersContent>
              <OutArgument x:TypeArguments=""x:Int32"" x:Key=""data"">[intData]</OutArgument>
            </p:ReceiveParametersContent>
          </p:Receive>
        </PickBranch.Trigger>
        <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""268,203"">
          <dwa:ObjectSpaceTransactionScope.Variables>
            <Variable x:TypeArguments=""wmo:Task"" Name=""task"" />
          </dwa:ObjectSpaceTransactionScope.Variables>
          <dwa:CreateObject x:TypeArguments=""wmo:Task"" sap:VirtualizedContainerService.HintSize=""242,22"" Result=""[task]"" />
          <Assign sap:VirtualizedContainerService.HintSize=""242,58"">
            <Assign.To>
              <OutArgument x:TypeArguments=""x:String"">[task.Subject]</OutArgument>
            </Assign.To>
            <Assign.Value>
              <InArgument x:TypeArguments=""x:String"">[""Integer data:"" + intData.ToString()]</InArgument>
            </Assign.Value>
          </Assign>
        </dwa:ObjectSpaceTransactionScope>
      </PickBranch>
      <PickBranch sap:VirtualizedContainerService.HintSize=""298,417"">
        <PickBranch.Variables>
          <Variable x:TypeArguments=""x:String"" Name=""stringData"" />
        </PickBranch.Variables>
        <PickBranch.Trigger>
          <p:Receive CanCreateInstance=""True"" sap:VirtualizedContainerService.HintSize=""268,100"" OperationName=""PassStringData"" ServiceContractName=""IPassStringData"">
            <p:ReceiveParametersContent>
              <OutArgument x:TypeArguments=""x:String"" x:Key=""data"">[stringData]</OutArgument>
            </p:ReceiveParametersContent>
          </p:Receive>
        </PickBranch.Trigger>
        <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""268,203"">
          <dwa:ObjectSpaceTransactionScope.Variables>
            <Variable x:TypeArguments=""wmo:Task"" Name=""task"" />
          </dwa:ObjectSpaceTransactionScope.Variables>
          <dwa:CreateObject x:TypeArguments=""wmo:Task"" sap:VirtualizedContainerService.HintSize=""242,22"" Result=""[task]"" />
          <Assign sap:VirtualizedContainerService.HintSize=""242,58"">
            <Assign.To>
              <OutArgument x:TypeArguments=""x:String"">[task.Subject]</OutArgument>
            </Assign.To>
            <Assign.Value>
              <InArgument x:TypeArguments=""x:String"">[""String data:"" + stringData]</InArgument>
            </Assign.Value>
          </Assign>
        </dwa:ObjectSpaceTransactionScope>
      </PickBranch>
    </Pick>
  </Sequence>
</Activity>
";

        const string ReceiveCorrelationsXaml =
            @"<Activity mc:Ignorable=""sap"" x:Class=""DevExpress.Workflow.xWF1"" sap:VirtualizedContainerService.HintSize=""330,1198"" mva:VisualBasic.Settings=""Assembly references and imported namespaces for internal implementation"" xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" xmlns:dpb=""clr-namespace:DevExpress.Persistent.BaseImpl;assembly=DevExpress.Persistent.BaseImpl.v13.1"" xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v13.1"" xmlns:dx=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Xpo.v13.1"" xmlns:dx1=""clr-namespace:DevExpress.Xpo;assembly=DevExpress.Data.v13.1"" xmlns:dxh=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Xpo.v13.1"" xmlns:dxh1=""clr-namespace:DevExpress.Xpo.Helpers;assembly=DevExpress.Data.v13.1"" xmlns:dxmh=""clr-namespace:DevExpress.Xpo.Metadata.Helpers;assembly=DevExpress.Xpo.v13.1"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:mv=""clr-namespace:Microsoft.VisualBasic;assembly=System"" xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities"" xmlns:p=""http://schemas.microsoft.com/netfx/2009/xaml/servicemodel"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:s1=""clr-namespace:System;assembly=System"" xmlns:s2=""clr-namespace:System;assembly=System.Xml"" xmlns:s3=""clr-namespace:System;assembly=System.Core"" xmlns:s4=""clr-namespace:System;assembly=System.ServiceModel"" xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities"" xmlns:sad=""clr-namespace:System.Activities.Debugger;assembly=System.Activities"" xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"" xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=System"" xmlns:scg1=""clr-namespace:System.Collections.Generic;assembly=System.ServiceModel"" xmlns:scg2=""clr-namespace:System.Collections.Generic;assembly=System.Core"" xmlns:scg3=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" xmlns:sd=""clr-namespace:System.Data;assembly=System.Data"" xmlns:sl=""clr-namespace:System.Linq;assembly=System.Core"" xmlns:ssa=""clr-namespace:System.ServiceModel.Activities;assembly=System.ServiceModel.Activities"" xmlns:ssx=""clr-namespace:System.ServiceModel.XamlIntegration;assembly=System.ServiceModel"" xmlns:st=""clr-namespace:System.Text;assembly=mscorlib"" xmlns:wmo=""clr-namespace:WorkflowDemo.Module.Objects;assembly=WorkflowDemo.Module"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Sequence DisplayName=""Main"" sap:VirtualizedContainerService.HintSize=""290,1158"">
    <Sequence.Variables>
      <Variable x:TypeArguments=""p:CorrelationHandle"" Name=""clientIdHandle"" />
      <Variable x:TypeArguments=""x:String"" Name=""localClientId"" />
      <Variable x:TypeArguments=""s:Guid"" Name=""taskId"" />
    </Sequence.Variables>
    <sap:WorkflowViewStateService.ViewState>
      <scg3:Dictionary x:TypeArguments=""x:String, x:Object"">
        <x:Boolean x:Key=""IsExpanded"">True</x:Boolean>
      </scg3:Dictionary>
    </sap:WorkflowViewStateService.ViewState>
    <p:Receive x:Name=""__ReferenceID0"" CanCreateInstance=""True"" CorrelatesWith=""[clientIdHandle]"" sap:VirtualizedContainerService.HintSize=""268,86"" OperationName=""Start"" ServiceContractName=""IStartStop"">
      <p:Receive.CorrelatesOn>
        <p:XPathMessageQuery x:Key=""key1"">
          <p:XPathMessageQuery.Namespaces>
            <ssx:XPathMessageContextMarkup>
              <x:String x:Key=""xgSc"">http://tempuri.org/</x:String>
            </ssx:XPathMessageContextMarkup>
          </p:XPathMessageQuery.Namespaces>sm:body()/xgSc:Start/xgSc:clientId</p:XPathMessageQuery>
      </p:Receive.CorrelatesOn>
      <p:ReceiveParametersContent>
        <OutArgument x:TypeArguments=""x:String"" x:Key=""clientId"">[localClientId]</OutArgument>
      </p:ReceiveParametersContent>
    </p:Receive>
    <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""268,333"">
      <dwa:ObjectSpaceTransactionScope.Variables>
        <Variable x:TypeArguments=""wmo:Task"" Name=""task"" />
      </dwa:ObjectSpaceTransactionScope.Variables>
      <dwa:CreateObject x:TypeArguments=""wmo:Task"" sap:VirtualizedContainerService.HintSize=""242,22"" Result=""[task]"" />
      <Assign sap:VirtualizedContainerService.HintSize=""242,58"">
        <Assign.To>
          <OutArgument x:TypeArguments=""x:String"">[task.Subject]</OutArgument>
        </Assign.To>
        <Assign.Value>
          <InArgument x:TypeArguments=""x:String"">[""Task for client "" + localClientId]</InArgument>
        </Assign.Value>
      </Assign>
      <dwa:CommitChanges sap:VirtualizedContainerService.HintSize=""242,22"" />
      <Assign sap:VirtualizedContainerService.HintSize=""242,58"">
        <Assign.To>
          <OutArgument x:TypeArguments=""s:Guid"">[taskId]</OutArgument>
        </Assign.To>
        <Assign.Value>
          <InArgument x:TypeArguments=""s:Guid"">[task.Oid]</InArgument>
        </Assign.Value>
      </Assign>
    </dwa:ObjectSpaceTransactionScope>
    <p:SendReply Request=""{x:Reference __ReferenceID0}"" DisplayName=""SendReplyToReceive"" sap:VirtualizedContainerService.HintSize=""268,86"">
      <p:SendParametersContent>
        <InArgument x:TypeArguments=""x:String"" x:Key=""result"">[""Task is created for client "" + localClientId + "".""]</InArgument>
      </p:SendParametersContent>
    </p:SendReply>
    <p:Receive x:Name=""__ReferenceID1"" CorrelatesWith=""[clientIdHandle]"" sap:VirtualizedContainerService.HintSize=""268,86"" OperationName=""Stop"" ServiceContractName=""IStartStop"">
      <p:Receive.CorrelatesOn>
        <p:XPathMessageQuery x:Key=""key1"">
          <p:XPathMessageQuery.Namespaces>
            <ssx:XPathMessageContextMarkup>
              <x:String x:Key=""xgSc"">http://tempuri.org/</x:String>
            </ssx:XPathMessageContextMarkup>
          </p:XPathMessageQuery.Namespaces>sm:body()/xgSc:Stop/xgSc:clientId</p:XPathMessageQuery>
      </p:Receive.CorrelatesOn>
      <p:ReceiveParametersContent>
        <OutArgument x:TypeArguments=""x:String"" x:Key=""clientId"" />
      </p:ReceiveParametersContent>
    </p:Receive>
    <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""268,157"">
      <dwa:DeleteObject x:TypeArguments=""wmo:Task"" sap:VirtualizedContainerService.HintSize=""200,59"" Key=""[taskId]"" />
    </dwa:ObjectSpaceTransactionScope>
    <p:SendReply Request=""{x:Reference __ReferenceID1}"" DisplayName=""SendReplyToReceive"" sap:VirtualizedContainerService.HintSize=""268,86"">
      <p:SendParametersContent>
        <InArgument x:TypeArguments=""x:String"" x:Key=""result"">[""Task for client "" + localClientId + "" was deleted.""]</InArgument>
      </p:SendParametersContent>
    </p:SendReply>
  </Sequence>
</Activity>
";

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            CreateDemoObjects();
        }

        void CreateDemoObjects() {
            var user = CreateSecurityObjects();

            CreateIssues(user);

            CreateXpoWorrkflowDefinitions();

            if (ObjectSpace.FindObject<ScheduledWorkflow>(null) == null) {
                var scheduledWorkflow = ObjectSpace.CreateObject<ScheduledWorkflow>();
                scheduledWorkflow.Name = "Create Task every day";
                scheduledWorkflow.Xaml = CreateTaskForScheduledWorkflow;
                scheduledWorkflow.IsActive = true;
                var launchSchedule = ObjectSpace.CreateObject<ScheduledWorkflowLaunchSchedule>();
                launchSchedule.StartMode=StartMode.Daily;
                launchSchedule.StartTime=DateTime.Now.TimeOfDay;
                launchSchedule.RuntASAPIfScheduledStartIsMissed=true;
                scheduledWorkflow.LaunchScheduleItems.Add(launchSchedule);
            }
            ObjectSpace.CommitChanges();
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
                adminRole.AddPermission(new ObjectAccessPermission(typeof (object), ObjectAccess.AllAccess));
                adminRole.AddPermission(new EditModelPermission(ModelAccessModifier.Allow));
                adminRole.Users.Add(user);
                adminRole.Users.Add(workflowServiceUser);
            }
            return user;
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

        void CreateXpoWorrkflowDefinitions() {
            if (ObjectSpace.GetObjects<XpoWorkflowDefinition>().Count == 0) {
                var definition = ObjectSpace.CreateObject<XpoWorkflowDefinition>();
                definition.Name = "Create Task for active Issue";
                definition.Xaml = CreateTaskForActiveIssueWorkflowXaml;
                definition.TargetObjectType = typeof (Issue);
                definition.AutoStartWhenObjectFitsCriteria = true;
                definition.Criteria = "[Active] = True";
                definition.IsActive = true;

                var codeActivityDefinition = ObjectSpace.CreateObject<XpoWorkflowDefinition>();
                codeActivityDefinition.Name = "Create Task for active Issue (Code Activity)";
                codeActivityDefinition.Xaml = CodeActivityCreateTaskForActiveIssueWorkflowXaml;
                codeActivityDefinition.TargetObjectType = typeof (Issue);
                codeActivityDefinition.AutoStartWhenObjectFitsCriteria = true;
                codeActivityDefinition.Criteria = "Contains([Subject], 'Code Activity')";
                codeActivityDefinition.IsActive = true;

                var customStartWorkflowActivityDefinition = ObjectSpace.CreateObject<XpoWorkflowDefinition>();
                customStartWorkflowActivityDefinition.Name = "Custom start workflow";
                customStartWorkflowActivityDefinition.Xaml = StartWorkflowViaReceiveAndCustomContractXaml;
                customStartWorkflowActivityDefinition.TargetObjectType = typeof (Task);
                customStartWorkflowActivityDefinition.IsActive = true;


                var receiveCorrelationsActivityDefinition = ObjectSpace.CreateObject<XpoWorkflowDefinition>();
                receiveCorrelationsActivityDefinition.Name = "Start/stop (correlations) demo";
                receiveCorrelationsActivityDefinition.Xaml = ReceiveCorrelationsXaml;
                receiveCorrelationsActivityDefinition.TargetObjectType = typeof (Task);
                receiveCorrelationsActivityDefinition.IsActive = true;
            }
        }
    }
}