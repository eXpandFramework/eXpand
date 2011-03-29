using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries {
    public abstract class QueryDifferenceObject<TDifferenceObject> : IQueryDifferenceObject<TDifferenceObject> where TDifferenceObject : ModelDifferenceObject {
        private readonly Session _session;

        protected QueryDifferenceObject(Session session) {
            _session = session;
        }

        public Session Session {
            get { return _session; }
        }

        public virtual IQueryable<TDifferenceObject> GetActiveModelDifferences(string uniqueApplicationName, string name) {
            var differenceObjects = new XPQuery<TDifferenceObject>(_session);
            IQueryable<TDifferenceObject> differences = GetDifferences(differenceObjects, uniqueApplicationName, name);
            if (typeof(TDifferenceObject) == typeof(ModelDifferenceObject))
                differences = differences.Where(o => o.DifferenceType == DifferenceType.Model);
            return differences;
        }

        protected IQueryable<TDifferenceObject> GetDifferences(IOrderedQueryable<TDifferenceObject> differenceObjects, string uniqueApplicationName, string name) {
            IQueryable<TDifferenceObject> queryable = differenceObjects.Where(IsActiveExpressionCore(uniqueApplicationName));
            if (!(string.IsNullOrEmpty(name))) {
                queryable = queryable.Where(o => o.Name == name);
            }
            return queryable.OrderBy(o => o.CombineOrder);
        }

        public static Expression<Func<TDifferenceObject, bool>> IsActiveExpression(string uniqueApplicationName) {
            Expression<Func<TDifferenceObject, bool>> isActiveExpressionCore = IsActiveExpressionCore(uniqueApplicationName);
            return isActiveExpressionCore.And(IsActiveExpressionCore());
        }

        static Expression<Func<TDifferenceObject, bool>> IsActiveExpressionCore(string uniqueApplicationName) {
            return o => o.PersistentApplication.UniqueName == uniqueApplicationName && o.Disabled == false;
        }
        public virtual TDifferenceObject GetActiveModelDifference(string name,XafApplication application) {
            return GetActiveModelDifference(application.GetType().FullName, name);
        }

        public virtual TDifferenceObject GetActiveModelDifference(string applicationName, string name) {
            return GetActiveModelDifferences(applicationName, name).Where(IsActiveExpressionCore()).FirstOrDefault();
        }

        static Expression<Func<TDifferenceObject, bool>> IsActiveExpressionCore() {
            if (typeof(TDifferenceObject) == typeof(ModelDifferenceObject))
                return o => o.DifferenceType == DifferenceType.Model;
            if (typeof(TDifferenceObject) == typeof(UserModelDifferenceObject))
                return o => o.DifferenceType == DifferenceType.User;
            return o => o.DifferenceType == DifferenceType.Role;
        }
    }
}