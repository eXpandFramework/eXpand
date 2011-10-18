using System;
using System.Collections.Generic;
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
            objectSpace.Delete(request);
            objectSpace.CommitChanges();
        }
        public virtual void ProcessRequestsToStartWorkflows() {
            using (IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
                foreach (var request in objectSpace.GetObjects<ObjectChangedXpoStartWorkflowRequest>()) {
                    try {
                        var definition = GetService<IWorkflowDefinitionProvider>().FindDefinition(request.TargetWorkflowUniqueId);
                        if (definition != null && definition.CanOpenHost) {
                            if (GetService<ObjectChangedStartWorkflowService>().StartWorkflow(definition.Name, request.TargetWorkflowUniqueId, request.TargetObjectKey, request.PropertyName, request.OldValue)) {
                                OnRequestProcessed(objectSpace, request);
                            }
                        }
                    } catch (Exception e) {
                        e.Data.Add("StartWorkflowOnObjectChangeService.ProcessRequestsToStartWorkflows.currentRequest",
                                   string.Format("Key={0}, TargetObjectKey={1}, TargetWorkflowUniqueId={2}", objectSpace.GetKeyValue(request), request.TargetObjectKey, request.TargetWorkflowUniqueId));
                        throw;
                    }
                }
            }
        }
    }
}