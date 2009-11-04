using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public class QueryUserModelDifferenceObject:QueryDifferenceObject<UserModelDifferenceObject>{
        public QueryUserModelDifferenceObject(Session session) : base(session){
        }

        public override IQueryable<UserModelDifferenceObject> GetActiveModelDifferences(string applicationName)
        {
            var containsOperator = new ContainsOperator("Users",
                                                        new BinaryOperator("Oid", ((XPBaseObject)SecuritySystem.CurrentUser)
                                                                                     .ClassInfo.KeyProperty.GetValue(
                                                                                     SecuritySystem.CurrentUser)));
            IQueryable<UserModelDifferenceObject> queryable = base.GetActiveModelDifferences(applicationName).OfType<UserModelDifferenceObject>();
            return queryable.ToList().Where(o => o.Fit(containsOperator.ToString())).AsQueryable();
        }

        public override UserModelDifferenceObject GetActiveModelDifference(string applicationName)
        {
            var containsOperator = new ContainsOperator("Users",
                                                        new BinaryOperator("Oid", ((XPBaseObject)SecuritySystem.CurrentUser)
                                                                                     .ClassInfo.KeyProperty.GetValue(
                                                                                     SecuritySystem.CurrentUser)));
            var queryable = GetActiveModelDifferences(applicationName).OfType<UserModelDifferenceObject>();
            return queryable.ToList().Where(o => o.Fit(containsOperator.ToString())).FirstOrDefault();
        }
    }
}