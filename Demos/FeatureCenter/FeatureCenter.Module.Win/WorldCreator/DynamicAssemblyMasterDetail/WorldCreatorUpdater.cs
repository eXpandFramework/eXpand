using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {
        private const string DMDOrder = "DMDOrder";
        private const string DMDOrderLine = "DMDOrderLine";

        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.QueryObject<PersistentAssemblyInfo>(info => info.Name == DynamicAssemblyCalculatedField.AttributeRegistrator.MasterDetailDynamicAssembly) == null) {
                var customer = DynamicAssemblyCalculatedField.AttributeRegistrator.DMDCustomer;
                var persistentAssemblyInfo = new DynamicAssemblyBuilder(ObjectSpace).Build(customer, DMDOrder,
                        DMDOrderLine, DynamicAssemblyCalculatedField.AttributeRegistrator.MasterDetailDynamicAssembly);

                var persistentClassInfo = persistentAssemblyInfo.PersistentClassInfos.First(info => info.Name == customer);
                var persistentCoreTypeMemberInfo = persistentClassInfo.CreateSimpleMember(DBColumnType.DateTime, "FirstOrderDate");
                AddAttributes(persistentCoreTypeMemberInfo);
                persistentCoreTypeMemberInfo.CodeTemplateInfo = CreateCalculatedCodeTemplateInfo(persistentCoreTypeMemberInfo);
            }

        }

        private static void AddAttributes(IPersistentCoreTypeMemberInfo persistentCoreTypeMemberInfo){
            persistentCoreTypeMemberInfo.TypeAttributes.Add(
                new PersistentVisibleInDetailViewAttribute(persistentCoreTypeMemberInfo.Session));
            persistentCoreTypeMemberInfo.TypeAttributes.Add(
                new PersistentVisibleInListViewAttribute(persistentCoreTypeMemberInfo.Session));
            persistentCoreTypeMemberInfo.TypeAttributes.Add(
                new PersistentVisibleInLookupListViewAttribute(persistentCoreTypeMemberInfo.Session));
            persistentCoreTypeMemberInfo.TypeAttributes.Add(
                new PersistentPersistentAliasAttribute(persistentCoreTypeMemberInfo.Session){
                    AliasExpression = "DMDOrders.Min(OrderDate)"
                });
        }

        private static CodeTemplateInfo CreateCalculatedCodeTemplateInfo(IPersistentCoreTypeMemberInfo persistentCoreTypeMemberInfo){
            var codeTemplateInfo = new CodeTemplateInfo(persistentCoreTypeMemberInfo.Session);
            var codeTemplate = new CodeTemplate(codeTemplateInfo.Session){
                TemplateType = TemplateType.XPCalculatedPropertyMember
            };
            codeTemplate.SetDefaults();
            codeTemplate.Name = "CalculatedProperty";
            codeTemplateInfo.TemplateInfo = codeTemplate;
            return codeTemplateInfo;
        }
    }

}
