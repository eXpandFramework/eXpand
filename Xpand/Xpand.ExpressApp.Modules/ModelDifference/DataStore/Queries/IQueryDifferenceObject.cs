using System.Linq;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries{
    public interface IQueryDifferenceObject<DifferenceObject> where DifferenceObject : ModelDifferenceObject{
        Session Session { get; }
        IQueryable<DifferenceObject> GetActiveModelDifferences(string applicationName, string name);
        DifferenceObject GetActiveModelDifference(string applicationName, string name);
    }
}