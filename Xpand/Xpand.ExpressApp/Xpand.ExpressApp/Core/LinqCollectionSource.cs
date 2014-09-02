using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Core {
    public class LinqCollectionSource : CollectionSource {
        public const string DefaultSuffix = "_Linq";
        private IBindingList _collectionCore;
        public IList ConvertQueryToCollection(IQueryable sourceQuery) {
            _collectionCore = new BindingList<object>();
            foreach (var item in sourceQuery) { _collectionCore.Add(item); }
            return _collectionCore;
        }

        public IQueryable Query { get; set; }
        protected override void ApplyCriteriaCore(DevExpress.Data.Filtering.CriteriaOperator criteria) {

        }
        protected override object CreateCollection() {
            ((XPQueryBase)Query).Session = ((XPObjectSpace)ObjectSpace).Session;
            return ConvertQueryToCollection(Query);
        }
        public LinqCollectionSource(IObjectSpace objectSpace, Type objectType) : base(objectSpace, objectType) { }
        public LinqCollectionSource(IObjectSpace objectSpace, Type objectType, IQueryable query)
            : base(objectSpace, objectType) {
            Query = query;
        }
        public override bool? IsObjectFitForCollection(object obj) {
            return _collectionCore.Contains(obj);
        }
    }
}