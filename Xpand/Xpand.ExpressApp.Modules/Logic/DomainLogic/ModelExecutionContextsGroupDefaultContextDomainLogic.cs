using System.Linq;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextsGroupDefaultContextDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelExecutionContextsGroup modelExecutionContextsGroup) {
            return modelExecutionContextsGroup.SingleOrDefault(context => context.Id == LogicRuleDomainLogic.DefaultExecutionContextGroup);
        }
    }
}