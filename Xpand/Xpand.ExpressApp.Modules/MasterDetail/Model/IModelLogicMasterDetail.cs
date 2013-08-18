using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.MasterDetail.Model {
    public interface IModelLogicMasterDetail:IModelNode  {
        IModelMasterDetailLogicRules Rules { get; }
        IModelExecutionContextsGroup ExecutionContextsGroup { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelMasterDetailLogicRules:IModelNode,IModelList<IModelMasterDetailRule> {
    }
}