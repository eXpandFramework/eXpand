using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public abstract class QueryDifferenceObject<TDifferenceObject> : IQueryDifferenceObject<TDifferenceObject> where TDifferenceObject:ModelDifferenceObject{
        private readonly Session _session;

        protected QueryDifferenceObject(Session session){
            _session = session;
        }

        public Session Session{
            get { return _session; }
        }

        public virtual IQueryable<TDifferenceObject> GetActiveModelDifferences(string uniqueApplicationName){
            var differenceObjects = new XPQuery<TDifferenceObject>(_session);
            IQueryable<TDifferenceObject> differences = GetDifferences(differenceObjects, uniqueApplicationName);
            return differences;
        }

        protected IQueryable<TDifferenceObject> GetDifferences(IOrderedQueryable<TDifferenceObject> differenceObjects, string uniqueApplicationName)
        {
            return differenceObjects.Where(o => o.PersistentApplication.UniqueName == uniqueApplicationName && o.Disabled == false).OrderBy(o => o.CombineOrder);
        }

        public virtual TDifferenceObject GetActiveModelDifference(string applicationName)
        {
            return GetActiveModelDifferences(applicationName).Where(o => o.DifferenceType==DifferenceType.Model).FirstOrDefault();
        }
    }
}