using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public interface IQueryDifferenceObject<DifferenceObject> where DifferenceObject : ModelDifferenceObject{
        Session Session { get; }
        IQueryable<DifferenceObject> GetActiveModelDifferences(string applicationName, string modelId);
        DifferenceObject GetActiveModelDifference(string applicationName, string modelId);
    }
}