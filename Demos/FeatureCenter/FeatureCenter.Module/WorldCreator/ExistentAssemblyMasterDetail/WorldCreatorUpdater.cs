using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {
    public class WorldCreatorUpdater : WorldCreatorModuleUpdater {
        public WorldCreatorUpdater(IObjectSpace objectSpace, Version version)
            : base(objectSpace,version) {
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.QueryObject<ExtendedCoreTypeMemberInfo>(info => info.Owner == typeof(EAMDCustomer)) != null) return;
            CreateCoreMemders();
            var extendedReferenceMemberInfo = ObjectSpace.CreateObject<ExtendedReferenceMemberInfo>();
            extendedReferenceMemberInfo.Name = "Customer";
            extendedReferenceMemberInfo.ReferenceType = typeof(EAMDCustomer);
            extendedReferenceMemberInfo.Owner = typeof(EAMDOrder);
            var persistentAssociationAttribute = ObjectSpace.CreateObject<PersistentAssociationAttribute>();
            persistentAssociationAttribute.AssociationName = "Customer-Orders";
            extendedReferenceMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            extendedReferenceMemberInfo.Save();

            var extendedCollectionMemberInfo = ObjectSpace.CreateObject<ExtendedCollectionMemberInfo>();
            extendedCollectionMemberInfo.Name = "Orders";
            extendedCollectionMemberInfo.Owner = typeof(EAMDCustomer);
            var persistentAttributeInfo = ObjectSpace.CreateObject<PersistentAssociationAttribute>();
            persistentAttributeInfo.AssociationName = "Customer-Orders";
            persistentAttributeInfo.ElementType = typeof(EAMDOrder);
            extendedCollectionMemberInfo.TypeAttributes.Add(persistentAttributeInfo);
            extendedCollectionMemberInfo.Save();

        }

        void CreateCoreMemders() {
            CreateCoreMember("Name", typeof(EAMDCustomer), DBColumnType.String);

            CreateCoreMember("City", typeof(EAMDCustomer), DBColumnType.String);
            var persistentAttributeInfos = ObjectSpace.CreateObject<PersistentSizeAttribute>();
            persistentAttributeInfos.Size = SizeAttribute.Unlimited;
            CreateCoreMember("Description", typeof(EAMDCustomer), DBColumnType.String, persistentAttributeInfos);

            CreateCoreMember("Reference", typeof(EAMDOrder), DBColumnType.String);
            CreateCoreMember("OrderDate", typeof(EAMDOrder), DBColumnType.DateTime);
            CreateCoreMember("Total", typeof(EAMDOrder), DBColumnType.Decimal);
        }

        void CreateCoreMember(string name, Type owner, DBColumnType dataType, params PersistentAttributeInfo[] persistentAttributeInfos) {

            var extendedCoreTypeMemberInfo = ObjectSpace.CreateObject<ExtendedCoreTypeMemberInfo>();
            extendedCoreTypeMemberInfo.Name = name;
            extendedCoreTypeMemberInfo.Owner = owner;
            extendedCoreTypeMemberInfo.DataType = dataType;
            if (persistentAttributeInfos != null)
                foreach (var persistentAttributeInfo in persistentAttributeInfos) {
                    extendedCoreTypeMemberInfo.TypeAttributes.Add(persistentAttributeInfo);
                }
            extendedCoreTypeMemberInfo.Save();
        }
    }
}