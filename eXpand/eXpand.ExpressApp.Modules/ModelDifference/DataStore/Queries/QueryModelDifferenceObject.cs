using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using System.Linq;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public class QueryModelDifferenceObject : QueryDifferenceObject<ModelDifferenceObject>{
        public QueryModelDifferenceObject(Session session) : base(session){
        }

        public IQueryable<ModelDifferenceObject> GetModelDifferences(IEnumerable<string> names){
            return new XPQuery<ModelDifferenceObject>(Session).Where(o => names.Contains(o.Name));
        }

    }
}