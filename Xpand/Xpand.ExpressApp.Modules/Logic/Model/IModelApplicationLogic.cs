using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Logic.Model
{

    public interface IModelApplicationLogic:IModelNode
    {
        IModelLogic ModelLogic { get; }
    }
}
