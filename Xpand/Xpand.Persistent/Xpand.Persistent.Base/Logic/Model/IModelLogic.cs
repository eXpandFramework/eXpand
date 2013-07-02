using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic.Model {
    public interface IModelLogic : IModelNode {
        IModelLogicRules Rules { get; }
        IModelExecutionContextsGroup ExecutionContextsGroup { get; }
        IModelActionExecutionContextGroup ActionExecutionContextGroup { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
    }
}