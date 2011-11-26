using System;
using System.Collections.Generic;
using System.ServiceModel;
using DevExpress.ExpressApp.Workflow.Server;

namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public class ObjectChangedStartWorkflowService : WorkflowServerService {
        public virtual bool StartWorkflow(string targetWorkflowName, string targetWorkflowUniqueId, object targetObjectKey,
                                          string propertyName, object oldValue) {
            if (!HostManager.Hosts.ContainsKey(targetWorkflowUniqueId) || HostManager.Hosts[targetWorkflowUniqueId].State != CommunicationState.Opened) {
                return false;
            }
            var workflowHost = HostManager.Hosts[targetWorkflowUniqueId];
            var dictionary = Dictionary(targetObjectKey, propertyName, oldValue);
            Guid instanceHandle = workflowHost.StartWorkflow(dictionary);
            var runningWorkflowInstanceInfoService = GetService<IRunningWorkflowInstanceInfoService>();
            runningWorkflowInstanceInfoService.CreateRunningWorkflowInstanceInfo(targetWorkflowName, workflowHost.ActivityUnigueId, targetObjectKey, instanceHandle);
            return true;
        }

        public static Dictionary<string, object> Dictionary(object targetObjectKey, string propertyName, object oldValue) {
            return new Dictionary<string, object> { { "targetObjectId", targetObjectKey }, { "propertyName", propertyName }, { "oldValue", oldValue } };
        }
    }
}