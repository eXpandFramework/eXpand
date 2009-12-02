using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries
{
    public class QueryUserModelDifferenceObject : QueryDifferenceObject<UserModelDifferenceObject>
    {
        public QueryUserModelDifferenceObject(Session session)
            : base(session)
        {
        }

        private static ContainsOperator UsersContainsOperator
        {
            get
            {
                XPMemberInfo mi = ((XPBaseObject)SecuritySystem.CurrentUser).ClassInfo.KeyProperty;
                return new ContainsOperator("Users", new BinaryOperator(mi.Name, mi.GetValue(SecuritySystem.CurrentUser)));
            }
        }

        public override IQueryable<UserModelDifferenceObject> GetActiveModelDifferences(string applicationName)
        {
            var queryable = base.GetActiveModelDifferences(applicationName).OfType<UserModelDifferenceObject>();
            return queryable.ToList().Where(o => o.Fit(UsersContainsOperator)).AsQueryable();
        }

        public override UserModelDifferenceObject GetActiveModelDifference(string applicationName)
        {
            var queryable = this.GetActiveModelDifferences(applicationName).OfType<UserModelDifferenceObject>();
            return queryable.ToList().Where(o => o.Fit(UsersContainsOperator)).FirstOrDefault();
        }
    }
}