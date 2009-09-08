using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public class QueryModelDifferenceObject : QueryDifferenceObject<ModelDifferenceObject>{
        public QueryModelDifferenceObject(Session session) : base(session){
        }

    }
}