using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using System.Linq;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries{
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