using System.Collections.Generic;
using System.Linq;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelExecutionContext modelExecutionContext) {
            var modelLogic = modelExecutionContext.Parent;
            while (!(modelLogic is IModelLogic)) {
                modelLogic = modelLogic.Parent;
            }
            var typeInfo = ((ITypesInfoProvider) modelLogic.Application).TypesInfo.FindTypeInfo(modelLogic.GetType());
            var modelLogicInterface = typeInfo.ImplementedInterfaces.First(info 
                => typeof (IModelLogic) != info.Type && typeof (IModelLogic).IsAssignableFrom(info.Type));
            return LogicInstallerManager.Instance.LogicInstallers.First(logicInstaller
                => ReferenceEquals(logicInstaller.GetModelLogic().GetType(), modelLogicInterface)).ExecutionContexts.Select(context 
                    => context.ToString()).ToList();

        }

    }
}