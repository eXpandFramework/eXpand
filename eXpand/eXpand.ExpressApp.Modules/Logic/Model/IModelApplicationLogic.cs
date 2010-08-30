using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Logic.Model
{

    public interface IModelApplicationLogic:IModelNode
    {
        IModelLogic ModelLogic { get; }
    }
}
