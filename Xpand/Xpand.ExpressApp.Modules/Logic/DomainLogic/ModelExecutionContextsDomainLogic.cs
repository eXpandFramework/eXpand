using System;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using System.Linq;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextsDomainLogic {
        public static ExecutionContext Get_ExecutionContext(IModelExecutionContexts modelExecutionContexts) {
            var executionContexts = modelExecutionContexts.Select(context => Enum.Parse(typeof (ExecutionContext), context.Name)).Cast<ExecutionContext>();
            return executionContexts.Aggregate(ExecutionContext.None, (current, context) => current | context);
        }
    }
}