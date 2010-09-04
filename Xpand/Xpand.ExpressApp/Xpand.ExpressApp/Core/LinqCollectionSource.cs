using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Core
{
    public class LinqCollectionSource : CollectionSource
    {
        public const string DefaultSuffix = "_Linq";
        private IBindingList collectionCore;
        public IList ConvertQueryToCollection(IQueryable sourceQuery)
        {
            collectionCore = new BindingList<object>();
            foreach (var item in sourceQuery) { collectionCore.Add(item); }
            return collectionCore;
        }

        public IQueryable Query { get; set; }

        protected override object CreateCollection()
        {
            ((XPQueryBase)Query).Session = ObjectSpace.Session;
            return ConvertQueryToCollection(Query);
        }
        public LinqCollectionSource(ObjectSpace objectSpace, Type objectType) : base(objectSpace, objectType) { }
        public LinqCollectionSource(ObjectSpace objectSpace, Type objectType, IQueryable query)
            : base(objectSpace, objectType)
        {
            Query = query;
        }
        public override bool? IsObjectFitForCollection(object obj)
        {
            return collectionCore.Contains(obj);
        }
    }
}