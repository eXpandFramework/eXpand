using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace ModelDifferenceTester.Module.FunctionalTests.WCModel {
    public class WCModelUpdater:ModuleUpdater {
        public WCModelUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

//        public override void UpdateDatabaseAfterUpdateSchema(){
//            base.UpdateDatabaseAfterUpdateSchema();
//            if (ObjectSpace.FindObject<PersistentAssemblyInfo>(info => info.Name=="TestAssembly")==null){
//                var assemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
//                var classInfo = ObjectSpace.CreateObject<PersistentClassInfo>();
//                assemblyInfo.PersistentClassInfos.Add(classInfo);
//                assemblyInfo.Name = "TestAssembly";
//                classInfo.Name = "TestClass";
//
//                classInfo.SetDefaultTemplate(TemplateType.Class);
//                var memberInfo = ObjectSpace.CreateObject<PersistentCoreTypeMemberInfo>();
//                classInfo.OwnMembers.Add(memberInfo);
//                memberInfo.Name = "TestProperty";
//                memberInfo.DataType=DBColumnType.String;
//                memberInfo.SetDefaultTemplate(TemplateType.XPReadWritePropertyMember);
//                
//            }
//            
//        }
    }
}
