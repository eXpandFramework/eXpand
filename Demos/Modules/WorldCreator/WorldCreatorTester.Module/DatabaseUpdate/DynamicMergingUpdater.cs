using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using WorldCreatorTester.Module.BusinessObjects;
using Xpand.ExpressApp.WorldCreator;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace WorldCreatorTester.Module.DatabaseUpdate {
    public class DynamicMergingUpdater : WorldCreatorUpdater {
        public DynamicMergingUpdater(Session session) : base(session) {
        }

        
        public override void Update() {
            var unitOfWork = new UnitOfWork(Session.DataLayer);

            if (unitOfWork.FindObject<PersistentAssemblyInfo>(null) != null)
                return;
            var persistentAssemblyInfo = new PersistentAssemblyInfo(unitOfWork){Name = "TestAssembly"};
            var persistentClassInfo = new PersistentClassInfo(unitOfWork){
                PersistentAssemblyInfo = persistentAssemblyInfo
            };
            persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
            persistentClassInfo.Name = "TestClassInfo";
            persistentClassInfo.BaseType = typeof (Customer);
            persistentClassInfo.MergedObjectType = typeof (Customer);
            persistentClassInfo.TypeAttributes.Add(new PersistentDefaultClassOptionsAttribute(unitOfWork));
            persistentClassInfo.TypeAttributes.Add(new PersistentMapInheritanceAttribute(unitOfWork){
                MapInheritanceType = MapInheritanceType.ParentTable
            });
            var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(unitOfWork){
                DataType = DBColumnType.String,
                Name = "TestProp"
            };
            persistentClassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
            persistentCoreTypeMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);
            

            unitOfWork.CommitChanges();
        }
    }
}