using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using System.Linq;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public class QueryPersistentApplication
    {
        private readonly Session _session;

        public QueryPersistentApplication(Session session){
            _session = session;
        }

        public PersistentApplication Find(string uniqueName)
        {
            return new XPQuery<PersistentApplication>(_session).Where(application => application.UniqueName==uniqueName).FirstOrDefault();
        }
    }
}