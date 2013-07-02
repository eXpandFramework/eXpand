using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    
    public class ModelExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelExecutionContext modelExecutionContext) {
            return Enum.GetValues(typeof(ExecutionContext)).OfType<ExecutionContext>().Select(context => context.ToString()).ToList();
        }
    }
    [DomainLogic(typeof(IModelActionExecutionContext))]
    public class ModelActionExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelActionExecutionContext modelExecutionContext) {
            return modelExecutionContext.Application.ActionDesign.Actions.Select(modelAction => modelAction.Id).ToList();
        }
    }

}
