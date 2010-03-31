using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Core
{
    public class LinqServerCollectionSource : ServerCollectionSource, ILinqCollectionSource
    {
        private readonly LinqCollectionHelper linqCollectionHelper = new LinqCollectionHelper();
        public LinqServerCollectionSource(ObjectSpace objectSpace, Type objectType)
            : base(objectSpace, objectType)
        {
        }


        public LinqServerCollectionSource(ObjectSpace objectSpace, Type objectType, IQueryable queryable)
            : base(objectSpace, objectType)
        {
            Query = queryable;
        }
        public IQueryable Query
        {
            get { return linqCollectionHelper.Query; }
            set
            {
                linqCollectionHelper.Query = value;
            }
        }
        protected override object RecreateCollection(CriteriaOperator criteria, SortingCollection sortings)
        {
            if (Query != null)
                return linqCollectionHelper.ConvertQueryToCollection(Query);
            return base.RecreateCollection(criteria, sortings);
        }
    }
}