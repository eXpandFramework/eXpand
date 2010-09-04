using System.Collections.Generic;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ArtifactState.NodeUpdaters {
    public abstract class ArtifactStateDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> {ExecutionContext.ViewChanging};
        }
    }
}