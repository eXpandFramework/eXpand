using System;
using System.Linq;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core
{
    public class LinqServerCollectionSource : CollectionSource, ILinqCollectionSource
    {
        private readonly LinqCollectionHelper linqCollectionHelper = new LinqCollectionHelper();

        public LinqServerCollectionSource(ObjectSpace objectSpace, Type objectType, bool isServerMode)
            : base(objectSpace, objectType, isServerMode) { }

        public LinqServerCollectionSource(ObjectSpace objectSpace, Type objectType, bool isServerMode, IQueryable queryable)
            : this(objectSpace, objectType, isServerMode)
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

        protected override object CreateCollection()
        {
            if (Query != null)
                return linqCollectionHelper.ConvertQueryToCollection(Query);

            return base.CreateCollection();
        }
    }
}