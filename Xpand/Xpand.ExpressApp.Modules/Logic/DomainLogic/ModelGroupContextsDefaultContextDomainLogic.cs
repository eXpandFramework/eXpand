using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (IModelGroupExecutionContexts))]
    public class ModelGroupContextsDefaultContextDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelGroupExecutionContexts modelGroupExecutionContexts){
            return modelGroupExecutionContexts.Where(context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).SingleOrDefault();
        }
    }
}