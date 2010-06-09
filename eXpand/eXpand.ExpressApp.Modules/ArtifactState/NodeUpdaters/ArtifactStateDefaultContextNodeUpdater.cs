using System.Collections.Generic;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ArtifactState.NodeUpdaters {
    public abstract class ArtifactStateDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> {
                                                  ExecutionContext.CurrentObjectChanged,
                                                  ExecutionContext.ObjectChanged,
                                                  ExecutionContext.ObjectSpaceReloaded,
                                                  ExecutionContext.TemplateViewChanged,
                                                  ExecutionContext.ViewActivated,
                                                  ExecutionContext.ViewChanging,
                                                  ExecutionContext.ViewControlAdding,
                                                  ExecutionContext.ViewControlsCreated
                                              };
        }
    }
}