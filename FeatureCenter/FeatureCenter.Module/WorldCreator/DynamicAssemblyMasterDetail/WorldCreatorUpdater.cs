using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater {
        public const string MasterDetailDynamicAssembly = "MasterDetailDynamicAssembly";
        private const string DMDOrder = "DMDOrder";

        private const string DMDOrderLine = "DMDOrderLine";

        public const string DMDCustomer = "DMDCustomer";

        public WorldCreatorUpdater(Session session)
            : base(session) {
        }

        public override void Update() {
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name == MasterDetailDynamicAssembly) == null) {
                var persistentAssemblyInfo = new DynamicAssemblyBuilder(Session).Build(DMDCustomer, DMDOrder, DMDOrderLine, MasterDetailDynamicAssembly);
                var persistentClassInfo = persistentAssemblyInfo.PersistentClassInfos.Where(info => info.Name == DMDCustomer).Single();
                var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(persistentClassInfo.Session);
                persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentVisibleInDetailViewAttribute(persistentCoreTypeMemberInfo.Session));
                persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentVisibleInListViewAttribute(persistentCoreTypeMemberInfo.Session));
                persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentVisibleInLookupListViewAttribute(persistentCoreTypeMemberInfo.Session));
                persistentCoreTypeMemberInfo.TypeAttributes.Add(new PersistentPersistentAliasAttribute(persistentCoreTypeMemberInfo.Session) { AliasExpression = "DMDOrders.Min(OrderDate)" });
                persistentCoreTypeMemberInfo.Name = "FirstOrderDate";
                persistentCoreTypeMemberInfo.DataType=DBColumnType.DateTime;
                var codeTemplateInfo = new CodeTemplateInfo(persistentCoreTypeMemberInfo.Session);
                var codeTemplate = new CodeTemplate(codeTemplateInfo.Session) { TemplateType = TemplateType.XPCalculatedPropertyMember };
                codeTemplate.SetDefaults();
                codeTemplate.Name = "CalculatedProperty";
                codeTemplateInfo.TemplateInfo = codeTemplate;

                persistentCoreTypeMemberInfo.CodeTemplateInfo = codeTemplateInfo;
                persistentClassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
                ObjectSpace.FindObjectSpaceByObject(persistentClassInfo).CommitChanges();
            }
        }

    }

}
