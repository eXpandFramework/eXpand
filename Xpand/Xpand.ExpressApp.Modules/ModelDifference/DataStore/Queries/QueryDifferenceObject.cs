using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.ModelDifference.DataStore.Queries
{
    public abstract class QueryDifferenceObject<TDifferenceObject> : IQueryDifferenceObject<TDifferenceObject> where TDifferenceObject : ModelDifferenceObject
    {
        private readonly Session _session;

        protected QueryDifferenceObject(Session session)
        {
            _session = session;
        }

        public Session Session
        {
            get { return _session; }
        }

        public virtual IQueryable<TDifferenceObject> GetActiveModelDifferences(string uniqueApplicationName, string name)
        {
            PersistentApplication application = _session.FindObject<PersistentApplication>(new BinaryOperator("UniqueName", uniqueApplicationName));
            if (application == null)
                return new List<TDifferenceObject>().AsQueryable();

            var differenceObjects = new XPQuery<TDifferenceObject>(_session);
            IQueryable<TDifferenceObject> differences = GetDifferences(differenceObjects, application, name);
            if (typeof(TDifferenceObject) == typeof(ModelDifferenceObject))
                differences = differences.Where(o => o.DifferenceType == DifferenceType.Model);
            return differences;
        }

        private IQueryable<TDifferenceObject> GetDifferences(IOrderedQueryable<TDifferenceObject> differenceObjects, PersistentApplication application, string name)
        {
            IQueryable<TDifferenceObject> queryable = differenceObjects.Where(IsActiveExpressionCore(application));
            if (!(string.IsNullOrEmpty(name)))
            {
                queryable = queryable.Where(o => o.Name == name);
            }
            return queryable.OrderBy(o => o.CombineOrder);
        }

        private static Expression<Func<TDifferenceObject, bool>> IsActiveExpression(PersistentApplication application)
        {
            Expression<Func<TDifferenceObject, bool>> isActiveExpressionCore = IsActiveExpressionCore(application);
            return isActiveExpressionCore.And(IsActiveExpressionCore());
        }

        static Expression<Func<TDifferenceObject, bool>> IsActiveExpressionCore(PersistentApplication application)
        {
            return o => o.PersistentApplication == application && o.Disabled == false;
        }
        public virtual TDifferenceObject GetActiveModelDifference(string name, XafApplication application)
        {
            return GetActiveModelDifference(application.GetType().FullName, name);
        }

        public virtual TDifferenceObject GetActiveModelDifference(string applicationName, string name)
        {
            return GetActiveModelDifferences(applicationName, name).Where(IsActiveExpressionCore()).FirstOrDefault();
        }

        static Expression<Func<TDifferenceObject, bool>> IsActiveExpressionCore()
        {
            if (typeof(TDifferenceObject) == typeof(ModelDifferenceObject))
                return o => o.DifferenceType == DifferenceType.Model;
            if (typeof(TDifferenceObject) == typeof(UserModelDifferenceObject))
                return o => o.DifferenceType == DifferenceType.User;
            return o => o.DifferenceType == DifferenceType.Role;
        }
    }
}