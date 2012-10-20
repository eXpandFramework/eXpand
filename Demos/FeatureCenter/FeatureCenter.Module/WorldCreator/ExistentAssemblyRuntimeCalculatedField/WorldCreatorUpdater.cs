using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyRuntimeCalculatedField {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater {

        private const string MinOfOrderTotals = "MinOfOrderTotals";
        public WorldCreatorUpdater(Session session)
            : base(session) {
        }
        public override void Update() {
            if (Session.FindObject<ExtendedCoreTypeMemberInfo>(info => info.Name == MinOfOrderTotals) != null) return;
            var extendedCoreTypeMemberInfo = new ExtendedCoreTypeMemberInfo(Session);
            extendedCoreTypeMemberInfo.TypeAttributes.Add(new PersistentPersistentAliasAttribute(Session) { AliasExpression = "Orders.Min(Total)" });
            extendedCoreTypeMemberInfo.TypeAttributes.Add(new PersistentVisibleInDetailViewAttribute(Session));
            extendedCoreTypeMemberInfo.TypeAttributes.Add(new PersistentVisibleInListViewAttribute(Session));
            extendedCoreTypeMemberInfo.TypeAttributes.Add(new PersistentVisibleInLookupListViewAttribute(Session));
            extendedCoreTypeMemberInfo.Owner = typeof(Customer);
            extendedCoreTypeMemberInfo.DataType = DBColumnType.Decimal;
            extendedCoreTypeMemberInfo.Name = MinOfOrderTotals;
            extendedCoreTypeMemberInfo.Save();
        }
    }
}