using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof (ILogicRule))]
    public class LogicRuleExecutionContextGroupDomainLogic {
        bool gettingExecutionContextGroup;

        public void BeforeGet(object logicRule, string propertyName) {
            if (propertyName == "ExecutionContextGroup") {
                if (!gettingExecutionContextGroup) {
                    gettingExecutionContextGroup = true;
                    var rule = ((ILogicRule) logicRule);
                    if (string.IsNullOrEmpty(rule.ExecutionContextGroup))
                        rule.ExecutionContextGroup = LogicDefaultGroupContextNodeUpdater.Default;
                    gettingExecutionContextGroup = false;
                }
            }
        }
    }

}