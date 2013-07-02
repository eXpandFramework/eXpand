using System.Linq;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    
    public class ModelExecutionContextsGroupDefaultContextDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelExecutionContextsGroup modelExecutionContextsGroup) {
            return modelExecutionContextsGroup.SingleOrDefault(context => context.Id == LogicDefaultGroupContextNodeUpdater<IModelLogic, IModelNode>.Default);
        }
    }
}