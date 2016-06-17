using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyRuntimeCalculatedField {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {

        private const string MinOfOrderTotals = "MinOfOrderTotals";
        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.QueryObject<ExtendedCoreTypeMemberInfo>(info => info.Name == MinOfOrderTotals) != null) return;
            var extendedCoreTypeMemberInfo = ObjectSpace.CreateObject<ExtendedCoreTypeMemberInfo>();
            var persistentPersistentAliasAttribute = ObjectSpace.CreateObject<PersistentPersistentAliasAttribute>();
            persistentPersistentAliasAttribute.AliasExpression = "Orders.Min(Total)";
            extendedCoreTypeMemberInfo.TypeAttributes.Add(persistentPersistentAliasAttribute);
            extendedCoreTypeMemberInfo.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentVisibleInDetailViewAttribute>());
            extendedCoreTypeMemberInfo.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentVisibleInListViewAttribute>());
            extendedCoreTypeMemberInfo.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentVisibleInLookupListViewAttribute>());
            extendedCoreTypeMemberInfo.Owner = typeof(Customer);
            extendedCoreTypeMemberInfo.DataType = DBColumnType.Decimal;
            extendedCoreTypeMemberInfo.Name = MinOfOrderTotals;
            extendedCoreTypeMemberInfo.Save();

        }
    }
}