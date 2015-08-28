using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace WorkflowTester.Module.FunctionalTests.ObjectChangedWorkflow{
    public class ObjectChangedWorkflow : ObjectViewController<ObjectView, Xpand.ExpressApp.Workflow.ObjectChangedWorkflows.ObjectChangedWorkflow> {
        public ObjectChangedWorkflow(){
            var simpleAction = new SimpleAction(this, "AssignXaml", PredefinedCategory.ObjectsCreation);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var objectChangedWorkflow = ((Xpand.ExpressApp.Workflow.ObjectChangedWorkflows.ObjectChangedWorkflow) View.CurrentObject);
            objectChangedWorkflow.PropertyName = "PropertyName";
            objectChangedWorkflow.Xaml = @"<Activity mc:Ignorable=""sap sads"" x:Class=""DevExpress.Workflow.xWF1""
  xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
  xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v15.1""
  xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
  xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities""
  xmlns:sads=""http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger""
  xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation""
  xmlns:wmb=""clr-namespace:WorkflowTester.Module.BusinessObjects;assembly=WorkflowTester.Module""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <x:Members>
     <x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
     <x:Property Name=""propertyName"" Type=""InArgument(x:String)"" />
     <x:Property Name=""oldValue"" Type=""InArgument(x:Object)"" />
   </x:Members>
   <sap:VirtualizedContainerService.HintSize>266,160</sap:VirtualizedContainerService.HintSize>
   <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
   <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""226,120"">
     <dwa:CreateObject x:TypeArguments=""wmb:DemoTask"" sap:VirtualizedContainerService.HintSize=""200,22"" />
   </dwa:ObjectSpaceTransactionScope>
 </Activity>";
        }
    }
}