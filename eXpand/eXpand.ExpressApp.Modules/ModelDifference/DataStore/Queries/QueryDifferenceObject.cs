using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public abstract class QueryDifferenceObject<DifferenceObject> : IQueryDifferenceObject<DifferenceObject> where DifferenceObject:ModelDifferenceObject{
        private readonly Session _session;

        protected QueryDifferenceObject(Session session){
            _session = session;
        }

        public Session Session{
            get { return _session; }
        }

        public virtual IQueryable<DifferenceObject> GetActiveModelDifferences(string uniqueApplicationName){
            IQueryable<DifferenceObject> differences = new XPQuery<DifferenceObject>(_session).Where(o => o.PersistentApplication.UniqueName == uniqueApplicationName && o.Disabled == false).OrderBy(o => o.CombineOrder);
            return differences;
        }

        public virtual DifferenceObject GetActiveModelDifference(string applicationName)
        {
            return GetActiveModelDifferences(applicationName).Where(o => o.DifferenceType==DifferenceType.Model).FirstOrDefault();
        }
    }
}