using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using eXpand.Utils.Linq;

namespace eXpand.ExpressApp.ModelDifference.DataStore.Queries{
    public abstract class QueryDifferenceObject<TDifferenceObject> : IQueryDifferenceObject<TDifferenceObject> where TDifferenceObject:ModelDifferenceObject{
        private readonly Session _session;

        protected QueryDifferenceObject(Session session){
            _session = session;
        }

        public Session Session{
            get { return _session; }
        }

        public virtual IQueryable<TDifferenceObject> GetActiveModelDifferences(string uniqueApplicationName, string modelId){
            var differenceObjects = new XPQuery<TDifferenceObject>(_session);
            IQueryable<TDifferenceObject> differences = GetDifferences(differenceObjects, uniqueApplicationName, modelId).Where(o => o.DifferenceType == DifferenceType.Model);
            return differences;
        }

        protected IQueryable<TDifferenceObject> GetDifferences(IOrderedQueryable<TDifferenceObject> differenceObjects, string uniqueApplicationName, string modelId)
        {
            IQueryable<TDifferenceObject> queryable = differenceObjects.Where(IsActiveExpressionCore(uniqueApplicationName));
            if (!(string.IsNullOrEmpty(modelId))) {
                queryable = queryable.Where(o => o.ModelId == modelId);
            }
            return queryable.OrderBy(o => o.CombineOrder);
        }

        public static Expression<Func<TDifferenceObject, bool>> IsActiveExpression(string uniqueApplicationName) {
            Expression<Func<TDifferenceObject, bool>> isActiveExpressionCore = IsActiveExpressionCore(uniqueApplicationName);
            return PredicateBuilder.And(isActiveExpressionCore, IsActiveExpressionCore());
        }

        static Expression<Func<TDifferenceObject, bool>> IsActiveExpressionCore(string uniqueApplicationName) {
            return o => o.PersistentApplication.UniqueName == uniqueApplicationName && o.Disabled == false;
        }
        public virtual TDifferenceObject GetActiveModelDifference(string modelId)
        {
            return GetActiveModelDifference(ModuleBase.Application.GetType().FullName,modelId);
        }

        public virtual TDifferenceObject GetActiveModelDifference(string applicationName, string modelId)
        {
            return GetActiveModelDifferences(applicationName,modelId).Where(IsActiveExpressionCore()).FirstOrDefault();
        }

        static Expression<Func<TDifferenceObject, bool>> IsActiveExpressionCore() {
            return o => o.DifferenceType==DifferenceType.Model;
        }
    }
}