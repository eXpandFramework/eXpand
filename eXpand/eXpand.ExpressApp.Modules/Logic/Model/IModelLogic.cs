using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Logic.Model {
    
    public interface IModelLogic : IModelNode {
        [Description("Provides access to the Rules node.")]
        IModelLogicRules Rules { get; }

        [Description("Provides access to the context node.")]
        IModelGroupContexts GroupContexts { get; }
    }
}