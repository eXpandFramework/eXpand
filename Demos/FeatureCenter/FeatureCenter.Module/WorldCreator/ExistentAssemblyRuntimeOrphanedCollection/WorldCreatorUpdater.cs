using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyRuntimeOrphanedCollection {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {

        private const string OrderLinesFromWorldCreator = "OrderLinesFromWorldCreator";
        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.QueryObject<ExtendedOrphanedCollection>(info => info.Name == OrderLinesFromWorldCreator) != null) return;
            var extendedOrphanedCollection = ObjectSpace.CreateObject<ExtendedOrphanedCollection>();
            extendedOrphanedCollection.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentVisibleInDetailViewAttribute>());
            extendedOrphanedCollection.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentVisibleInListViewAttribute>());
            extendedOrphanedCollection.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentVisibleInLookupListViewAttribute>());
            extendedOrphanedCollection.Owner = typeof(Customer);
            extendedOrphanedCollection.Criteria = "Order.Customer.Oid='@This.Oid'";
            extendedOrphanedCollection.Name = OrderLinesFromWorldCreator;
            extendedOrphanedCollection.ElementType = typeof(OrderLine);
            extendedOrphanedCollection.Save();

        }
    }
}