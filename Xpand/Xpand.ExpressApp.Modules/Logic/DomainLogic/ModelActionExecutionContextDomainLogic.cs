using System.Collections.Generic;
using System.Linq;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelActionExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelActionExecutionContext modelExecutionContext) {
            return modelExecutionContext.Application.ActionDesign.Actions.Select(modelAction => modelAction.Id).ToList();
        }
    }
}
