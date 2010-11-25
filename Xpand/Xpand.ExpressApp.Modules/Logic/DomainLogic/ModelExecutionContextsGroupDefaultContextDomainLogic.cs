using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (IModelExecutionContextsGroup))]
    public class ModelExecutionContextsGroupDefaultContextDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelExecutionContextsGroup modelExecutionContextsGroup){
            return modelExecutionContextsGroup.Where(context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).SingleOrDefault();
        }
    }
}