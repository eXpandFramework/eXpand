using DevExpress.ExpressApp.Workflow;

namespace Xpand.ExpressApp.Workflow {
    public interface IXpandWorkflowDefinition : IWorkflowDefinition {
        bool IsActive { get; set; }
    }
}
