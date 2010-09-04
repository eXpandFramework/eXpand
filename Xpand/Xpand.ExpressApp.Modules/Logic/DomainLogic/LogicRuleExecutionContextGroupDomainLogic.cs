using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (ILogicRule))]
    public class LogicRuleExecutionContextGroupDomainLogic {
        public static string Get_ExecutionContextGroup(ILogicRule modelNode)
        {
            return LogicDefaultGroupContextNodeUpdater.Default;
        }
    }

}