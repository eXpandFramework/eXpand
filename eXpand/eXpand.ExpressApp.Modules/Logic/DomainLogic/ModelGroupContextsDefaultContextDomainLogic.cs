using System.Linq;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (IModelGroupContexts))]
    public class ModelGroupContextsDefaultContextDomainLogic {
        public static IModelExecutionContexts Get_DefaultContext(IModelGroupContexts modelGroupContexts){
            return modelGroupContexts.Where(context => context.Id == LogicDefaultGroupContextNodeUpdater.Default).Single();
        }
    }
}