using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    
    public class LogicRuleExecutionContextGroupDomainLogic {
        public static string Get_ExecutionContextGroup(ILogicRule modelNode) {
            return LogicDefaultGroupContextNodeUpdater<IModelLogic,IModelNode>.Default;
        }
    }

}