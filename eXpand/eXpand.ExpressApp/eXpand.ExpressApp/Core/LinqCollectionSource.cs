using System;
using System.Linq;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core
{
    public class LinqCollectionSource : CollectionSource, ILinqCollectionSource
    {
        public const string DefaultSuffix = "_Linq";

        private readonly LinqCollectionHelper linqCollectionHelper = new LinqCollectionHelper();
        public LinqCollectionSource(ObjectSpace objectSpace, Type objectType, bool isServerMode) 
            : base(objectSpace, objectType, isServerMode) { }

        public LinqCollectionSource(ObjectSpace objectSpace, Type objectType, bool isServerMode, IQueryable query)
            : this(objectSpace, objectType, isServerMode)
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

        protected override object CreateCollection()
        {
            if (Query != null)
                return linqCollectionHelper.ConvertQueryToCollection(Query);

            return base.CreateCollection();
        }
    }
}