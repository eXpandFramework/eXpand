using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule {
    public class TopReturnedRecordsController:ViewController<ListView> {
        private const string TopReturnedRecords = "TopReturnedRecords";
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CollectionSource.CollectionChanged +=CollectionSourceOnCollectionChanged;
            
        }

        void CollectionSourceOnCollectionChanged(object sender, EventArgs eventArgs) {
//            var collection = View.CollectionSource.Collection;
//            var propertyCollectionSource = ((PropertyCollectionSource) View.CollectionSource);
//            XPBaseCollection xpBaseCollection = propertyCollectionSource.MemberInfo.GetValue(propertyCollectionSource.MasterObject) as XPBaseCollection;
//            if (xpBaseCollection != null) {
//
//            }
        }


        public override Schema GetSchema()
        {
            return new Schema(new SchemaBuilder().InjectAttribute(TopReturnedRecords, ModelElement.ListView));
        }
    }
}