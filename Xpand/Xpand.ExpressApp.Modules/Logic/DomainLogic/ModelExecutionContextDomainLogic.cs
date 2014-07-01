using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelExecutionContext modelExecutionContext) {
            var modelLogic = modelExecutionContext.Parent;
            while (!(modelLogic is IEnumerable<IModelExecutionContext>)) {
                modelLogic = modelLogic.Parent;
            }
            var logicInstaller = LogicInstallerManager.Instance.LogicInstallers.First(installer =>IdMatch(modelExecutionContext, installer, modelLogic));
            return logicInstaller.ValidExecutionContexts.Select(context => context.ToString()).ToList();
        }

        private static bool IdMatch(IModelExecutionContext modelExecutionContext, ILogicInstaller installer, IModelNode modelLogic){
            var modelNode = (installer.GetModelLogic(modelExecutionContext.Application).ExecutionContextsGroup) as ModelNode;
            return modelNode != null && Equals(modelNode.Parent.Id,((ModelNode) modelLogic.Parent.Parent).Id);
        }
    }
}