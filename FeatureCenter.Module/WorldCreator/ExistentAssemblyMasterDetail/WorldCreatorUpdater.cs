using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {
    public class WorldCreatorUpdater : eXpand.ExpressApp.WorldCreator.WorldCreatorUpdater
    {
        public WorldCreatorUpdater(Session session) : base(session) {
        }
        public override void Update()
        {
            if (Session.FindObject<ExtendedCoreTypeMemberInfo>(info => info.Owner == typeof(EAMDCustomer)) != null) return;
            CreateCoreMemders();
            var extendedReferenceMemberInfo = new ExtendedReferenceMemberInfo(Session) { Name = "Customer", ReferenceType = typeof(EAMDCustomer), Owner = typeof(EAMDOrder) };
            extendedReferenceMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session) { AssociationName = "Customer-Orders" });
            extendedReferenceMemberInfo.Save();

            var extendedCollectionMemberInfo = new ExtendedCollectionMemberInfo(Session) { Name = "Orders", Owner = typeof(EAMDCustomer) };
            extendedCollectionMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(Session) { AssociationName = "Customer-Orders", ElementType = typeof(EAMDOrder) });
            extendedCollectionMemberInfo.Save();
            
        }

        void CreateCoreMemders() {
            CreateCoreMember("Name", typeof(EAMDCustomer), DBColumnType.String);

            CreateCoreMember("City", typeof(EAMDCustomer), DBColumnType.String);
            CreateCoreMember("Description", typeof(EAMDCustomer), DBColumnType.String, new PersistentSizeAttribute(Session) { Size = SizeAttribute.Unlimited });

            CreateCoreMember("Reference", typeof(EAMDOrder), DBColumnType.String);
            CreateCoreMember("OrderDate", typeof(EAMDOrder), DBColumnType.DateTime);
            CreateCoreMember("Total", typeof(EAMDOrder), DBColumnType.Decimal);
        }

        void CreateCoreMember(string name, Type owner, DBColumnType dataType, params PersistentAttributeInfo[] persistentAttributeInfos)
        {
            
            var extendedCoreTypeMemberInfo = new ExtendedCoreTypeMemberInfo(Session){Name = name,Owner = owner,DataType = dataType};
            if (persistentAttributeInfos != null)
                foreach (var persistentAttributeInfo in persistentAttributeInfos) {
                    extendedCoreTypeMemberInfo.TypeAttributes.Add(persistentAttributeInfo);
                }
            extendedCoreTypeMemberInfo.Save();
            return;
        }
    }
}