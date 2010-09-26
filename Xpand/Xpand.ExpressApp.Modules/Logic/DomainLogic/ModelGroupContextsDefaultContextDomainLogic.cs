using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (IModelGroupContexts))]
    public class ModelGroupContextsDefaultContextDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelGroupContexts modelGroupContexts){
            return modelGroupContexts.Where(context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).SingleOrDefault();
        }
    }
}