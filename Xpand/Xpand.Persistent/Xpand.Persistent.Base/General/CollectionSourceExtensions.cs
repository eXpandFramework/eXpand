using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public static class CollectionSourceExtensions {
        public static CriteriaOperator GetAssociatedCollectionCriteria(this CollectionSourceBase collectionSource) {
            return (CriteriaOperator) collectionSource.CallMethod("GetAssociatedCollectionCriteria");
        }

        public static CriteriaOperator GetExternalCriteria(this CollectionSourceBase collectionSource) {
            return (CriteriaOperator)collectionSource.CallMethod("GetExternalCriteria");
        }

        public static CriteriaOperator GetCriteria(this CollectionSourceBase collectionSource) {
            var externalCriteria = collectionSource.GetExternalCriteria();
            var associatedCollectionCriteria = collectionSource.GetAssociatedCollectionCriteria();
            return (CriteriaOperator) typeof(CollectionSourceBase).CallMethod("CombineCriteria",  new object[] { new[] { associatedCollectionCriteria, externalCriteria } });
        }
    }
}