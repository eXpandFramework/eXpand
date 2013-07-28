using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Model {
    public interface IModelApplicationModelAdaptor : IModelNode {
        IModelModelAdaptorLogic ModelAdaptor { get; }
    }
}
