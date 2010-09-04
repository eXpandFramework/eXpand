using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(GroupContextsNodeGenerator))]
    public interface IModelGroupContexts : IModelNode, IModelList<IModelExecutionContexts> {
        [DataSourceProperty("this")][Required]
        IModelExecutionContexts DefaultContext { get; set; }
    }
}