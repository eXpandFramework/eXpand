using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.Logic {
    public interface ILogicInstaller {
        List<ExecutionContext> ExecutionContexts { get; }
        IModelLogic GetModelLogic(IModelApplication applicationModel);
        IModelLogic GetModelLogic();
    }

}