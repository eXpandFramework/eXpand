using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.CommonServices;
using System.Linq;

namespace Xpand.ExpressApp.Workflow {
    public class XpandWorkflowDefinitionProvider : WorkflowDefinitionProvider {
        readonly IList<Type> _types;

        public XpandWorkflowDefinitionProvider(Type workflowDefinitionType, IList<Type> types)
            : base(workflowDefinitionType) {
            _types = types;
        }

        public XpandWorkflowDefinitionProvider(Type workflowDefinitionType, IObjectSpaceProvider objectSpaceProvider) : base(workflowDefinitionType, objectSpaceProvider) { }
        public override IList<IWorkflowDefinition> GetDefinitions() {
            IList<IWorkflowDefinition> result = base.GetDefinitions();
            IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace();
            foreach (var type in _types) {
                IEnumerable<IXpandWorkflowDefinition> objects = objectSpace.GetObjects(type).OfType<IXpandWorkflowDefinition>();
                foreach (var definition in objects) {
                    result.Add(definition);
                }
            }
            return result;
        }
    }
}
