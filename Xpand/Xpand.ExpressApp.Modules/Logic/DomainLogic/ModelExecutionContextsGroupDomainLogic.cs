using System.Linq;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextsGroupDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelExecutionContextsGroup modelExecutionContextsGroup) {
            return modelExecutionContextsGroup.SingleOrDefault(context => context.Id == ContextLogicRuleDomainLogic.DefaultExecutionContextGroup);
        }

    }
}