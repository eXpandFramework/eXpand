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
    public class ModelFrameTemplateContextsDomainLogic {
        public static FrameTemplateContext Get_FrameTemplateContext(IModelFrameTemplateContexts modelFrameTemplateContexts) {
            var executionContexts = modelFrameTemplateContexts.Select(context => Enum.Parse(typeof(FrameTemplateContext), context.Name)).Cast<FrameTemplateContext>();
            return executionContexts.Aggregate(FrameTemplateContext.None, (current, context) => current | context);
        }
    }
}