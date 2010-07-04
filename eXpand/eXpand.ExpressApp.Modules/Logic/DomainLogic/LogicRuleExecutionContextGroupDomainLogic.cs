using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (ILogicRule))]
    public class LogicRuleExecutionContextGroupDomainLogic {
        public static string Get_ExecutionContextGroup(IModelNode modelNode) {
            return LogicDefaultGroupContextNodeUpdater.Default;
        }
    }

}