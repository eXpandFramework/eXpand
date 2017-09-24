using System.Linq;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries{
    public interface IQueryDifferenceObject<out TDifferenceObject> where TDifferenceObject : ModelDifferenceObject{
        Session Session { get; }
        IQueryable<TDifferenceObject> GetActiveModelDifferences(string applicationName, string name,DeviceCategory deviceCategory=DeviceCategory.All);
        TDifferenceObject GetActiveModelDifference(string applicationName, string name, DeviceCategory deviceCategory=DeviceCategory.All);
    }
}