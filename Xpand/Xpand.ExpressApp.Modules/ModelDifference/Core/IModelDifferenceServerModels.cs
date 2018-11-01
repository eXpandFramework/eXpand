using System.Linq;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.Core{
    public interface IModelDifferenceServerModels {
        IQueryable<ModelDifferenceObject> Where(IQueryable<ModelDifferenceObject> modelDifferenceObjects);
    }
}