using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public class ModelExecutionContextDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelExecutionContext modelExecutionContext) {
            var modelLogic = modelExecutionContext.Parent;
            while (!(modelLogic is IEnumerable<IModelExecutionContext>)) {
                modelLogic = modelLogic.Parent;
            }
            var first = LogicInstallerManager.Instance.LogicInstallers.First(installer 
                =>Equals(((ModelNode) installer.GetModelLogic().ExecutionContextsGroup).Parent.Id,
                         ((ModelNode) modelLogic.Parent.Parent).Id));
            return first.ExecutionContexts.Select(context => context.ToString()).ToList();
        }

    }
}