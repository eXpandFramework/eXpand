using DevExpress.Xpo;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyRuntimeOrphanedCollection {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater
    {

        private const string OrderLinesFromWorldCreator = "OrderLinesFromWorldCreator";
        public WorldCreatorUpdater(Session session) : base(session) {
        }
        public override void Update()
        {
            if (Session.FindObject<ExtendedOrphanedCollection>(info => info.Name == OrderLinesFromWorldCreator) != null) return;
            var extendedOrphanedCollection = new ExtendedOrphanedCollection(Session);
            extendedOrphanedCollection.TypeAttributes.Add(new PersistentVisibleInDetailViewAttribute(Session));
            extendedOrphanedCollection.TypeAttributes.Add(new PersistentVisibleInListViewAttribute(Session));
            extendedOrphanedCollection.TypeAttributes.Add(new PersistentVisibleInLookupListViewAttribute(Session));
            extendedOrphanedCollection.Owner = typeof (Customer);
            extendedOrphanedCollection.Criteria = "Order.Customer.Oid='@This.Oid'";
            extendedOrphanedCollection.Name = OrderLinesFromWorldCreator;
            extendedOrphanedCollection.ElementType = typeof (OrderLine);
            extendedOrphanedCollection.Save();
        }
    }
}