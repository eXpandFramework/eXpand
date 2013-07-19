using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using System.ServiceModel;
using DevExpress.ExpressApp.Actions;
using WorkflowDemo.Module.Objects;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Workflow;

namespace WorkflowDemo.Module {

	[ServiceContract]
	public interface IPassIntegerData {
		[OperationContract(IsOneWay = true)]
		void PassIntegerData(int data);
	}

	[ServiceContract]
	public interface IPassStringData {
		[OperationContract(IsOneWay = true)]
		void PassStringData(string data);
	}

	public class StartWorkflowViaReceiveActivityController : ViewController {
		private const string WorkflowServerAddress = "http://localhost:46232/";
		private ParametrizedAction passIntAction;
		private ParametrizedAction passStringAction;

		private void passIntAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
			IWorkflowDefinition definition = View.ObjectSpace.FindObject<XpoWorkflowDefinition>(CriteriaOperator.Parse("Name = 'Custom start workflow'"));
			if(definition != null) {
				IPassIntegerData serverWorkflow = ChannelFactory<IPassIntegerData>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(WorkflowServerAddress + definition.GetUniqueId()));
				serverWorkflow.PassIntegerData((int)e.ParameterCurrentValue);
			}
		}
		private void passStringAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
			IWorkflowDefinition definition = View.ObjectSpace.FindObject<XpoWorkflowDefinition>(CriteriaOperator.Parse("Name = 'Custom start workflow'"));
			if(definition != null) {
				IPassStringData serverWorkflow = ChannelFactory<IPassStringData>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(WorkflowServerAddress + definition.GetUniqueId()));
				serverWorkflow.PassStringData((string)e.ParameterCurrentValue);
			}
		}

		public StartWorkflowViaReceiveActivityController() {
			TargetObjectType = typeof(Task);
			passIntAction = new ParametrizedAction(this, "StartWithInteger", PredefinedCategory.Edit, typeof(int));
			passIntAction.Execute += new ParametrizedActionExecuteEventHandler(passIntAction_Execute);
			passStringAction = new ParametrizedAction(this, "StartWithString", PredefinedCategory.Edit, typeof(string));
			passStringAction.Execute += new ParametrizedActionExecuteEventHandler(passStringAction_Execute);
		}

	}
}
