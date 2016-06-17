using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace WorldCreatorTester.Module.FunctionalTests.OneToMany {
    public class OneToManyModuleUpdater:WorldCreatorModuleUpdater {
       
        public OneToManyModuleUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (!ObjectSpace.GetObjectsQuery<PersistentAssemblyInfo>().Any(info => info.Name == "OneToMany")) {
                var persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
                persistentAssemblyInfo.Name = "OneToMany";

                var projectCassInfo = persistentAssemblyInfo.CreateClass("Project");
                var contributorClassInfo = persistentAssemblyInfo.CreateClass("Contributor");

                projectCassInfo.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentDefaultClassOptionsAttribute>());
                projectCassInfo.CreateSimpleMember(DBColumnType.String, "Name");
                projectCassInfo.CreateCollection(contributorClassInfo).CreateAssociation("Project-Contributors");

                contributorClassInfo.TypeAttributes.Add(ObjectSpace.CreateObject<PersistentDefaultClassOptionsAttribute>());
                contributorClassInfo.CreateSimpleMember(DBColumnType.String, "Name");
                contributorClassInfo.CreateReferenceMember(projectCassInfo).CreateAssociation("Project-Contributors");
                ObjectSpace.CommitChanges();
            }
        }
    }
}
