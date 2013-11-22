using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public static class CollectionSourceExtensions {
        public static CriteriaOperator GetCriteria(this CollectionSourceBase collectionSource) {
            var externalCriteria = (CriteriaOperator)collectionSource.CallMethod("GetExternalCriteria");
            var associatedCollectionCriteria = (CriteriaOperator)collectionSource.CallMethod("GetAssociatedCollectionCriteria");
            var method = typeof(CollectionSourceBase).GetMethod("CombineCriteria", BindingFlags.Static | BindingFlags.NonPublic);
            return (CriteriaOperator)method.Invoke(null, new object[] { new[] { associatedCollectionCriteria, externalCriteria } });
        }
    }
}