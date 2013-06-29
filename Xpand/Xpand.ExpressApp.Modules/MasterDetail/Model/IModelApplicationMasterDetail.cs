using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.MasterDetail.Model {
    public interface IModelApplicationMasterDetail : IModelNode {
        [Description("Provides access to the MasterDetail node.")]
        IModelLogicMasterDetail MasterDetail { get; }
    }
}