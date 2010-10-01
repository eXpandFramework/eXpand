using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof(IModelExecutionContext))]
    public class ModelExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelExecutionContext modelExecutionContext) {
            return Enum.GetValues(typeof(ExecutionContext)).OfType<ExecutionContext>().Select(context => context.ToString()).ToList();
        }
    }
    [DomainLogic(typeof(IModelViewContext))]
    public class ModelViewContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelViewContext modelViewContext) {
            return Enum.GetValues(typeof(ExecutionContext)).OfType<ExecutionContext>().Select(context => context.ToString()).ToList();
        }
    }
}
