using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (ILogicRule))]
    public class LogicRuleExecutionContextGroupDomainLogic {
        public static string Get_ExecutionContextGroup(ILogicRule modelNode)
        {
            return LogicDefaultGroupContextNodeUpdater.Default;
        }
    }

}