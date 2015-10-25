using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Email.Model {
    public interface IModelApplicationEmail : IModelNode {
        IModelLogicEmail Email { get; }
    }
}