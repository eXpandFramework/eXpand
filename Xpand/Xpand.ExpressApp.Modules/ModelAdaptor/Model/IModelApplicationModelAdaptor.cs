using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ModelAdaptor.Model {
    public interface IModelApplicationModelAdaptor : IModelNode {
        IModelModelAdaptorLogic ModelAdaptor { get; }
    }
}
