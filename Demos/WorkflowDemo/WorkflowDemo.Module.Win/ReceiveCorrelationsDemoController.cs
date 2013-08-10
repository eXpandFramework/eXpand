using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Actions;
using System.ServiceModel;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Workflow.Xpo;
using System.Threading;
using DevExpress.ExpressApp;
using WorkflowDemo.Module.Objects;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;

namespace WorkflowDemo.Module {

	[ServiceContract]
	public interface IStartStop {

		[OperationContract(IsOneWay = false)]
		[return: MessageParameterAttribute(Name="result")]
		string Start(string clientId);

		[OperationContract(IsOneWay = false)]
		[return: MessageParameterAttribute(Name="result")]
		string Stop(string clientId);
	}

	public class ReceiveCorrelationsDemoController : ObjectViewController {
		private const string WorkflowServerAddress = "http://localhost:46232/";
		private ParametrizedAction startWorkflowAction;
		private ParametrizedAction stopWorkflowAction;

		private IStartStop StartStopClient {
			get {
				IWorkflowDefinition definition = View.ObjectSpace.FindObject<XpoWorkflowDefinition>(CriteriaOperator.Parse("Name = 'Start/stop (correlations) demo'"));
				if(definition != null) {
					return ChannelFactory<IStartStop>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(WorkflowServerAddress + definition.GetUniqueId()));
				}
				return null;
			}
		}

		private void startWorkflowAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
			if(StartStopClient != null) {
				string result = StartStopClient.Start((string)e.ParameterCurrentValue);
				XtraMessageBox.Show(result);
			}
		}
		private void stopWorkflowAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
			if(StartStopClient != null) {
				string result = StartStopClient.Stop((string)e.ParameterCurrentValue);
				XtraMessageBox.Show(result);
			}
		}

		public ReceiveCorrelationsDemoController() {
			TargetObjectType = typeof(Task);
			startWorkflowAction = new ParametrizedAction(this, "StartWorkflow", PredefinedCategory.Edit, typeof(string));
			startWorkflowAction.Execute += new ParametrizedActionExecuteEventHandler(startWorkflowAction_Execute);
			stopWorkflowAction = new ParametrizedAction(this, "StopWorkflow", PredefinedCategory.Edit, typeof(string));
			stopWorkflowAction.Execute += new ParametrizedActionExecuteEventHandler(stopWorkflowAction_Execute);
		}
                                      
	}
}
