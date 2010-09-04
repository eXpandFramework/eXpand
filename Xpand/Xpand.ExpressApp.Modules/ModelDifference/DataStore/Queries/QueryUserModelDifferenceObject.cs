using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries
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

        public override IQueryable<UserModelDifferenceObject> GetActiveModelDifferences(string applicationName, string name)
        {
            return new XPCollection<UserModelDifferenceObject>(Session,new GroupOperator(UsersContainsOperator,
                new GroupOperator(new BinaryOperator("PersistentApplication.UniqueName",applicationName),new BinaryOperator("Disabled", false))),
                new[] {new SortProperty("CombineOrder",SortingDirection.Ascending)}).AsQueryable();
        }

        public override UserModelDifferenceObject GetActiveModelDifference(string applicationName, string name)
        {
            return GetActiveModelDifferences(applicationName, name).FirstOrDefault();
        }

}
}