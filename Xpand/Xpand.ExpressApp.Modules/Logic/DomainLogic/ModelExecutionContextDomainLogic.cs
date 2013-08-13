using System;
using System.Collections.Generic;
using System.Linq;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelExecutionContext modelExecutionContext) {
            return Enum.GetValues(typeof(System.Threading.ExecutionContext)).OfType<System.Threading.ExecutionContext>().Select(context => context.ToString()).ToList();
        }
    }
}