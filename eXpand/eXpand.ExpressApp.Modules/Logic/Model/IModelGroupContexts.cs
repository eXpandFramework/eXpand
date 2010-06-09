using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace eXpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(GroupContextsNodeGenerator))]
    public interface IModelGroupContexts : IModelNode, IModelList<IModelIModelExecutionContexts> {
        [DataSourceProperty("this")][Required]
        IModelIModelExecutionContexts DefaultContext { get; set; }
    }
}