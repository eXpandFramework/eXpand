using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.Base;

namespace WorkflowTester.Module.FunctionalTests {
    public class AssignXmlController:ViewController{
        public AssignXmlController(){
            var singleChoiceAction = new SingleChoiceAction(this, "AssignXaml", PredefinedCategory.ObjectsCreation);
            singleChoiceAction.Items.Add(new ChoiceActionItem("WorldCreatorWorkflow", "WorldCreatorWorkflow"));
            singleChoiceAction.Items.Add(new ChoiceActionItem("ObjectChangedWorkflow", "ObjectChangedWorkflow"));
            singleChoiceAction.Execute += SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if ((string) e.SelectedChoiceActionItem.Data == "WorldCreatorWorkflow"){
                var objectChangedWorkflow =((XpoWorkflowDefinition)View.CurrentObject);
                objectChangedWorkflow.Xaml = @"<Activity mc:Ignorable=""sads sap"" x:Class=""DevExpress.Workflow.XafWorkflow""
 xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
 xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v15.2""
 xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
 xmlns:mva=""clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities""
 xmlns:sa=""clr-namespace:System.Activities;assembly=System.Activities""
 xmlns:sads=""http://schemas.microsoft.com/netfx/2010/xaml/activities/debugger""
 xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation""
 xmlns:t=""clr-namespace:TestAssembly;assembly=TestAssembly""
 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <x:Members>
    <x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
    <x:Property Name=""argument1"" Type=""InArgument(x:String)"" />
  </x:Members>
  <sap:VirtualizedContainerService.HintSize>266,245</sap:VirtualizedContainerService.HintSize>
  <mva:VisualBasic.Settings>Assembly references and imported namespaces for internal implementation</mva:VisualBasic.Settings>
  <dwa:ObjectSpaceTransactionScope AutoCommit=""True"" sap:VirtualizedContainerService.HintSize=""226,205"">
    <dwa:ObjectSpaceTransactionScope.Variables>
      <Variable x:TypeArguments=""t:TestClass"" Name=""tc"" />
    </dwa:ObjectSpaceTransactionScope.Variables>
    <dwa:GetObjectByKey x:TypeArguments=""t:TestClass"" sap:VirtualizedContainerService.HintSize=""200,60"" Key=""[targetObjectId]"" Result=""[tc]"" />
    <dwa:CreateObject x:TypeArguments=""t:TestClass"" sap:VirtualizedContainerService.HintSize=""200,22"" />
  </dwa:ObjectSpaceTransactionScope>
</Activity>";

            }
            else if ((string) e.SelectedChoiceActionItem.Data == "ObjectChangedWorkflow"){
                var objectChangedWorkflow =
                    ((Xpand.ExpressApp.Workflow.ObjectChangedWorkflows.ObjectChangedWorkflow) View.CurrentObject);
                objectChangedWorkflow.PropertyName = "PropertyName";
                objectChangedWorkflow.Xaml = @"<Activity mc:Ignorable=""sap sads"" x:Class=""DevExpress.Workflow.xWF1""
  xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities""
  xmlns:dwa=""clr-namespace:DevExpress.Workflow.Activities;assembly=DevExpress.Workflow.Activities.v15.2""
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
}
