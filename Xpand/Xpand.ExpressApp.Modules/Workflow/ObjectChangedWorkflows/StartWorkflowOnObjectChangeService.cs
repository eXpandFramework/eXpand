using System;
using System.Collections.Generic;
using System.ServiceModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;

namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public class StartWorkflowOnObjectChangeService : BaseTimerService {
        public StartWorkflowOnObjectChangeService(TimeSpan requestsDetectionPeriod)
            : this(requestsDetectionPeriod, null) {
        }
        public StartWorkflowOnObjectChangeService(TimeSpan requestsDetectionPeriod, IObjectSpaceProvider objectSpaceProvider)
            : base(requestsDetectionPeriod, objectSpaceProvider) {

        }
        public override void OnTimer() {
            ProcessRequestsToStartWorkflows();
        }
        protected virtual void OnRequestProcessed(IObjectSpace objectSpace, ObjectChangedXpoStartWorkflowRequest request) {

        }

        public virtual bool StartWorkflow(string targetWorkflowName, string targetWorkflowUniqueId, object targetObjectKey,
            string propertyName, object oldValue) {
            if (!HostManager.Hosts.ContainsKey(targetWorkflowUniqueId) || HostManager.Hosts[targetWorkflowUniqueId].State != CommunicationState.Opened) {
                return false;
            }
            var workflowHost = HostManager.Hosts[targetWorkflowUniqueId];
            var dictionary = Dictionary(targetObjectKey, propertyName, oldValue);
            Guid instanceHandle = workflowHost.StartWorkflow(dictionary);
            var runningWorkflowInstanceInfoService = GetService<IRunningWorkflowInstanceInfoService>();
            runningWorkflowInstanceInfoService.CreateRunningWorkflowInstanceInfo(targetWorkflowName, workflowHost.ActivityUniqueId, targetObjectKey, instanceHandle);
            return true;
        }

        public static Dictionary<string, object> Dictionary(object targetObjectKey, string propertyName, object oldValue) {
            return new Dictionary<string, object> { { "targetObjectId", targetObjectKey }, { "propertyName", propertyName }, { "oldValue", oldValue } };
        }

        public virtual void ProcessRequestsToStartWorkflows() {
            var objectChangedXpoStartWorkflowRequests = new List<ObjectChangedXpoStartWorkflowRequest>();
            using (IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
                foreach (var request in objectSpace.GetObjects<ObjectChangedXpoStartWorkflowRequest>()) {
                    try {
                        var definition = GetService<IWorkflowDefinitionProvider>().FindDefinition(request.TargetWorkflowUniqueId);
                        if (definition != null && definition.CanOpenHost) {
                            if (StartWorkflow(definition.Name, request.TargetWorkflowUniqueId, request.TargetObjectKey, request.PropertyName, request.OldValue)) {
                                OnRequestProcessed(objectSpace, request);
                                objectChangedXpoStartWorkflowRequests.Add(request);
                            }
                        }
                    } catch (Exception e) {
                        e.Data.Add("StartWorkflowOnObjectChangeService.ProcessRequestsToStartWorkflows.currentRequest",
                            $"Key={objectSpace.GetKeyValue(request)}, TargetObjectKey={request.TargetObjectKey}, TargetWorkflowUniqueId={request.TargetWorkflowUniqueId}");
                        throw;
                    }
                }
                objectSpace.Delete(objectChangedXpoStartWorkflowRequests);
                objectSpace.CommitChanges();
            }
        }
    }
}