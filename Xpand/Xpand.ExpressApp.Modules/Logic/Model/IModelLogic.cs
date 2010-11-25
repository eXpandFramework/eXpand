using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Logic.Model {
    public interface IModelLogic : IModelNode {
        IModelLogicRules Rules { get; }
        IModelExecutionContextsGroup ExecutionContextsGroup { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
    }
}