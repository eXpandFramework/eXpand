using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic.Model {

    public interface IModelApplicationLogic : IModelNode {
        IModelLogic ModelLogic { get; }
    }
}
