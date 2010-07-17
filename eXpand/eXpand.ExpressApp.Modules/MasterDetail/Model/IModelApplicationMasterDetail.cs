using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.MasterDetail.Model {
    public interface IModelApplicationMasterDetail : IModelNode
    {
        [Description("Provides access to the MasterDetail node.")]
        IModelLogic MasterDetail { get; }
    }
}