using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ContextLogicRuleDomainLogic {
        public const string DefaultExecutionContextGroup = "Default";
        public static string Get_ExecutionContextGroup(ILogicRule modelNode) {
            return DefaultExecutionContextGroup;
        }
    }

    public class LogicRuleDomainLogic {
        public static ITypeInfo Get_TypeInfo(ILogicRule modelNode) {
            return ((IModelLogicRule)modelNode).ModelClass.TypeInfo;
        }

    }
}