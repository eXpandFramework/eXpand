using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class LogicRuleDomainLogic {
        public const string DefaultExecutionContextGroup = "Default";
        public static ITypeInfo Get_TypeInfo(ILogicRule modelNode) {
            return ((ILogicModelClassRule)modelNode).ModelClass.TypeInfo;
        }

        public static string Get_ExecutionContextGroup(ILogicRule modelNode) {
            return DefaultExecutionContextGroup;
        }
    }
}