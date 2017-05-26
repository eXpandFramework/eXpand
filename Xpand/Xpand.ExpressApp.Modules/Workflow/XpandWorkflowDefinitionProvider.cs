using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.CommonServices;
using System.Linq;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Workflow.Versioning;

namespace Xpand.ExpressApp.Workflow {
    public class XpandWorkflowDefinitionProvider<T> : ServiceBase,IWorkflowDefinitionProvider where T: IUserActivityVersionBase {
        readonly IList<Type> _workflowTypes;
        private readonly WorkflowVersioningEngine _workflowVersioningEngine;
        private IObjectSpaceProvider _objectSpaceProvider;

        protected virtual WorkflowVersioningEngine GetWorkflowVersioningEngine() {
            return _workflowVersioningEngine ?? new PersistentWorkflowVersioningEngine<T>(ObjectSpaceProvider);
        }
        public XpandWorkflowDefinitionProvider( IList<Type> workflowTypes, WorkflowVersioningEngine workflowVersioningEngine=null){
            _workflowVersioningEngine = workflowVersioningEngine;
            _workflowTypes = workflowTypes;
        }

        public IList<IWorkflowDefinition> GetDefinitions() {
            var result = new List<IWorkflowDefinition>();
            var objectSpace = ObjectSpaceProvider.CreateObjectSpace();
            foreach (var type in _workflowTypes) {
                var objects = objectSpace.GetObjects(type).OfType<IXpandWorkflowDefinition>();
                foreach (var definition in objects) {
                    result.Add(definition);
                }
            }
            WorkflowVersioningEngine versioningEngine = GetWorkflowVersioningEngine();
            return versioningEngine.GetVersionedDefinitions(result.ToArray());
        }

        public IObjectSpaceProvider ObjectSpaceProvider {
            get { return _objectSpaceProvider ?? GetService<IObjectSpaceProvider>(); }
            set { _objectSpaceProvider = value; }
        }

        public virtual IWorkflowDefinition FindDefinition(string uniqueId) {
            IWorkflowDefinition definition = null;
            foreach (IWorkflowDefinition item in GetDefinitions()) {
                if (item.GetUniqueId() == uniqueId) {
                    definition = item;
                }
            }
            return definition;
        }
    }
}
