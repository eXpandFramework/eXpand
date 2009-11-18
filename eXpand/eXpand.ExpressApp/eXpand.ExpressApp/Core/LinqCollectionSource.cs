using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Core
{
    public class LinqCollectionSource : CollectionSource, ILinqCollectionSource
    {
        public const string DefaultSuffix = "_Linq";

        private readonly LinqCollectionHelper linqCollectionHelper = new LinqCollectionHelper();
        public LinqCollectionSource(ObjectSpace objectSpace, Type objectType) : base(objectSpace, objectType)
        {
        }

        public LinqCollectionSource(ObjectSpace objectSpace, Type objectType, IQueryable query)
            : base(objectSpace, objectType)
        {
            Query = query;
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